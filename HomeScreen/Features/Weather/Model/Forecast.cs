using System;

namespace HomeScreen.Features.Weather
{
    public class Forecast
    {
        public DateTime Date { get; set; }
        public double MaxTemperature { get; set; }
        public double MinTemperature { get; set; }
        public double Temperature { get; set; }
        public int? WeatherTypeId { get; set; }
    }
}