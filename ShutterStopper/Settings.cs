using System;
using BS_Utils.Utilities;
using UnityEngine;

namespace ShutterStopper
{
    public class Settings
    {
        public static readonly Settings Instance = new Settings();
        public static readonly float MinGCBudget = 0.25f; 
        public static readonly float MaxGCBudget = 10f; 
        public static readonly string MainSection = "Main";
        
        private readonly Lazy<Config> _configLazy = 
            new Lazy<Config>(() => new Config("ShutterStopper"));
        
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

        private void OnChanged() => Changed?.Invoke(this);
    }
}
