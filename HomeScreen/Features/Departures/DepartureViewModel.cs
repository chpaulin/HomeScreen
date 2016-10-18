using GalaSoft.MvvmLight;
using System;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Departures
{
    public class DepartureViewModel : ViewModelBase
    {
        public DepartureViewModel()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

            timer.Tick += (_, __) => RaisePropertyChanged(nameof(Departs));

            timer.Start();
        }

        public DateTime Departs { get; internal set; }
        public string Destination { get; internal set; }
        public string Number { get; internal set; }
    }
}