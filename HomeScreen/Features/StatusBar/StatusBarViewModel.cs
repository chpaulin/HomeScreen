using GalaSoft.MvvmLight;
using HomeScreen.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HomeScreen.Features.StatusBar
{
    public class StatusBarViewModel : AsyncInitViewModelBase
    {
        public override async Task Init()
        {
            //Sync with minute change
            await Task.Delay(TimeSpan.FromSeconds(60 - DateTime.Now.Second));

            RaisePropertyChanged(() => Minute);

            var minutesChanges = Observable
                .Interval(TimeSpan.FromMinutes(1))
                .Select((_) => DateTime.Now.Minute)
                .DistinctUntilChanged()
                .ObserveOnDispatcher();

            minutesChanges
                .Subscribe((_) => RaisePropertyChanged(() => Minute));

            var hourChanges = minutesChanges
                .Select((_) => DateTime.Now.Hour)
                .DistinctUntilChanged();

            hourChanges
                .Subscribe((_) => RaisePropertyChanged(() => Hour));

            var dateChanges = hourChanges
                .Select((_) => DateTime.Today)
                .DistinctUntilChanged();

            dateChanges
                .Subscribe((_) => RaisePropertyChanged(() => CurrentDate));
        }

        public int Hour
        {
            get { return DateTime.Now.Hour; }
        }

        public int Minute
        {
            get { return DateTime.Now.Minute; }
        }

        public DateTime CurrentDate
        {
            get { return DateTime.Today; }
        }
    }
}
