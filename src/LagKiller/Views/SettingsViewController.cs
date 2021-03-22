using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using LagKiller.Configuration;
using System.Linq;

namespace LagKiller.Views
{
    [HotReload]
    public class SettingsViewController : PersistentSingleton<SettingsViewController>
    {
        public string MenuItemTitle => "Lag Killer";

        public string ResourceName => "LagKiller.Views.SettingsView.bsml";

        private static IPA.Logging.Logger Logger => Plugin.Log;

        [UIValue("is-enabled")]
        public bool IsEnabled
        {
            get => PluginConfig.Instance.IsEnabled;
            set => PluginConfig.Instance.IsEnabled = value;
        }

        [UIValue("gc-budget")]
        public float GCBudget
        {
            get => PluginConfig.Instance.GCBudget;
            set => PluginConfig.Instance.GCBudget = value;
        }
        [UIValue("min-gc-budget")]
        public float MinGCBudget => PluginConfig.MinGCBudget;
        [UIValue("max-gc-budget")]
        public float MaxGCBudget => PluginConfig.MaxGCBudget;

        [UIValue("gc-mode-info")]
        public string GCModeInfo => GCInfo.GetSummary();

        [UIAction("apply-recommended-settings")]
        private void ApplyRecommendedSettings()
        {
            try {
                (this._toggleChild as CurvedTextMeshPro).GetComponentsInParent<ToggleSetting>(true).First().toggle.isOn = true;
                this._sliderSetting.slider.value = PluginConfig.DefaultGCBudget;
            }
            catch (System.Exception e) {
                Plugin.Log.Error(e);
            }
        }

        [UIAction("apply-off-settings")]
        private void ApplyTurnedOffSettings()
        {
            try {
                (this._toggleChild as CurvedTextMeshPro).GetComponentsInParent<ToggleSetting>(true).First().toggle.isOn = false;
                this._sliderSetting.slider.value = PluginConfig.DefaultGCBudget;
            }
            catch (System.Exception e) {
                Plugin.Log.Error(e);
            }
        }

        [UIComponent("enable-gc")]
        public object _toggleChild;


        [UIComponent("budget")]
        public GenericSliderSetting _sliderSetting;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Logger?.Debug($"{this.GetType().Name}: Awake");
        }
    }
}
