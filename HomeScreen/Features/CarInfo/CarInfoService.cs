using HomeScreen.Common;
using HomeScreen.Common.Configuration;
using HomeScreen.Features.CarInfo.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeScreen.Features.CarInfo
{
    public class CarInfoService
    {
        private const string GRANT_TYPE = "password";

        private readonly FeatureConfig _configuration;
        private readonly string _baseUrl;
        private readonly RESTClient _api;

        private Session _currentSession;

        public CarInfoService(FeatureConfig configuration)
        {
            _configuration = configuration;

            _baseUrl = _configuration.Settings["teslaApiUrl"];

            _api = new RESTClient(_baseUrl);
        }

        private bool IsLoggedIn
        {
            get
            {
                if (_currentSession == null)
                    return false;

                var created = UnixTimeStampUtility.UnixTimeStampToDateTime(_currentSession.created_at);

                var expires = created.AddSeconds(_currentSession.expires_in);

                return expires < DateTime.Now;
            }
        }

        private async Task Login()
        {
            var parameters = new LoginParameters
            {
                GrantType = "password",
                ClientId = _configuration.Settings["clientId"],
                ClientSecret = _configuration.Settings["clientSecret"],
                Email = _configuration.Settings["email"],
                Password = _configuration.Settings["password"]
            };

            _currentSession = await _api.PostAsync<Session>("oauth/token", parameters);
        }

        public async Task<ChargeState> GetChargeState(string vehicleId)
        {
            if (!IsLoggedIn)
                await Login();

            var path = $"api/1/vehicles/{vehicleId}/data_request/charge_state";

            return await _api.GetAsync<ChargeState>(path, new Dictionary<string, string> { { "Authorization", AuthorizationHeader } });
        }

        private string AuthorizationHeader
        {
            get
            {
                return $"Bearer {_currentSession?.access_token}";
            }
        }
    }
}
