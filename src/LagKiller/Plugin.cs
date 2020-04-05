using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Logging;
using LagKiller.Controllers;

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
            var settings = SettingsController.instance;
            BSMLSettings.instance.AddSettingsMenu(settings.MenuItemTitle, settings.ResourceName, settings);
            var statistics = StatisticsController.instance;
            BSMLSettings.instance.AddSettingsMenu(statistics.MenuItemTitle, statistics.ResourceName, statistics);
            GCManager.TouchInstance();
        }
    }
}
