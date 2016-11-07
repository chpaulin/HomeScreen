using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HomeScreen.Common;
using HomeScreen.Features.Agenda;
using HomeScreen.Features.Departures;
using HomeScreen.Features.StatusBar;
using HomeScreen.Features.Weather;

namespace HomeScreen
{
    public class MainViewModel : AsyncInitViewModelBase
    {
        public MainViewModel()
        {
            var configuration = new Configuration();

            Status = new StatusBarViewModel();
            Agenda = new AgendaViewModel(configuration);
            Weather = new WeatherViewModel(configuration);
            Departures = new DeparturesViewModel(configuration);
        }

        public WeatherViewModel Weather { get; private set; }

        public StatusBarViewModel Status { get; private set; }

        public DeparturesViewModel Departures { get; private set; }

        public AgendaViewModel Agenda { get; private set; }

        public override async Task Init()
        {            
            await Task.WhenAny(
                Status.Init(),
                Agenda.Init(),
                Weather.Init(),
                Departures.Init()
                );
        }
    }
}
