using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HomeScreen.Common;
using HomeScreen.Features.Agenda;
using HomeScreen.Features.Departures;
using HomeScreen.Features.StatusBar;
using HomeScreen.Features.Weather;
using HomeScreen.Features.CarInfo;

namespace HomeScreen
{
    public class MainViewModel : AsyncInitViewModelBase
    {
        private readonly Configuration _configuration;

        public MainViewModel()
        {
            _configuration = new Configuration();

            Status = new StatusBarViewModel();
            Weather = new WeatherViewModel(_configuration);
            Departures = new DeparturesViewModel(_configuration);            
            Agenda = new AgendaWorker(_configuration);
            CarInfo = new CarInfoViewModel();
        }        

        public WeatherViewModel Weather { get; private set; }

        public StatusBarViewModel Status { get; private set; }

        public DeparturesViewModel Departures { get; private set; }

        public AgendaWorker Agenda { get; private set; }

        public CarInfoViewModel CarInfo { get; private set; }

        public override async Task Init()
        {
            await _configuration.LoadConfiguration();

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
