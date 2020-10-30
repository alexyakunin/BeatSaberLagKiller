using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;

namespace LagKiller.Views
{
    [HotReload]
    public class StatisticsViewController : PersistentSingleton<StatisticsViewController>
    {
        public string ResourceName => "LagKiller.Views.StatisticsView.bsml";
        public string MenuItemTitle => "Perf. Statistics";

        private static IPA.Logging.Logger Logger => Plugin.Log;
        private GCManager GCManager => GCManager.instance;

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
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Logger?.Debug($"{GetType().Name}: Awake");
        }
    }
}
