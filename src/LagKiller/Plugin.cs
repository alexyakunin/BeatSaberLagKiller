using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using IPA;
using IPA.Logging;
using LagKiller.Controllers;
using Logger = IPA.Logging.Logger;

namespace LagKiller
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Logger Log { get; set; }

        [Init]
        public void Init(Logger log) => Log = log;

        [OnStart]
        public void OnStart()
        {
            BSEvents.earlyMenuSceneLoadedFresh += this.BSEvents_earlyMenuSceneLoadedFresh;
        }

        private void BSEvents_earlyMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            var settings = BeatSaberUI.CreateViewController<SettingsController>();
            BSMLSettings.instance.AddSettingsMenu(settings.MenuItemTitle, settings.ResourceName, settings);

            var statistics = BeatSaberUI.CreateViewController<StatisticsController>();
            BSMLSettings.instance.AddSettingsMenu(statistics.MenuItemTitle, statistics.ResourceName, statistics);
            GCManager.TouchInstance();
        }
    }
}
