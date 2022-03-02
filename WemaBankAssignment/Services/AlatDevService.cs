using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;
using WemaBankAssignment.Integrations.AlatTechTest;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Models.Configurations;

namespace WemaBankAssignment.Services
{
    public class AlatDevService : IAlatDevService
    {
        private readonly AlatDevSettings _alatDevSettings;
        private readonly IUtilityService _utilityService;
        private readonly ILogger<AlatDevService> _log;

        public AlatDevService(IOptions<AlatDevSettings> alatDevSettings,
             IUtilityService utilityService,
             ILogger<AlatDevService> log)
        {
            _alatDevSettings = alatDevSettings.Value;
            _utilityService = utilityService;
            _log = log;
        }


        public async Task<GetBanksResponse> GetBanks()
        {
            try
            {

                var requestTime = DateTime.Now;
                var uri = _alatDevSettings.GetBanksUrl;
                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);
                request.AddHeader(_alatDevSettings.SubscriptionHeader, _alatDevSettings.SubscriptionKey);
                IRestResponse response = await client.ExecuteAsync(request);
                var responseTime = DateTime.Now;

                if (response.IsSuccessful)
                {
                    var result = JsonConvert.DeserializeObject<GetBanksResponse>(response.Content);

                    await _utilityService.SaveApiLogs("", "", requestTime, responseTime,
                        uri, response.Content, "", response.StatusCode.ToString(), response.StatusDescription,
                        ThirdParty.AlatDev.ToString(), "", null, null);

                    return result;
                }
                else
                {
                    await _utilityService.SaveApiLogs("", "", requestTime, responseTime,
                      uri, response.Content, "", response.StatusCode.ToString(), response.StatusDescription,
                      ThirdParty.AlatDev.ToString(), "", null, null);

                    return new GetBanksResponse
                    {
                        errorMessage = "Service unavailable",
                        hasError = true
                    };
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error in EpBackgroundService.SendCorporateOnboardingMail ->  {ex}");

                return new GetBanksResponse
                {
                    errorMessage = "Service unavailable",
                    hasError = true
                };
            }
        }
    }
}
