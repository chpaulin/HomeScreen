using GalaSoft.MvvmLight;
using System;
using System.Reactive.Linq;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Departures
{
    public class DepartureViewModel : ViewModelBase
    {
        public DepartureViewModel()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe((_) => RaisePropertyChanged(nameof(Departs)));            
        }

        public DateTime Departs { get; internal set; }
        public string Destination { get; internal set; }
        public string Number { get; internal set; }
    }
}