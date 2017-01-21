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

            Departures.Clear();

            foreach (var departure in departureData.Departure.Where(d => GetDepartureTime(d) > DateTime.Now).Take(NO_DEPARTURES_TO_SHOW))
            {
                var stopTime = GetDepartureTime(departure);

                var departureVM = new DepartureViewModel
                {
                    Departs = stopTime,
                    Destination = departure.direction,
                    Number = departure.Product.num
                };

                Departures.Add(departureVM);
            }
        }

        private DateTime GetDepartureTime(Departure departureData)
        {
            return DateTime.Parse(departureData.date).Add(DateTime.Parse(departureData.rtTime ?? departureData.time).TimeOfDay);
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
