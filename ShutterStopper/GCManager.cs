using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace ShutterStopper
{
    public class GCManager : PersistentSingleton<GCManager>
    {
        public static readonly float ApplyGCModePeriod = 5f; // in seconds
        public static readonly float ShutterDuration = 1f/70; // in seconds
        public static readonly float LagDuration = 1f/10; // in seconds

        private static IPA.Logging.Logger Log => Plugin.Log;
        private Stopwatch Stopwatch { get; }
        private float ApplyGCModeTimer { get; set; }
        public bool IsInGameCore { get; private set; }
        public float? GCBudget { get; private set; }

        public double GameTime { get; private set; }
        public int ShutterCount { get; private set; }
        public int LagCount { get; private set; }
        public int GCOverBudgetCount { get; private set; }
        public double GCTime { get; private set; }
        public double MaxGCTime { get; private set; }
        
        public double ShutterFrequency => ShutterCount / GameTime;
        public double LagFrequency => LagCount / GameTime;
        public double GCOverBudgetFrequency => GCOverBudgetCount / GameTime;
        public double GCTimeToBudgetRatio => GCTime / MaxGCTime;

        public GCManager()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            GarbageCollector.GCModeChanged += GCModeChanged;
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            Settings.Instance.Changed += SettingsChanged;
            SettingsChanged(Settings.Instance);
            ResetStatistics();
        }

        public void ResetStatistics()
        {
            Log?.Debug("Reset statistics.");
            GameTime = 0.001f;
            ShutterCount = 0;
            LagCount = 0;
            GCOverBudgetCount = 0;
            GCTime = 0f;
            MaxGCTime = GameTime;
        }

        private void Update()
        {
            var timeDelta = Time.deltaTime;
            if (IsInGameCore) {
                GameTime += timeDelta;
                if (timeDelta > LagDuration) {
                    Log?.Debug($"Lag: {timeDelta*1000:F2}ns");
                    LagCount++;
                }
                else if (timeDelta > ShutterDuration)
                    ShutterCount++;
                else if (GCBudget.HasValue) {
                    var gcBudget = (ulong) (GCBudget.GetValueOrDefault() * 1000_000);
                    var gcStart = Stopwatch.Elapsed;
                    GarbageCollector.CollectIncremental(gcBudget);
                    var gcTimeDelta = (Stopwatch.Elapsed - gcStart).TotalMilliseconds;
                    GCTime += gcTimeDelta;
                    MaxGCTime += gcBudget;
                    if (gcTimeDelta > gcBudget)
                        GCOverBudgetCount++;
                }
            }
            
            ApplyGCModeTimer -= timeDelta;
            if (ApplyGCModeTimer < 0f) {
                ApplyGCModeTimer = ApplyGCModePeriod;
                ApplyGCMode();
            }
        }

        private void ActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            IsInGameCore = nextScene.name == "GameCore";
            ApplyGCModeTimer = ShutterDuration;
            Log?.Debug($"Scene changed to: {nextScene.name}");
        }

        private void GCModeChanged(GarbageCollector.Mode mode)
        {
            ApplyGCModeTimer = ShutterDuration;
            Log?.Debug($"GC mode changed.");
        }

        private void SettingsChanged(Settings settings) 
            => GCBudget = settings.IsEnabled ? (float?) settings.GCBudget : null;

        private void ApplyGCMode()
        {
            if (!GCBudget.HasValue)
                return;
            var gcMode = GarbageCollector.Mode.Enabled;
            if (GarbageCollector.GCMode != gcMode) {
                Log?.Debug($"GarbageCollector.GCMode: {GarbageCollector.GCMode} -> {gcMode}");
                GarbageCollector.GCMode = gcMode;
            }
        }

        private void LogStatistics()
        {
            Log?.Debug($"In game: {IsInGameCore}, GCBudget: {GCBudget}");
            Log?.Debug($"Statistics: {GameTime}s playing -> " +
                $"{LagCount} lags, {ShutterCount} shatters, " +
                $"{GCOverBudgetCount} over-budget GCs, {GCTimeToBudgetRatio:P} in GC");
        }
    }
}
