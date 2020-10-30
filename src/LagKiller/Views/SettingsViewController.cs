using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using HMUI;
using LagKiller.Configuration;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LagKiller.Views
{
    [HotReload]
    public class SettingsViewController : PersistentSingleton<SettingsViewController>
    {
        public string MenuItemTitle => "Lag Killer";

        public string ResourceName => "LagKiller.Views.SettingsView.bsml";

        private static IPA.Logging.Logger Logger => Plugin.Log;

        [UIValue("is-enabled")]
        public bool IsEnabled {
            get => PluginConfig.Instance.IsEnabled;
            set => PluginConfig.Instance.IsEnabled = value;
        }

        [UIValue("gc-budget")]
        public float GCBudget {
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
                (_toggleChild as CurvedTextMeshPro).GetComponentsInParent<ToggleSetting>(true).First().toggle.isOn = true;
                _sliderSetting.slider.value = PluginConfig.DefaultGCBudget;
            }
            catch (System.Exception e) {
                Plugin.Log.Error(e);
            }
        }

        [UIAction("apply-off-settings")]
        private void ApplyTurnedOffSettings()
        {
            try {
                (_toggleChild as CurvedTextMeshPro).GetComponentsInParent<ToggleSetting>(true).First().toggle.isOn = false;
                _sliderSetting.slider.value = PluginConfig.DefaultGCBudget;
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
            Logger?.Debug($"{GetType().Name}: Awake");
        }
    }
}
