using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Common.Configuration;
using HomeScreen.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        
        private readonly FeatureConfig _configuration;
        private readonly TimeSpan _lowFrequencyStart;
        private readonly TimeSpan _lowFrequencyEnd;
        private readonly int _lowFrequencyInterval;
        private readonly int _highFrequencyInterval;

        private DeparturesService _departureService;

        public DeparturesViewModel(FeatureConfig configuration)
        {
            _configuration = configuration;

            var lowFrequencyPeriod = _configuration.Settings["lowFrequencyUpdates"];
            _lowFrequencyInterval = int.Parse(_configuration.Settings["lowFrequencyInteval"]);
            _highFrequencyInterval = int.Parse(_configuration.Settings["highFrequencyInteval"]);

            _lowFrequencyStart = TimeSpan.Parse(lowFrequencyPeriod.Substring(0, 5));
            _lowFrequencyEnd = TimeSpan.Parse(lowFrequencyPeriod.Substring(6, 5));

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
            SetUpHighFrequencyDepartureCheck();
            SetUpLowFrequencyDepartureCheck();
        }

        private void SetUpLowFrequencyDepartureCheck()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(_lowFrequencyInterval))
                .Where((_) => IsLowFrequencyPeriod)
                .ObserveOnDispatcher()
                .Subscribe(async (_) =>
                {
                    Debug.WriteLine("Low frequency departure check");
                    var departureData = await _departureService.RetrieveDepartureData(NO_DEPARTURES_TO_SHOW);
                    await UpdateDepartureData(departureData);
                });
        }

        private void SetUpHighFrequencyDepartureCheck()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(_highFrequencyInterval))
                .Where((_) => GetTimeToFirstDeparture() < TimeSpan.FromMinutes(5) && !IsLowFrequencyPeriod)
                .ObserveOnDispatcher()
                .Subscribe(async (_) =>
                {
                    Debug.WriteLine("High frequency departure check");
                    var departureData = await _departureService.RetrieveDepartureData(NO_DEPARTURES_TO_SHOW);
                    await UpdateDepartureData(departureData);
                });
        }        

        private TimeSpan GetTimeToFirstDeparture()
        {
            var firstDeparture = Departures.FirstOrDefault();

            if (firstDeparture == null)
                return TimeSpan.MaxValue;

            Debug.WriteLine(firstDeparture.Departs - DateTime.Now);

            return firstDeparture.Departs - DateTime.Now;
        }

        private async Task UpdateDepartureData(DepartureData departureData)
        {
            var initTasks = new List<Task>();

            foreach (var departure in departureData.Departure.Where(d => d.GetDepartureTime() > DateTime.Now))
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
            await SetUpDepartureChecker();
        }

        public ObservableCollection<DepartureViewModel> Departures { get; } = new ObservableCollection<DepartureViewModel>();

        public bool IsLowFrequencyPeriod => DateTime.Now.TimeOfDay > _lowFrequencyStart && DateTime.Now.TimeOfDay < _lowFrequencyEnd;
    }
}
