using HomeScreen.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Weather
{
    public class WeatherService
    {
        private readonly Configuration _configuration;

        public WeatherService(Configuration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CurrentWeatherConditions> RetrieveCurrentWeatherData()
        {
            var outcome = await PollyUtility.ExecuteWebRequest(async () =>
            {
                var url = _configuration.Settings["weatherDataUrl"];

                var request = WebRequest.CreateHttp(url);
                var response = await request.GetResponseAsync();

                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var data = JsonConvert.DeserializeObject<CurrentWeatherConditions>(await reader.ReadToEndAsync());

                    return data;
                }
            });

            if (outcome.Result == null)
                return CurrentWeatherConditions.Empty;

            return outcome.Result;
        }

        public async Task<IEnumerable<Forecast>> RetrieveForcastWeatherData()
        {
            var outcome = await PollyUtility.ExecuteWebRequest(async () =>
            {

                var url = _configuration.Settings["forcastDataUrl"];

                var request = WebRequest.CreateHttp(url);
                var response = await request.GetResponseAsync();

                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var data = JsonConvert.DeserializeObject<WeatherForecast>(await reader.ReadToEndAsync());

                    return data.list.Select(f => new Forecast
                    {
                        Temperature = f.main.temp,
                        MinTemperature = f.main.temp_min,
                        MaxTemperature = f.main.temp_max,
                        WeatherTypeId = f.weather.FirstOrDefault()?.id,
                        Date = UnixTimeStampUtility.UnixTimeStampToDateTime(f.dt)
                    });
                }
            });

            if (outcome.Result == null)
                return new List<Forecast>();

            return outcome.Result;
        }
    }
}
