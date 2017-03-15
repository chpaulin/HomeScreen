using GalaSoft.MvvmLight;
using HomeScreen.Common;
using HomeScreen.Common.Configuration;
using HomeScreen.Features.CarInfo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.CarInfo
{
    public class CarInfoViewModel : AsyncInitViewModelBase
    {
        private readonly FeatureConfig _configuration;

        private double _stateOfCharge = 0.62;                
        private CarInfoService _service;

        public CarInfoViewModel(FeatureConfig configuration)
        {
            _configuration = configuration;            
        }

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
            _service = new CarInfoService(_configuration);

            await GetChargeState();

            Observable
               .Interval(TimeSpan.FromMinutes(30))
               .Subscribe(async (_) => await GetChargeState());
        }

        private async Task GetChargeState()
        {
            var chargeState = await _service.GetChargeState(_configuration.Settings["vehicleId"].As<int>());

            _stateOfCharge = (double)chargeState.response.battery_level / 100;

            RaisePropertyChanged(nameof(StateOfChargePercentage));
            RaisePropertyChanged(nameof(StateOfChargeKm));
            RaisePropertyChanged(nameof(StateOfCharge));
        }
    }
}
