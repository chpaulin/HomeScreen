using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace HomeScreen.Common.Configuration
{
    public class ConfigurationHelper
    {
        public Config LoadConfiguration()
        {
            var configFile = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration.json", UriKind.Absolute)).AsTask().Result;

            using (var stream = configFile.OpenReadAsync().AsTask().Result)
            using (var reader = new StreamReader(stream.AsStreamForRead()))
            {
                return JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
            }
        }
    }
}
