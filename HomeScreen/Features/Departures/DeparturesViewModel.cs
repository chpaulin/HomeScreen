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
    public class DeparturesViewModel : ViewModelBase
    {
        private const string RISSNE_STATION_NAME = "Rissne T-bana (Sundbyberg kn)";
        private const string SUNDBYBERG_STATION_NAME = "Sundbyberg station";

        private DeparturesService _departureService;

        public DeparturesViewModel(Configuration configuration)
        {
            _departureService = new DeparturesService(configuration);

            if (!configuration.Loaded)
                Messenger.Default.Register<ConfigurationLoadedEvent>(this, async (_) => await SetUpDepartureChecker());
            else
                Task.Run(SetUpDepartureChecker);
        }

        private async Task SetUpDepartureChecker()
        {
            await UpdateDepartureData();

            Observable
                .Interval(TimeSpan.FromSeconds(30))
                .ObserveOnDispatcher()
                .Subscribe(async (_) => await UpdateDepartureData());
        }

        private async Task UpdateDepartureData()
        {
            var departureData = await _departureService.RetrieveDepartureData();

            if (departureData.Departure.Count == 0)
            {
                OfflineUpdateDepatures(DeparturesSundbyberg);
                OfflineUpdateDepatures(DeparturesRissne);
                return;
            }

            DeparturesSundbyberg.Clear();
            DeparturesRissne.Clear();

            foreach (var departure in departureData.Departure.Where(d => DateTime.Parse(d.time) > DateTime.Now))
            {
                var stopTime = DateTime.Parse(departure.rtTime ?? departure.time);

                var departureVM = new DepartureViewModel
                {
                    Departs = stopTime,
                    Destination = departure.direction,
                    Number = departure.Product.num
                };

                if (departure.direction == RISSNE_STATION_NAME && DeparturesRissne.Count < 3)
                    DeparturesRissne.Add(departureVM);
                else if (DeparturesSundbyberg.Count < 3)
                    DeparturesSundbyberg.Add(departureVM);
            }
        }

        private void OfflineUpdateDepatures(IList<DepartureViewModel> departures)
        {
            var departed = departures.Where(d => d.Departs < DateTime.Now);

            foreach (var departure in departed)
            {
                departures.Remove(departure);
            }
        }

        public string Test => "Hej";

        public ObservableCollection<DepartureViewModel> DeparturesSundbyberg { get; } = new ObservableCollection<DepartureViewModel>();
        public ObservableCollection<DepartureViewModel> DeparturesRissne { get; } = new ObservableCollection<DepartureViewModel>();
    }
}
