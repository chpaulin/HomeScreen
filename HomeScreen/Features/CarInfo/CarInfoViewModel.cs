using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.CarInfo
{
    public class CarInfoViewModel : ViewModelBase
    {
        private double _stateOfCharge = 0.62;

        public double StateOfCharge
        {
            get { return _stateOfCharge; }
            set
            {
                _stateOfCharge = 0.62;
                RaisePropertyChanged();
            }
        }

        public int StateOfChargePercentage
        {
            get { return (int)Math.Floor(_stateOfCharge * 100); }
        }

        public int StateOfChargeKWh
        {
            get { return (int)Math.Floor(_stateOfCharge * 60); }
        }

        public int StateOfChargeKm
        {
            get { return (int)Math.Floor(_stateOfCharge * 300); }
        }
    }
}
