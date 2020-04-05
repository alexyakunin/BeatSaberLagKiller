using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;

namespace LagKiller.Controllers
{
    public class StatisticsController : NotifiableSingleton<StatisticsController>
    {
        public string ResourceName => "LagKiller.Views.Statistics.bsml";
        public string MenuItemTitle => "Perf. Statistics";

        private static IPA.Logging.Logger Log => Plugin.Log;
        private GCManager GCManager => GCManager.instance;

        [UIParams]
        private BSMLParserParams parserParams = null;

        [UIValue("play-time-info")]
        public string PlayTimeInfo 
            => $"{GCManager.PlayTime / 60:F3} min";

        [UIValue("lag-info")]
        public string LagInfo 
            => $"{GCManager.LagRatio:P} ({GCManager.LagFrequency*60:F3} / min)";

        [UIValue("dropped-frame-info")]
        public string DroppedFrameInfo 
            => $"{GCManager.DroppedFrameRatio:P} ({GCManager.DroppedFrameFrequency*60:F3} / min)";

        [UIValue("gc-time-info")]
        public string GCTimeInfo 
            => GCManager.GCTimeRatio.ToString("P");

        [UIValue("incomplete-gc-info")]
        public string IncompleteGCInfo 
            => GCManager.GCIncompleteRatio.ToString("P");

        [UIAction("reset-statistics")]
        private void ResetStatistics()
        {
            GCManager.ResetStatistics();
            Refresh();
        }

        [UIAction("#cancel")]
        private void Cancel()
        {
            this.NotifyChanged((me, name) => me.NotifyPropertyChanged(name));
        }

        private void Refresh(bool reset = false)
        {
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
