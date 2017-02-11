using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Common.Configuration;
using HomeScreen.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Weather
{
    public class WeatherViewModel : AsyncInitViewModelBase
    {
        private const int FORCAST_LENGHT = 7;

        private WeatherService _weatherService;
        private FeatureConfig _configuration;
        private TimeSpan _dawn;
        private TimeSpan _dusk;
        private ForecastViewModel _currentWeather;

        public WeatherViewModel(FeatureConfig configuration)
        {
            _configuration = configuration;
        }

        private async Task SetUp()
        {
            Observable
                .Interval(TimeSpan.FromMinutes(30))
                .ObserveOnDispatcher()
                .Subscribe(async (_) =>
                {
                    var currentWeatherData = await _weatherService.RetrieveCurrentWeatherData();
                    UpdateCurrentWeatherData(currentWeatherData);

                    var forcasts = await _weatherService.RetrieveForcastWeatherData();
                    await UpdateForecastWeatherData(forcasts);
                });

            await _weatherService.RetrieveCurrentWeatherData()
                    .ContinueWith(async currentWeatherData => UpdateCurrentWeatherData(await currentWeatherData), TaskScheduler.FromCurrentSynchronizationContext());

            await _weatherService.RetrieveForcastWeatherData()
                .ContinueWith(async forcasts => UpdateForecastWeatherData(await forcasts), TaskScheduler.FromCurrentSynchronizationContext());

            Messenger.Default.Register<ForecastDueEvent>(this, async (e) => await OnForcastDue(e));
        }

        private async Task OnForcastDue(ForecastDueEvent eventArgs)
        {
            Forecasts.Remove(eventArgs.Forecast);

            await _weatherService.RetrieveForcastWeatherData()
                .ContinueWith(async forcasts => UpdateForecastWeatherData(await forcasts), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task UpdateForecastWeatherData(IEnumerable<Forecast> forecasts)
        {
            forecasts = forecasts.OrderBy(f => f.Date).Where(f => f.Date > DateTime.Now && f.Date.Date >= DateTime.Today && f.Date.Date <= DateTime.Today.AddDays(1));

            foreach (var forecast in forecasts.Take(FORCAST_LENGHT))
            {
                var existingForcastVM = Forecasts.FirstOrDefault(f => f.Date == forecast.Date);

                if (existingForcastVM != null)
                {
                    existingForcastVM.Update(forecast);
                }
                else
                {
                    var icons = GetIcons(forecast.Date.TimeOfDay, _dawn, _dusk);

                    var forecastVM = new ForecastViewModel(forecast, icons);

                    Forecasts.Add(forecastVM);

                    await forecastVM.Init();
                }
            }
        }

        private IDictionary<int, string> GetIcons(TimeSpan timeOfDay, TimeSpan dawn, TimeSpan dusk)
        {
            if (dawn <= timeOfDay && timeOfDay <= dusk) //Day
                return Constants.WeatherIconsDay;

            return Constants.WeatherIconsNight;
        }

        private bool CheckIfDateIsAroundNoon(int dt)
        {
            var hour = UnixTimeStampUtility.UnixTimeStampToDateTime(dt).Hour;

            return hour >= 12 && hour <= 15;
        }

        public ObservableCollection<ForecastViewModel> Forecasts { get; } = new ObservableCollection<ForecastViewModel>();


        private void UpdateCurrentWeatherData(CurrentWeatherConditions currentWeatherData)
        {
            _dawn = UnixTimeStampUtility.UnixTimeStampToDateTime(currentWeatherData?.sys?.sunrise ?? 0).TimeOfDay;
            _dusk = UnixTimeStampUtility.UnixTimeStampToDateTime(currentWeatherData?.sys?.sunset ?? 0).TimeOfDay;

            var icons = GetIcons(DateTime.Now.TimeOfDay, _dawn, _dusk);
            CurrentWeather = new ForecastViewModel(new Forecast
            {
                Temperature = currentWeatherData?.main?.temp ?? 0,
                WeatherTypeId = currentWeatherData.weather.FirstOrDefault()?.id
            }, icons);
        }



        public override async Task Init()
        {
            _weatherService = new WeatherService(_configuration);

            await SetUp();
        }

        public ForecastViewModel CurrentWeather
        {
            get { return _currentWeather; }
            set
            {
                _currentWeather = value;
                RaisePropertyChanged();
            }
        }
    }
}
