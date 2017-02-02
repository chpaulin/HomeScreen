using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Weather
{
    public class ForecastViewModel : AsyncInitViewModelBase
    {
        private Forecast _forecast;
        private readonly IDictionary<int, string> _icons;
        private IDisposable _subscription;

        public ForecastViewModel(Forecast forecast, IDictionary<int, string> icons)
        {
            _forecast = forecast;
            _icons = icons;
        }

        public string Icon => GetWeatherIcon(_forecast.WeatherTypeId, _icons);

        public double MaxTemperature => _forecast.MaxTemperature;

        public double MinTemperature => _forecast.MinTemperature;

        public double Temperature => _forecast.Temperature;

        public DateTime Date => _forecast.Date;

        private string GetWeatherIcon(int? id, IDictionary<int, string> icons)
        {
            if (!id.HasValue || !icons.ContainsKey(id.Value))
                return "?";

            return icons[id.Value];
        }

        internal void Update(Forecast forecast)
        {
            _forecast = forecast;

            RaisePropertyChanged(() => Temperature);
            RaisePropertyChanged(() => Icon);
        }

        public override async Task Init()
        {
            await Task.CompletedTask;

            _subscription = Observable
            .Timer(Date - DateTime.Now)
            .ObserveOnDispatcher()
            .Subscribe((_) => Remove());
        }

        private void Remove()
        {
            _subscription.Dispose();

            Messenger.Default.Send(new ForecastDueEvent(this));
        }
    }
}
