using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Messages;
using System;
using System.Reactive.Linq;
using Windows.UI.Xaml;
using System.Threading.Tasks;

namespace HomeScreen.Features.Departures
{
    public class DepartureViewModel : AsyncInitViewModelBase
    {
        private IDisposable _subscription;
        private Departure _departure;

        public DepartureViewModel(Departure departure)
        {
            _departure = departure;
        }

        private void Refresh()
        {
            if (Departs < DateTime.Now)
            {
                _subscription.Dispose();
                Messenger.Default.Send(new DepartureDepartedEvent { Departure = this });
            }
            else
                RaisePropertyChanged(nameof(TimeToDeparture));
        }

        public DateTime Departs
        {
            get
            {
                return _departure.GetDepartureTime();
            }
        }

        public string Destination
        {
            get
            {
                return _departure.direction;
            }
        }

        public string Number
        {
            get
            {
                return _departure.Product.num;
            }
        }

        public DateTime OriginalDeparture
        {
            get
            {
                return _departure.GetOriginalDepartureTime();
            }
        }

        public string TimeToDeparture
        {
            get
            {
                var timeToDeparture = Departs.Subtract(DateTime.Now);

                if (timeToDeparture.TotalMinutes <= 1) //Time passed
                    return "Nu";

                if (timeToDeparture.TotalMinutes >= 60)
                    return Departs.ToString("HH:mm");

                return $"{timeToDeparture.TotalMinutes:0} min";
            }
        }

        public void Update(Departure departure)
        {
            _departure = departure;
        }

        public override async Task Init()
        {
            //Sync with minute change
            await Task.Delay(TimeSpan.FromSeconds(60 - DateTime.Now.Second));

            RaisePropertyChanged(() => Departs);

            _subscription = Observable
                .Interval(TimeSpan.FromMinutes(1))
                .ObserveOnDispatcher()
                .Subscribe((_) => Refresh());
        }
    }
}