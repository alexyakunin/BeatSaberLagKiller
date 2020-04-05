using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using UnityEngine;

namespace LagKiller.Controllers
{
    public class SettingsController : NotifiableSingleton<SettingsController>
    {
        public string ResourceName => "LagKiller.Views.Settings.bsml";
        public string MenuItemTitle => "Lag Killer";

        private static IPA.Logging.Logger Log => Plugin.Log;
        public Settings Settings => Settings.Instance; 

        [UIParams]
#pragma warning disable 414
        private BSMLParserParams parserParams = null;
#pragma warning restore 414

        private bool _isEnabled;
        private float _gcBudget;
        private bool _ignoreCancel = false;

        [UIValue("is-enabled")]
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                if (_isEnabled != Settings.IsEnabled)
                    _isEnabled = Settings.IsEnabled = _isEnabled;
                NotifyPropertyChanged();
            }
        }

        [UIValue("gc-budget")]
        public float GCBudget {
            get => _gcBudget;
            set {
                _gcBudget = value;
                if (_gcBudget != Settings.GCBudget)
                    _gcBudget = Settings.GCBudget = _gcBudget;
                NotifyPropertyChanged();
            }
        }
        [UIValue("min-gc-budget")]
        public float MinGCBudget => Settings.MinGCBudget; 
        [UIValue("max-gc-budget")]
        public float MaxGCBudget => Settings.MaxGCBudget; 

        [UIValue("gc-mode-info")]
        public string GCModeInfo => GCInfo.GetSummary();

        [UIAction("recommended")]
        private void ApplyRecommendedSettings()
        {
            IsEnabled = true;
            GCBudget = 2;
            Refresh();
        }

        [UIAction("off")]
        private void ApplyTurnedOffSettings()
        {
            IsEnabled = false;
            GCBudget = 2;
            Refresh();
        }

        [UIAction("#cancel")]
        private void Cancel()
        {
            if (_ignoreCancel) {
                _ignoreCancel = false;
                return;
            }
            IsEnabled = Settings.IsEnabled;
            GCBudget = Settings.GCBudget;
            this.NotifyChanged();
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
