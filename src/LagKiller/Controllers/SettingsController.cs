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
                Settings.IsEnabled = value;
                _isEnabled = Settings.IsEnabled;
                NotifyPropertyChanged();
            }
        }

        [UIValue("gc-budget")]
        public float GCBudget {
            get => _gcBudget;
            set {
                Settings.GCBudget = value;
                _gcBudget = Settings.GCBudget;
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
            _isEnabled = true;
            _gcBudget = Settings.DefaultGCBudget;
            Refresh();
        }

        [UIAction("apply-off-settings")]
        private void ApplyTurnedOffSettings()
        {
            _isEnabled = false;
            _gcBudget = Settings.DefaultGCBudget;
            Refresh();
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
