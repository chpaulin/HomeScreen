using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Common.Configuration
{
    public class Config
    {
        public List<FeatureConfig> Features { get; set; }

        public FeatureConfig GetFeature(string feature)
        {
            return Features.FirstOrDefault(f => f.Name.ToUpperInvariant().Equals(feature.ToUpperInvariant()));
        }
    }
}
