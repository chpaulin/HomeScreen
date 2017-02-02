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

            Messenger.Default.Register<DepartureDepartedEvent>(this, async (e) => await OnDepartureDeparted(e));
        }

        private async Task OnDepartureDeparted(DepartureDepartedEvent eventArgs)
        {
            Departures.Remove(eventArgs.Departure);

            await _departureService.RetrieveDepartureData(NO_DEPARTURES_TO_SHOW)
                .ContinueWith(async departures => UpdateDepartureData(await departures), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task SetUpDepartureChecker()
        {
            var initialDepartureData = await _departureService.RetrieveDepartureData(NO_DEPARTURES_TO_SHOW);

            await UpdateDepartureData(initialDepartureData);

            Observable
                .Interval(TimeSpan.FromSeconds(30))
                .ObserveOnDispatcher()
                .Subscribe(async (_) =>
                {
                    var departureData = await _departureService.RetrieveDepartureData(NO_DEPARTURES_TO_SHOW);
                    await UpdateDepartureData(departureData);
                });
        }

        private async Task UpdateDepartureData(DepartureData departureData)
        {
            var initTasks = new List<Task>();

            foreach (var departure in departureData.Departure)
            {
                var existingDeparture = Departures.FirstOrDefault(d => d.OriginalDeparture == departure.GetOriginalDepartureTime());

                if (existingDeparture != null)
                {
                    //Update
                    existingDeparture.Update(departure);
                }
                else
                {
                    //New
                    var departureVM = new DepartureViewModel(departure);
                    Departures.Add(departureVM);
                    initTasks.Add(departureVM.Init());
                }
            }

            await Task.WhenAll(initTasks);
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
