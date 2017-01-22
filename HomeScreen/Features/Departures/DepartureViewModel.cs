﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Messages;
using System;
using System.Reactive.Linq;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Departures
{
    public class DepartureViewModel : ViewModelBase
    {
        private readonly IDisposable _subscription;

        public DepartureViewModel()
        {
            _subscription = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe((_) => Update());
        }

        private void Update()
        {
            if (Departs < DateTime.Now)
            {
                _subscription.Dispose();
                Messenger.Default.Send(new DepartureDepartedEvent { Departure = this });
            }
            else
                RaisePropertyChanged(nameof(TimeToDeparture));
        }

        public DateTime Departs { get; internal set; }
        public string Destination { get; internal set; }
        public string Number { get; internal set; }
        public DateTime OriginalDeparture { get; internal set; }

        public string TimeToDeparture
        {
            get
            {
                var timeToDeparture = Departs.Subtract(DateTime.Now);

                if (timeToDeparture.TotalMinutes < 1) //Time passed
                    return "Nu";

                if (timeToDeparture.TotalMinutes >= 60)
                    return Departs.ToString("HH:mm");

                return $"{timeToDeparture.TotalMinutes:0} min";
            }
        }
    }
}