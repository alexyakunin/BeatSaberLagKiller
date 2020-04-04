using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace LagKiller
{
    public class GCManager : PersistentSingleton<GCManager>
    {
        public static readonly float ApplyGCModePeriod = 30f; // in seconds
        public static readonly float MaxFrameDuration = 1f/70; // in seconds
        public static readonly float LagDuration = 1f/20; // in seconds

        private static IPA.Logging.Logger Log => Plugin.Log;
        private Stopwatch Stopwatch { get; }
        private float ApplyGCModeTimer { get; set; }
        public bool IsInGameCore { get; private set; }
        public float? GCBudget { get; private set; }

        public double GameTime { get; private set; }
        public long FrameCount { get; private set; }
        public int DroppedFrameCount { get; private set; }
        public int LagCount { get; private set; }
        public int GCIncompleteCount { get; private set; }
        public double GCTime { get; private set; }
        
        public double DroppedFrameRatio => DroppedFrameCount / (double) FrameCount;
        public double LagRatio => LagCount / (double) FrameCount;
        public double LagFrequency => LagCount / GameTime;
        public double GCIncompleteRatio => GCIncompleteCount / (double) FrameCount;
        public double GCTimeRatio => GCTime / GameTime;

        public GCManager()
        {
            Stopwatch = new Stopwatch();
            GarbageCollector.GCModeChanged += GCModeChanged;
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            Settings.Instance.Changed += SettingsChanged;
            SettingsChanged(Settings.Instance);
            ResetStatistics();
        }

        public void ResetStatistics()
        {
            Log?.Debug("Reset statistics.");
            Log?.Debug(GCInfo.GetUnityDescription());
            FrameCount = 1;
            GameTime = 0.001f;
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
                GameTime += timeDelta;
                var gcBudget = GCBudget.GetValueOrDefault();
                if (timeDelta > LagDuration) {
                    Log?.Debug($"Lag: {timeDelta*1000:F2}ns");
                    LagCount++;
                    DroppedFrameCount++;
                }
                else if (timeDelta > MaxFrameDuration)
                    DroppedFrameCount++;
                else if (gcBudget > 0) {
                    var isIncomplete = false;
                    Stopwatch.Restart();
                    if (GarbageCollector.isIncremental)
                        isIncomplete = GarbageCollector.CollectIncremental((ulong) (gcBudget * 1000_000));
                    else
                        GC.Collect(0, GCCollectionMode.Optimized, true, true);
                    Stopwatch.Stop();
                    if (isIncomplete)
                        GCIncompleteCount++;
                    GCTime += Stopwatch.Elapsed.TotalSeconds;
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
            ApplyGCModeTimer = MaxFrameDuration;
            Log?.Debug($"Scene changed to: {nextScene.name}");
        }

        private void GCModeChanged(GarbageCollector.Mode mode)
        {
            ApplyGCModeTimer = MaxFrameDuration;
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
