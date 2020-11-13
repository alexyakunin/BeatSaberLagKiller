using BeatSaberMarkupLanguage.Attributes;
using BS_Utils.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using UnityEngine.SceneManagement;

namespace LagKiller.Views
{
    [HotReload]
    public class StatisticsViewController : PersistentSingleton<StatisticsViewController>, INotifyPropertyChanged
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
            => $"{GCManager.LagRatio:P} ({GCManager.LagFrequency * 60:F3} / min)";

        [UIValue("dropped-frame-info")]
        public string DroppedFrameInfo
            => $"{GCManager.DroppedFrameRatio:P} ({GCManager.DroppedFrameFrequency * 60:F3} / min)";

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
            this.UpdateUI();
        }

        /// <summary>
        /// 30秒ごとに画面更新
        /// </summary>
        private static readonly System.Timers.Timer _timer = new System.Timers.Timer(30000);

        public event PropertyChangedEventHandler PropertyChanged;

        private void Awake()
        {
            Logger?.Debug($"{GetType().Name}: Awake");
            _timer.Elapsed += this.TimerElapsed;
            _timer.Start();
        }

        protected override void OnDestroy()
        {
            _timer.Elapsed -= this.TimerElapsed;
            _timer.Dispose();
            base.OnDestroy();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            try {
                this.UpdateUI();
            }
            catch (Exception ex) {
                Logger.Error(ex);
            }
            finally {
                _timer.Start();
            }
        }

        void UpdateUI()
        {
            while (SceneManager.GetActiveScene().name == "GameCore") {
                Thread.Sleep(500);
            }
            this.NotyfyPropertyChanged(nameof(this.PlayTimeInfo));
            this.NotyfyPropertyChanged(nameof(this.LagInfo));
            this.NotyfyPropertyChanged(nameof(this.DroppedFrameInfo));
            this.NotyfyPropertyChanged(nameof(this.GCTimeInfo));
            this.NotyfyPropertyChanged(nameof(this.IncompleteGCInfo));
        }

        private void NotyfyPropertyChanged([CallerMemberName] string member = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(member));
        }
    }
}
