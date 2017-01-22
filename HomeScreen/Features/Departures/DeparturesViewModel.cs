using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Departures
{
    public class DeparturesViewModel : AsyncInitViewModelBase
    {
        private const int NO_DEPARTURES_TO_SHOW = 4;

        private DeparturesService _departureService;
        private Configuration _configuration;

        public DeparturesViewModel(Configuration configuration)
        {
            _configuration = configuration;

            Messenger.Default.Register<DepartureDepartedEvent>(this, OnDepartureDeparted);
        }

        private void OnDepartureDeparted(DepartureDepartedEvent eventArgs)
        {
            Departures.Remove(eventArgs.Departure);
        }

        private async Task SetUpDepartureChecker()
        {
            var initialDepartureData = await _departureService.RetrieveDepartureData(6);

            UpdateDepartureData(initialDepartureData);

            Observable
                .Interval(TimeSpan.FromSeconds(30))
                .ObserveOnDispatcher()
                .Subscribe(async (_) =>
                {
                    var departureData = await _departureService.RetrieveDepartureData(6);
                    UpdateDepartureData(departureData);
                });
        }

        private void UpdateDepartureData(DepartureData departureData)
        {
            foreach (var departure in departureData.Departure.Where(d => GetDepartureTime(d) > DateTime.Now).Take(NO_DEPARTURES_TO_SHOW))
            {
                var stopTime = GetDepartureTime(departure);
                var originalStopTime = GetOriginalDepartureTime(departure);

                if (Departures.Any(d => d.OriginalDeparture == originalStopTime))
                {
                    //Update
                }
                else
                {
                    //New
                    var departureVM = new DepartureViewModel
                    {
                        Departs = stopTime,
                        OriginalDeparture = originalStopTime,
                        Destination = departure.direction,
                        Number = departure.Product.num
                    };

                    Departures.Add(departureVM);
                }
            }
        }

        private DateTime GetDepartureTime(Departure departureData)
        {
            return DateTime.Parse(departureData.date).Add(DateTime.Parse(departureData.rtTime ?? departureData.time).TimeOfDay);
        }

        private DateTime GetOriginalDepartureTime(Departure departureData)
        {
            return DateTime.Parse(departureData.date).Add(DateTime.Parse(departureData.time).TimeOfDay);
        }

        public override async Task Init()
        {
            _departureService = new DeparturesService(_configuration);

            if (!_configuration.Loaded)
                Messenger.Default.Register<ConfigurationLoadedEvent>(this, async (_) => await SetUpDepartureChecker());
            else
                await SetUpDepartureChecker();
        }

        public ObservableCollection<DepartureViewModel> Departures { get; } = new ObservableCollection<DepartureViewModel>();
    }
}
