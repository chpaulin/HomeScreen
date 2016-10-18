using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace HomeScreen.Features.Agenda
{
    public class AgendaService
    {
        private readonly Configuration _configuration;

        public AgendaService(Configuration configuration)
        {
            _configuration = configuration;

            var timer = ThreadPoolTimer.CreatePeriodicTimer(async (_) =>
            {
                await RefreshAccessToken();

            }, TimeSpan.FromMinutes(30));

            if (configuration.Loaded)
                Task.Run(RefreshAccessToken);
            else
                Messenger.Default.Register<ConfigurationLoadedEvent>(this, async (_) => await RefreshAccessToken());
        }

        private async Task RefreshAccessToken()
        {
            var urlTemplate = _configuration.Settings["agendaRefreshTokenUrl"];
            var url = string.Format(urlTemplate, _configuration.Settings["agendaRefreshToken"]);

            var request = WebRequest.CreateHttp(url);
            request.Accept = "application/json";
            var response = await request.GetResponseAsync();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();

                var data = JsonConvert.DeserializeObject<TokenRefreshResponse>(json);
            }
        }

        public async Task<AgendaData> RetrieveAgendaData()
        {
            var startTime = DateTime.Today;
            var endTime = startTime.AddHours(23).AddMinutes(59).AddSeconds(59);

            var urlTemplate = _configuration.Settings["agendaDataUrl"];
            var url = string.Format(urlTemplate, startTime.ToString("s"), endTime.ToString("s"));

            var request = WebRequest.CreateHttp(url);
            request.Headers["Authorization"] = $"Bearer {_configuration.Settings["agendaAccessToken"]}";
            request.Headers["Accept"] = "text/*, application/xml, application/json; odata.metadata=none";
            var response = await request.GetResponseAsync();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var data = JsonConvert.DeserializeObject<AgendaData>(await reader.ReadToEndAsync());

                return data;
            }
        }

        public async Task<DateSignificanceData> RetrieveDateSigificanceData(DateTime date)
        {
            var urlTemplate = _configuration.Settings["dateSignificanceUrl"];
            var url = string.Format(urlTemplate, date.ToString("yyyy/m/d"));

            var request = WebRequest.CreateHttp(url);
            var response = await request.GetResponseAsync();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var data = JsonConvert.DeserializeObject<DateSignificanceData>(await reader.ReadToEndAsync());

                return data;
            }
        }
    }
}
