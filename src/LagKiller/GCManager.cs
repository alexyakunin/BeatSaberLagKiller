using System;
using System.Diagnostics;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using LagKiller.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

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
        public double DroppedFrameRatio => DroppedFrameCount / (double) FrameCount;
        public double DroppedFrameFrequency => DroppedFrameCount / PlayTime;
        public double LagRatio => LagCount / (double) FrameCount;
        public double LagFrequency => LagCount / PlayTime;
        public double GCIncompleteRatio => GCIncompleteCount / (double) FrameCount;
        public double GCTimeRatio => GCTime / PlayTime;

        public GCManager()
        {
            ResetStatistics();
            GarbageCollector.GCModeChanged += GCModeChanged;
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            SettingsChanged(PluginConfig.Instance);
            PluginConfig.Instance.OnChangedEvent += SettingsChanged;
        }

        private void SettingsChanged(PluginConfig settings)
        {
            GCBudget = settings.IsEnabled ? (float?) settings.GCBudget : null;
            FrameDropDuration = 1 / settings.FrameDropFpsBoundary;
            LagDuration = 1 / settings.LagFpsBoundary;
            ApplyGCModePeriod = settings.ApplyGCModePeriod;
            GameStartupDuration = settings.GameStartupDuration;
        }

        public void ResetStatistics()
        {
            Log?.Debug("Reset statistics.");
            Log?.Debug(GCInfo.GetUnityDescription());
            GCStopwatch = new Stopwatch();
            FrameCount = 1;
            PlayTime = 0.001f;
            GameTime = PlayTime;
            DroppedFrameCount = 0;
            LagCount = 0;
            GCIncompleteCount = 0;
            GCTime = 0f;
        }

        private void Update()
        {
            var timeDelta = Time.deltaTime;
            if (IsInGameCore) {
                FrameCount++;
                PlayTime += timeDelta;
                GameTime += timeDelta;
                if (GameTime < GameStartupDuration) {
                    if (timeDelta < FrameDropDuration)
                        GC(false);
                }
                else {
                    if (timeDelta > LagDuration) {
                        Log?.Debug($"Lag: {timeDelta * 1000:F2}ns");
                        LagCount++;
                        DroppedFrameCount++;
                    }
                    else if (timeDelta > FrameDropDuration)
                        DroppedFrameCount++;
                    else 
                        GC();
                }
            }
            
            ApplyGCModeTimer -= timeDelta;
            if (ApplyGCModeTimer < 0f) {
                ApplyGCModeTimer = ApplyGCModePeriod;
                ApplyGCMode();
            }
        }

        private void GC(bool captureMetrics = true)
        {
            var gcBudget = GCBudget.GetValueOrDefault();
            if (GCBudget <= 0)
                return;
            GCStopwatch.Restart();
            var isIncomplete = GarbageCollector.CollectIncremental((ulong) (gcBudget * 1000_000));
            GCStopwatch.Stop();
            if (!captureMetrics)
                return;
            GCIncompleteCount += isIncomplete ? 1 : 0;
            GCTime += GCStopwatch.Elapsed.TotalSeconds;
        }

        private void ActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            var sceneName = nextScene.name;
            Log?.Debug($"Scene changed to: {sceneName}");
            if (sceneName.Contains("Menu") && sceneName != "MenuViewControllers")
                GameTime = 0;
            IsInGameCore = sceneName == "GameCore";
            ApplyGCModeTimer = FrameDropDuration;
        }

        private void GCModeChanged(GarbageCollector.Mode mode)
        {
            ApplyGCModeTimer = FrameDropDuration;
            Log?.Debug($"GC mode changed.");
        }

        private void ApplyGCMode()
        {
            if (!GCBudget.HasValue)
                return;
            var gcMode = GarbageCollector.Mode.Enabled;
            if (GarbageCollector.GCMode != gcMode) {
                Log?.Debug($"GarbageCollector.GCMode: {GarbageCollector.GCMode} -> {gcMode}");
                GarbageCollector.GCMode = gcMode;
            }
            LogStatistics();
        }

        private void LogStatistics()
        {
            Log?.Debug($"Performance: " +
                $"{LagRatio:P} lags, {DroppedFrameRatio:P} frames dropped, " +
                $"{GCIncompleteRatio:P} incomplete GCs, {GCTimeRatio:P} time in GC");
        }
    }
}
