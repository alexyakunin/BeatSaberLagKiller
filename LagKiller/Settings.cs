using System;
using BS_Utils.Utilities;
using UnityEngine;

namespace LagKiller
{
    public class Settings
    {
        public static readonly Settings Instance = new Settings();
        public static readonly float MinGCBudget = 0.25f; 
        public static readonly float MaxGCBudget = 10; 
        public static readonly float MinFps = 1; 
        public static readonly float MaxFps = 1000; 
        public static readonly float MinDuration = 0.01f; 
        public static readonly float MaxDuration = 3600 * 24 * 366; 
        public static readonly string MainSection = "Main";
        
        private readonly Lazy<Config> _configLazy = 
            new Lazy<Config>(() => new Config("LagKiller"));
        
        private static IPA.Logging.Logger Log => Plugin.Log;
        private Config Config => _configLazy.Value;
        
        public event Action<Settings> Changed;

        public bool IsEnabled {
            get => Config.GetBool(MainSection, nameof(IsEnabled), true);
            set {
                Config.SetBool(MainSection, nameof(IsEnabled), value);
                OnChanged();
            }
        }

        public float GCBudget {
            get => Mathf.Clamp(Config.GetFloat(MainSection, nameof(GCBudget), 2f), MinGCBudget, MaxGCBudget);
            set {
                value = Mathf.Clamp(value, MinGCBudget, MaxGCBudget);
                Config.SetFloat(MainSection, nameof(GCBudget), value);
                OnChanged();
            }
        }

        public float FrameDropFpsBoundary {
            get => Mathf.Clamp(
                Config.GetFloat(MainSection, nameof(FrameDropFpsBoundary), 70f), 
                MinFps, MaxFps);
            set {
                value = Mathf.Clamp(value, MinFps, MaxFps);
                Config.SetFloat(MainSection, nameof(FrameDropFpsBoundary), value);
                OnChanged();
            }
        }

        public float LagFpsBoundary {
            get => Mathf.Clamp(
                Config.GetFloat(MainSection, nameof(LagFpsBoundary), 10f), 
                MinFps, MaxFps);
            set {
                value = Mathf.Clamp(value, MinFps, MaxFps);
                Config.SetFloat(MainSection, nameof(LagFpsBoundary), value);
                OnChanged();
            }
        }

        public float ApplyGCModePeriod {
            get => Mathf.Clamp(
                Config.GetFloat(MainSection, nameof(ApplyGCModePeriod), 30), 
                MinDuration, MaxDuration);
            set {
                value = Mathf.Clamp(value, MinDuration, MinDuration);
                Config.SetFloat(MainSection, nameof(ApplyGCModePeriod), value);
                OnChanged();
            }
        }

        public float GameStartupDuration {
            get => Mathf.Clamp(
                Config.GetFloat(MainSection, nameof(GameStartupDuration), 1), 
                MinDuration, MaxDuration);
            set {
                value = Mathf.Clamp(value, MinDuration, MinDuration);
                Config.SetFloat(MainSection, nameof(GameStartupDuration), value);
                OnChanged();
            }
        }

        private void OnChanged() => Changed?.Invoke(this);
    }
}
