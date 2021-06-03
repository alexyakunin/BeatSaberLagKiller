using HMUI;
using LagKiller.Configuration;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace LagKiller
{
    public class GCManager : PersistentSingleton<GCManager>
    {
        private static IPA.Logging.Logger Log => Plugin.Log;
        private Stopwatch GCStopwatch { get; set; }
        private float ApplyGCModeTimer { get; set; }
        public bool IsInGameCore { get; private set; }
        public float? GCBudget { get; private set; }
        public float FrameDropDuration { get; private set; }
        public float LagDuration { get; private set; }
        public float ApplyGCModePeriod { get; private set; }
        public float GameStartupDuration { get; private set; }
        public double PlayTime { get; private set; }
        public double GameTime { get; private set; }
        public long FrameCount { get; private set; }
        public int DroppedFrameCount { get; private set; }
        public int LagCount { get; private set; }
        public int GCIncompleteCount { get; private set; }
        public double GCTime { get; private set; }
        public double DroppedFrameRatio => this.DroppedFrameCount / (double)this.FrameCount;
        public double DroppedFrameFrequency => this.DroppedFrameCount / this.PlayTime;
        public double LagRatio => this.LagCount / (double)this.FrameCount;
        public double LagFrequency => this.LagCount / this.PlayTime;
        public double GCIncompleteRatio => this.GCIncompleteCount / (double)this.FrameCount;
        public double GCTimeRatio => this.GCTime / this.PlayTime;

        private TextMeshProUGUI notifyText;
        private readonly CurvedCanvasSettings curvedCanvasSettings;
        private Canvas canvas;

        private void SettingsChanged(PluginConfig settings)
        {
            this.GCBudget = settings.IsEnabled ? (float?)settings.GCBudget : null;
            this.FrameDropDuration = 1 / settings.FrameDropFpsBoundary;
            this.LagDuration = 1 / settings.LagFpsBoundary;
            this.ApplyGCModePeriod = settings.ApplyGCModePeriod;
            this.GameStartupDuration = settings.GameStartupDuration;
        }

        public void ResetStatistics()
        {
            Log?.Debug("Reset statistics.");
            Log?.Debug(GCInfo.GetUnityDescription());
            this.GCStopwatch = new Stopwatch();
            this.FrameCount = 1;
            this.PlayTime = 0.001f;
            this.GameTime = this.PlayTime;
            this.DroppedFrameCount = 0;
            this.LagCount = 0;
            this.GCIncompleteCount = 0;
            this.GCTime = 0f;
        }
        #region // Unity message
        private void Awake()
        {
            this.ResetStatistics();
            GarbageCollector.GCModeChanged += this.GCModeChanged;
            SceneManager.activeSceneChanged += this.ActiveSceneChanged;
            this.SettingsChanged(PluginConfig.Instance);
            PluginConfig.Instance.OnChangedEvent += this.SettingsChanged;
        }

        private IEnumerator Start()
        {
            if (!GarbageCollector.isIncremental) {
                this.canvas = this.gameObject.AddComponent<Canvas>();
                yield return new WaitWhile(() => !this.canvas);
                (this.canvas.transform as RectTransform).sizeDelta = new Vector2(3f, 2f);
                this.canvas.transform.localPosition = new Vector3(0f, 1.5f, 2.4f);
                var content = this.gameObject.AddComponent<ContentSizeFitter>();
                yield return new WaitWhile(() => !content);
                content.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                content.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                this.notifyText = this.canvas.gameObject.AddComponent<TextMeshProUGUI>();
                yield return new WaitWhile(() => !this.notifyText);
                this.notifyText.text = "Lag Killer - Please restart BeatSaber.";
                this.notifyText.fontSize = 0.3f;
                this.notifyText.color = Color.red;
            }
        }

        protected override void OnDestroy()
        {
            GarbageCollector.GCModeChanged -= this.GCModeChanged;
            SceneManager.activeSceneChanged -= this.ActiveSceneChanged;
            PluginConfig.Instance.OnChangedEvent -= this.SettingsChanged;
            base.OnDestroy();
        }

        private void Update()
        {
            var timeDelta = Time.deltaTime;
            if (this.IsInGameCore) {
                this.FrameCount++;
                this.PlayTime += timeDelta;
                this.GameTime += timeDelta;
                if (this.GameTime < this.GameStartupDuration) {
                    if (timeDelta < this.FrameDropDuration)
                        this.GC(false);
                }
                else {
                    if (timeDelta > this.LagDuration) {
                        Log?.Debug($"Lag: {timeDelta * 1000:F2}ns");
                        this.LagCount++;
                        this.DroppedFrameCount++;
                    }
                    else if (timeDelta > this.FrameDropDuration)
                        this.DroppedFrameCount++;
                    else
                        this.GC();
                }
            }

            this.ApplyGCModeTimer -= timeDelta;
            if (this.ApplyGCModeTimer < 0f) {
                this.ApplyGCModeTimer = this.ApplyGCModePeriod;
                this.ApplyGCMode();
            }
        }
        #endregion

        private void GC(bool captureMetrics = true)
        {
            var gcBudget = this.GCBudget.GetValueOrDefault();
            if (this.GCBudget <= 0)
                return;
            this.GCStopwatch.Restart();
            var isIncomplete = GarbageCollector.CollectIncremental((ulong)(gcBudget * 1000_000));
            this.GCStopwatch.Stop();
            if (!captureMetrics)
                return;
            this.GCIncompleteCount += isIncomplete ? 1 : 0;
            this.GCTime += this.GCStopwatch.Elapsed.TotalSeconds;
        }

        private void ActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            var sceneName = nextScene.name;
            Log?.Debug($"Scene changed to: {sceneName}");
            if (sceneName.Contains("Menu") && sceneName != "MenuViewControllers")
                this.GameTime = 0;
            this.IsInGameCore = sceneName == "GameCore";
            this.ApplyGCModeTimer = this.FrameDropDuration;
        }

        private void GCModeChanged(GarbageCollector.Mode mode)
        {
            this.ApplyGCModeTimer = this.FrameDropDuration;
            Log?.Debug($"GC mode changed.");
        }

        private void ApplyGCMode()
        {
            if (!this.GCBudget.HasValue)
                return;
            var gcMode = GarbageCollector.Mode.Enabled;
            if (GarbageCollector.GCMode != gcMode) {
                Log?.Debug($"GarbageCollector.GCMode: {GarbageCollector.GCMode} -> {gcMode}");
                GarbageCollector.GCMode = gcMode;
            }
            this.LogStatistics();
        }

        private void LogStatistics() => Log?.Debug($"Performance: " +
                $"{this.LagRatio:P} lags, {this.DroppedFrameRatio:P} frames dropped, " +
                $"{this.GCIncompleteRatio:P} incomplete GCs, {this.GCTimeRatio:P} time in GC");
    }
}
