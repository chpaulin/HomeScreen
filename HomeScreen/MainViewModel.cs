using GalaSoft.MvvmLight;
using HomeScreen.Common;
using HomeScreen.Features.Agenda;
using HomeScreen.Features.Departures;
using HomeScreen.Features.StatusBar;
using HomeScreen.Features.Weather;

namespace HomeScreen
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            var configuration = new Configuration();

            Status = new StatusBarViewModel();
            Agenda = new AgendaViewModel(configuration);
            Weather = new WeatherViewModel(configuration);            
            Departures = new DeparturesViewModel(configuration);
        }

        public WeatherViewModel Weather { get; }

        public StatusBarViewModel Status { get; }

        public DeparturesViewModel Departures { get; }

        public AgendaViewModel Agenda { get; }
    }
}
