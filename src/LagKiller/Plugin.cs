using BeatSaberMarkupLanguage.Settings;
using BS_Utils.Utilities;
using IPA;
using IPA.Config.Stores;
using LagKiller.Views;
using System;
using System.IO;
using System.Linq;
using IPALogger = IPA.Logging.Logger;

namespace LagKiller
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "LagKiller";
        public static IPALogger Log { get; set; }
        public static string BOOTCONFIG_PATH = Path.Combine(Environment.CurrentDirectory, "Beat Saber_Data", "boot.config");
        public const string GC_OPTION = "gc-max-time-slice=3";

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        [Init]
        public void InitWithConfig(IPALogger logger, IPA.Config.Config conf)
        {
            instance = this;
            Log = logger;
            this.AddIncriemtGCModeText();
            Log.Debug("Logger initialized.");
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        #endregion

        [OnStart]
        public void OnStart() => BSEvents.earlyMenuSceneLoadedFresh += this.BSEvents_earlyMenuSceneLoadedFresh;
        [OnExit]
        public void OnExit() => BSEvents.earlyMenuSceneLoadedFresh -= this.BSEvents_earlyMenuSceneLoadedFresh;

        private void BSEvents_earlyMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            BSMLSettings.instance.AddSettingsMenu(SettingsViewController.instance.MenuItemTitle, SettingsViewController.instance.ResourceName, SettingsViewController.instance);
            BSMLSettings.instance.AddSettingsMenu(StatisticsViewController.instance.MenuItemTitle, StatisticsViewController.instance.ResourceName, StatisticsViewController.instance);
            GCManager.TouchInstance();
        }

        private void AddIncriemtGCModeText()
        {
            try {
                var fs = File.ReadAllLines(BOOTCONFIG_PATH);
                if (!fs.Contains(GC_OPTION)) {
                    fs = fs.Union(new string[] { GC_OPTION }).ToArray();
                    File.WriteAllLines(BOOTCONFIG_PATH, fs);
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
