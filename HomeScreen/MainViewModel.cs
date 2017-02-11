using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HomeScreen.Common;
using HomeScreen.Features.Agenda;
using HomeScreen.Features.Departures;
using HomeScreen.Features.StatusBar;
using HomeScreen.Features.Weather;
using HomeScreen.Features.CarInfo;
using HomeScreen.Common.Configuration;

namespace HomeScreen
{
    public class MainViewModel : AsyncInitViewModelBase
    {
        public MainViewModel()
        {
            var configHelper = new ConfigurationHelper();

            var configuration = configHelper.LoadConfiguration();

            Status = new StatusBarViewModel();
            Weather = new WeatherViewModel(configuration.GetFeature("weather"));
            Departures = new DeparturesViewModel(configuration.GetFeature("departures"));
            Agenda = new AgendaWorker(configuration.GetFeature("agenda"));
            CarInfo = new CarInfoViewModel();
        }

        public WeatherViewModel Weather { get; private set; }

        public StatusBarViewModel Status { get; private set; }

        public DeparturesViewModel Departures { get; private set; }

        public AgendaWorker Agenda { get; private set; }

        public CarInfoViewModel CarInfo { get; private set; }

        public override async Task Init()
        {                   
            await Task.WhenAny(
                Agenda.RunAsync(),
                Status.Init(),
                Weather.Init(),
                Departures.Init(),
                CarInfo.Init()
                );
        }
    }
}
