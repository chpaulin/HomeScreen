using GalaSoft.MvvmLight;
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
    public class StatusBarViewModel : ViewModelBase
    {
        private DateTime _currentDate;
        private int _hour;
        private int _minute;

        public StatusBarViewModel()
        {
            SetCurrentTime();

            var minutesChanges = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Select((_) => DateTime.Now.Minute)
                .DistinctUntilChanged()
                .ObserveOnDispatcher();

            minutesChanges
                .Subscribe((minute) => Minute = minute);

            var hourChanges = minutesChanges
                .Select((_) => DateTime.Now.Hour)
                .DistinctUntilChanged();

            hourChanges
                .Subscribe((hour) => Hour = hour);

            var dateChanges = hourChanges
                .Select((_) => DateTime.Today)
                .DistinctUntilChanged();

            dateChanges
                .Subscribe((date) => CurrentDate = date);
        }

        private void SetCurrentTime()
        {
            CurrentDate = DateTime.Now;

            Hour = CurrentDate.Hour;

            Minute = CurrentDate.Minute;
        }

        public int Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                RaisePropertyChanged();
            }
        }

        public int Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                RaisePropertyChanged();
            }
        }

        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;
                RaisePropertyChanged();
            }
        }
    }
}
