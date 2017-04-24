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
        private const double ONE_MILE_IN_KM = 1.609344;

        private readonly FeatureConfig _configuration;
                     
        private CarInfoService _service;
        private ChargeState _chargeState = ChargeState.Empty;

        public CarInfoViewModel(FeatureConfig configuration)
        {
            _configuration = configuration;            
        }

        public double StateOfCharge
        {
            get { return (double)_chargeState.response.battery_level / 100; }
        }

        public int StateOfChargePercentage
        {
            get { return _chargeState.response.battery_level; }
        }       

        public int StateOfChargeKm
        {
            get { return (int)(_chargeState.response.ideal_battery_range * ONE_MILE_IN_KM); }
        }

        public async override Task Init()
        {
            _service = new CarInfoService(_configuration);

            await GetChargeState();

            Observable
               .Interval(TimeSpan.FromMinutes(30))
               .ObserveOnDispatcher()
               .Subscribe(async (_) => await GetChargeState());
        }

        private async Task GetChargeState()
        {
            var chargeState = await _service.GetChargeState(_configuration.Settings["vehicleId"]);

            _chargeState = chargeState;

            RaisePropertyChanged(nameof(StateOfChargePercentage));
            RaisePropertyChanged(nameof(StateOfChargeKm));
            RaisePropertyChanged(nameof(StateOfCharge));
        }
    }
}
