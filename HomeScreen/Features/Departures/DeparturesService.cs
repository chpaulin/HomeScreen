﻿using HomeScreen.Common;
using HomeScreen.Common.Configuration;
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
        private readonly FeatureConfig _configuration;

        public DeparturesService(FeatureConfig configuration)
        {
            _configuration = configuration;
        }

        public async Task<DepartureData> RetrieveDepartureData(int count)
        {
            var outcome = await PollyUtility.ExecuteWebRequest(async () =>
                {
                    var url = string.Format(_configuration.Settings["departuresDataUrl"], count);

                    var request = WebRequest.CreateHttp(url);
                    var response = await request.GetResponseAsync();

                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var json = await reader.ReadToEndAsync();

                        var data = JsonConvert.DeserializeObject<DepartureData>(json);

                        return data;
                    }
                });

            if (outcome.Result == null)
                return DepartureData.Empty;

            return outcome.Result;
        }
    }
}
