using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HomeScreen.Features.StatusBar
{
    public class StatusBarViewModel : ViewModelBase
    {
        private DateTime _currentDate;

        public StatusBarViewModel()
        {
            SetCurrentTime();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            //timer.Tick += (_, __) =>
            //{
            //    SetCurrentTime();
            //};

            timer.Start();
        }

        private void SetCurrentTime()
        {
            CurrentDate = DateTime.Now;

            if (CurrentDate.Hour != Hour)
            {
                Hour = CurrentDate.Hour;
                RaisePropertyChanged(nameof(Hour));
            }

            Minute = CurrentDate.Minute;
            RaisePropertyChanged(nameof(Minute));
        }

        public int Hour { get; set; }

        public int Minute { get; set; }

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
