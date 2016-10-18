using HomeScreen.Common;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Departures
{
    public class DeparturesService
    {
        private readonly Configuration _configuration;

        public DeparturesService(Configuration configuration)
        {
            _configuration = configuration;
        }


        public async Task<DepartureData> RetrieveDepartureData()
        {
            var outcome = await PollyUtility.ExecuteWebRequest(async () =>
                {
                    var url = _configuration.Settings["departuresDataUrl"];

                    var request = WebRequest.CreateHttp(url);
                    var response = await request.GetResponseAsync();

                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var data = JsonConvert.DeserializeObject<DepartureData>(await reader.ReadToEndAsync());

                        return data;
                    }
                });

            if (outcome.Result == null)
                return DepartureData.Empty;

            return outcome.Result;
        }
    }
}
