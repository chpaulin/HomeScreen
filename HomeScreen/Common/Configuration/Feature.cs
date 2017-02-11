using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Common.Configuration
{
    public class FeatureConfig
    {
        private IDictionary<string, string> _settings;

        public string Name { get; set; }
        public List<KeyValue> Values { get; set; }

        public IDictionary<string, string> Settings
        {
            get
            {
                if(_settings == null)
                    _settings = Values.ToDictionary(v => v.Key, v => v.Value);

                return _settings;
            }
        }
    }
}
