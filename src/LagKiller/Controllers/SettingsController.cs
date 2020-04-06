using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;

namespace LagKiller.Controllers
{
    public class SettingsController : NotifiableSingleton<SettingsController>
    {
        public string ResourceName => "LagKiller.Views.Settings.bsml";
        public string MenuItemTitle => "Lag Killer";

        private static IPA.Logging.Logger Log => Plugin.Log;
        private Settings Settings => Settings.Instance; 

        [UIParams]
        private BSMLParserParams parserParams = null;
        private bool _isEnabled;
        private float _gcBudget;
        private bool _ignoreCancel = false;

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
                _gcBudget = value;
                NotifyPropertyChanged();
            }
        }
        [UIValue("min-gc-budget")]
        public float MinGCBudget => Settings.MinGCBudget; 
        [UIValue("max-gc-budget")]
        public float MaxGCBudget => Settings.MaxGCBudget; 

        [UIValue("gc-mode-info")]
        public string GCModeInfo => GCInfo.GetSummary();

        [UIAction("apply-recommended-settings")]
        private void ApplyRecommendedSettings()
        {
            IsEnabled = true;
            GCBudget = Settings.DefaultGCBudget;
            Refresh();
        }

        [UIAction("apply-off-settings")]
        private void ApplyTurnedOffSettings()
        {
            IsEnabled = false;
            GCBudget = Settings.DefaultGCBudget;
            Refresh();
        }

        [UIAction("#apply")]
        private void Apply()
        {
            Settings.IsEnabled = IsEnabled;
            Settings.GCBudget = GCBudget;
            Cancel(); // To copy possibly clamped values back
        }

        [UIAction("#cancel")]
        private void Cancel()
        {
            if (!_ignoreCancel) {
                IsEnabled = Settings.IsEnabled;
                GCBudget = Settings.GCBudget;
            }
            this.NotifyChanged((me, name) => me.NotifyPropertyChanged(name));
        }

        private void Refresh(bool reset = false)
        {
            _ignoreCancel = !reset;
            parserParams.EmitEvent("cancel");
        }

        private void Awake()
        {
            Log?.Debug($"{GetType().Name}: Awake");
            Cancel();
            BSEvents.menuSceneActive += Cancel;
        }
    }
}
