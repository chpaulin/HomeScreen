﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Weather
{
    public class WeatherViewModel : ViewModelBase
    {
        private const int FORCAST_LENGHT = 5;

        private double _temperature;
        private string _icon;
        private readonly WeatherService _weatherService;

        public WeatherViewModel(Configuration configuration)
        {
            _weatherService = new WeatherService(configuration);

            if (!configuration.Loaded)
                Messenger.Default.Register<ConfigurationLoadedEvent>(this, async (_) => await SetUpWeatherChecker());
            else
                Task.Run(SetUpWeatherChecker);
        }

        private async Task SetUpWeatherChecker()
        {
            await UpdateWeatherData();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(10)
            };

            timer.Tick += async (_, __) => await UpdateWeatherData();

            timer.Start();
        }

        private async Task UpdateWeatherData()
        {
            await UpdateCurrentWeatherData();

            await UpdateForecastWeatherData();
        }

        private async Task UpdateForecastWeatherData()
        {
            var forecastData = await _weatherService.RetrieveForcastWeatherData();

            var forecasts = forecastData.list.Select(f => new
            {
                Temperature = f.main.temp,
                MinTemperature = f.main.temp_min,
                MaxTemperature = f.main.temp_max,
                WeatherTypeId = f.weather.FirstOrDefault()?.id,
                Date = UnixTimeStampUtility.UnixTimeStampToDateTime(f.dt)
            });

            forecasts = forecasts.OrderBy(f => f.Date).Where(f => f.Date > DateTime.Now && f.Date.Date >= DateTime.Today && f.Date.Date <= DateTime.Today.AddDays(1));        

            Forecasts.Clear();

            foreach (var forecast in forecasts.Take(FORCAST_LENGHT))
            {
                var forecastVM = new ForecastViewModel
                {
                    Temperature = forecast.Temperature,
                    MinTemperature = forecast.MinTemperature,
                    MaxTemperature = forecast.MaxTemperature,
                    Icon = GetWeatherIcon(forecast.WeatherTypeId, Constants.WeatherIconsDay),
                    Date = forecast.Date
                };

                Forecasts.Add(forecastVM);
            }
        }       

        private bool CheckIfDateIsAroundNoon(int dt)
        {
            var hour = UnixTimeStampUtility.UnixTimeStampToDateTime(dt).Hour;

            return hour >= 12 && hour <= 15;
        }

        public ObservableCollection<ForecastViewModel> Forecasts { get; } = new ObservableCollection<ForecastViewModel>();


        private async Task UpdateCurrentWeatherData()
        {
            var currentWeatherData = await _weatherService.RetrieveCurrentWeatherData();

            if (currentWeatherData.main.temp == 0d)
                return;

            Temperature = currentWeatherData.main.temp;

            var dawn = UnixTimeStampUtility.UnixTimeStampToDateTime(currentWeatherData.sys.sunrise).TimeOfDay;
            var dusk = UnixTimeStampUtility.UnixTimeStampToDateTime(currentWeatherData.sys.sunset).TimeOfDay;

            var icons = GetIcons(dawn, dusk);

            Icon = GetWeatherIcon(currentWeatherData.weather.FirstOrDefault()?.id, icons);
        }

        private IDictionary<int, string> GetIcons(TimeSpan dawn, TimeSpan dusk)
        {
            var now = DateTime.Now.TimeOfDay;

            if (dawn <= now && now <= dusk) //Day
                return Constants.WeatherIconsDay;

            return Constants.WeatherIconsNight;
        }

        private string GetWeatherIcon(int? id, IDictionary<int, string> icons)
        {
            if (!id.HasValue || !icons.ContainsKey(id.Value))
                return "?";

            return icons[id.Value];
        }

        public double Temperature
        {
            get { return _temperature; }
            private set
            {
                _temperature = value;
                RaisePropertyChanged();
            }
        }

        public string Icon
        {
            get { return _icon; }
            private set
            {
                _icon = value;
                RaisePropertyChanged();
            }
        }
    }
}