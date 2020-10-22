using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using IPA;
using IPA.Config.Stores;
using IPA.Logging;
using LagKiller.Views;
using IPALogger = IPA.Logging.Logger;

namespace LagKiller
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "LagKiller";
        public static IPALogger Log { get; set; }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        [Init]
        public void InitWithConfig(IPALogger logger, IPA.Config.Config conf)
        {
            instance = this;
            Log = logger;
            Log.Debug("Logger initialized.");
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        #endregion

        [OnStart]
        public void OnStart()
        {
            BSEvents.earlyMenuSceneLoadedFresh += this.BSEvents_earlyMenuSceneLoadedFresh;
        }

        private void BSEvents_earlyMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            var settings = BeatSaberUI.CreateViewController<SettingsViewController>();
            BSMLSettings.instance.AddSettingsMenu(settings.MenuItemTitle, settings.ResourceName, settings);

            var statistics = BeatSaberUI.CreateViewController<StatisticsViewController>();
            BSMLSettings.instance.AddSettingsMenu(statistics.MenuItemTitle, statistics.ResourceName, statistics);
            GCManager.TouchInstance();
        }
    }
}
