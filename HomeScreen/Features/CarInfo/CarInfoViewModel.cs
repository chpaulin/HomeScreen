using GalaSoft.MvvmLight;
using HomeScreen.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.CarInfo
{
    public class CarInfoViewModel : AsyncInitViewModelBase
    {
        private double _stateOfCharge = 0.62;

        public double StateOfCharge
        {
            get { return _stateOfCharge; }
            set
            {
                _stateOfCharge = value;
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

        public async override Task Init()
        {
            //TODO
            await Task.CompletedTask;
        }
    }
}
