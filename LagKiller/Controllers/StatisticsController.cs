using System.Linq;
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
        public GCManager GCManager => GCManager.instance;

        [UIParams]
#pragma warning disable 414
        private BSMLParserParams parserParams = null;
#pragma warning restore 414

        [UIValue("play-time-info")]
        public string PlayTimeInfo 
            => $"{GCManager.PlayTime / 60:F3} min";

        [UIValue("lag-info")]
        public string LagInfo 
            => $"{GCManager.LagRatio:P} ({GCManager.LagFrequency * 60:F3} / min)";

        [UIValue("dropped-frame-info")]
        public string DroppedFrameInfo 
            => $"{GCManager.DroppedFrameRatio:P} ({GCManager.DroppedFrameFrequency:F3} / sec)";

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
            parserParams.EmitEvent("cancel");
        }

        [UIAction("#cancel")]
        private void Cancel()
        {
            this.NotifyChanged();
        }

        private void Awake()
        {
            Log?.Debug($"{GetType().Name}: Awake");
            Cancel();
            BSEvents.menuSceneActive += Cancel;
        }
    }
}
