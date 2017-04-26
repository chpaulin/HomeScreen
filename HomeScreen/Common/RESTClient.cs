using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Common
{
    public class RESTClient
    {
        private readonly string _baseAddress;

        public RESTClient(string baseAddress, string[] defaultHeaders = null)
        {
            _baseAddress = baseAddress;
        }

        public async Task<T> GetAsync<T>(string path, Dictionary<string, string> headers = null)
        {
            var url = $"{_baseAddress}{path}";

            var request = WebRequest.CreateHttp(url);

            foreach(var header in headers.Keys)
            {
                request.Headers[header] = headers[header];
            }

            var response = await request.GetResponseAsync();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var data = JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());

                return data;
            }
        }

        public async Task<T> PostAsync<T>(string path, object content, string[] headers = null)
        {
            var url = $"{_baseAddress}{path}";

            var request = WebRequest.CreateHttp(url);
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var contentStream = await request.GetRequestStreamAsync())
            using (var writer = new StreamWriter(contentStream))
            {
                var jsonContent = JsonConvert.SerializeObject(content);
                writer.Write(jsonContent);
            }

            var response = await request.GetResponseAsync();

            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var data = JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());

                return data;
            }
        }
    }  
}
