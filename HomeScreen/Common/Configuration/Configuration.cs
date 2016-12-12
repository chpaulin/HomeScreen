using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HomeScreen.Common
{
    public class Configuration
    {
        public async Task LoadConfiguration()
        {
            var configFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Configuration.json", UriKind.Absolute));

            using (var stream = await configFile.OpenReadAsync())
            using (var reader = new StreamReader(stream.AsStreamForRead()))
            {
                var json = JObject.Parse(reader.ReadToEnd());

                var settings = json["settings"]
                    .ToArray()
                    .Select(t =>
                            new
                            {
                                Key = t["key"].Value<string>(),
                                Value = t["value"].Value<string>()
                            });


                foreach (var setting in settings)
                {
                    Settings.Add(setting.Key, setting.Value);
                }
            }

            Loaded = true;
        }      

        public IDictionary<string, string> Settings { get; } = new Dictionary<string, string>();

        public bool Loaded { get; private set; }
    }
}
