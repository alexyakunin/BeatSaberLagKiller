using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using UnityEngine;

namespace ShutterStopper.Controllers
{
    public class SettingsController : NotifiableSingleton<SettingsController>
    {
        public string ResourceName => "ShutterStopper.Views.Settings.bsml";
        public string MenuItemTitle => "Shutter Stopper";

        private static IPA.Logging.Logger Log => Plugin.Log;
        public Settings Settings => Settings.Instance; 
        public GCManager GCManager => GCManager.instance;

        [UIParams]
#pragma warning disable 414
        private BSMLParserParams parserParams = null;
#pragma warning restore 414

        private bool _isEnabled;
        private float _gcBudget;

        [UIValue("is-enabled")]
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("gc-budget")]
        public float GCBudget {
            get => _gcBudget;
            set {
                value = Mathf.Round(value * 10) / 10f;
                value = Mathf.Clamp(value, MinGCBudget, MaxGCBudget);
                _gcBudget = value;
                NotifyPropertyChanged();
            }
        }
        [UIValue("min-gc-budget")]
        public float MinGCBudget => Settings.MinGCBudget; 
        [UIValue("max-gc-budget")]
        public float MaxGCBudget => Settings.MaxGCBudget; 

        [UIValue("lag-frequency")]
        public string LagFrequency => (GCManager.LagFrequency * 60).ToString("F3") + " / min";

        [UIValue("shutter-frequency")]
        public string ShutterFrequency => GCManager.ShutterFrequency.ToString("F3") + " / sec";

        [UIValue("gc-over-budget-frequency")]
        public string GCOverBudgetFrequency => (GCManager.GCOverBudgetFrequency * 60).ToString("F3") + " / min";

        [UIValue("gc-percent-of-budget")]
        public string GCTimeToBudgetPercent => GCManager.GCTimeToBudgetRatio.ToString("P");

        [UIAction("recommended")]
        private void ApplyRecommendedSettings()
        {
            IsEnabled = true;
            GCBudget = 2f;
        }

        [UIAction("off")]
        private void ApplyTurnedOffSettings()
        {
            IsEnabled = false;
            GCBudget = 2f;
        }

        [UIAction("reset-statistics")]
        private void ResetStatistics()
        {
            GCManager.ResetStatistics();
            NotifyPropertyChanged();
        }

        [UIAction("#apply")]
        private void Apply()
        {
            Log?.Debug("Settings: Apply");
            Settings.IsEnabled = IsEnabled;
            Settings.GCBudget = GCBudget;
        }

        private void Refresh()
        {
            Log?.Debug("Settings: Refresh");
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            foreach (var p in GetType().GetProperties(bindingFlags))
                NotifyPropertyChanged(p.Name);
        }

        private void Awake()
        {
            Log?.Debug($"Settings: Awake");
            IsEnabled = Settings.IsEnabled;
            GCBudget = Settings.GCBudget;
            BSEvents.menuSceneActive += Refresh;
        }
    }
}
