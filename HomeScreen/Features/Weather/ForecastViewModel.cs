using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Weather
{
    public class ForecastViewModel : ViewModelBase
    {
        //public string Day { get; internal set; }
        public string Icon { get; internal set; }
        public double MaxTemperature { get; internal set; }
        public double MinTemperature { get; internal set; }
        public double Temperature { get; internal set; }
        public DateTime Date { get; internal set; }
    }
}
