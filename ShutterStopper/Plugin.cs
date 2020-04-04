﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Logging;
using ShutterStopper.Controllers;

namespace ShutterStopper
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Logger Log { get; set; }

        [Init]
        public void Init(Logger log)
        {
            Log = log;
            Log?.Debug("Initialized.");
        }

        [OnStart]
        public void OnStart()
        {
            var sc = SettingsController.instance;
            BSMLSettings.instance.AddSettingsMenu(sc.MenuItemTitle, sc.ResourceName, sc);
            GCManager.TouchInstance();
        }

        [OnExit]
        public void OnExit()
        {
        }
    }
}
