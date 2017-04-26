using HomeScreen.Features.Weather;

namespace HomeScreen.Messages
{
    class ForecastDueEvent
    {
        public ForecastDueEvent(ForecastViewModel forecastViewModel)
        {
            Forecast = forecastViewModel;
        }

        public ForecastViewModel Forecast { get; private set; }
    }
}