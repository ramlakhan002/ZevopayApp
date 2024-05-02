using Newtonsoft.Json;
using RestSharp;
using System.Text.Json.Serialization;
using Zevopay.Contracts;
using Zevopay.HelperClasses;
using Zevopay.Models;

namespace Zevopay.Services
{
    public class ApiService : IApiService
    {

        public async Task<PayoutsMoneyTransferResponseModel> PayoutsMoneyTransferResponseAsync(PayoutsMoneyTransferRequestModel requestModel)
        {
            var client = new RestClient(Constants.apiBaseUrl);
            client.AddDefaultHeader("Authorization", $"Basic {Base64Encode(Constants.apiKey + ":" + Constants.secretKey)}");
            var request = new RestRequest("payouts", Method.Post);
            request.AddJsonBody(requestModel);

            var response = await client.ExecuteAsync(request);
            if (response == null) return new() { Error = new Error() { Description = "Error!" } };
            return JsonConvert.DeserializeObject<PayoutsMoneyTransferResponseModel>(response.Content) ?? new();
        }

        public async Task<UpiPayoutResponseModel> UPIPayoutsAsync(UpiPayoutRequestModel requestModel)
        {
            var client = new RestClient(Constants.apiBaseUrl);
            client.AddDefaultHeader("Authorization", $"Basic {Base64Encode(Constants.apiKey + ":" + Constants.secretKey)}");
            var request = new RestRequest("payouts", Method.Post);
            request.AddJsonBody(requestModel);

            var response = await client.ExecuteAsync(request);
            if (response == null) return new() { Error = new Error() { Description = "Error!" } };
            return JsonConvert.DeserializeObject<UpiPayoutResponseModel>(response.Content) ?? new();
        }

        public async Task<PayoutsLinkResponseModel> PayoutsLinkAsync(PayoutsLinkRequestModel requestModel)
        {
            var client = new RestClient(Constants.apiBaseUrl);
            client.AddDefaultHeader("Authorization", $"Basic {Base64Encode(Constants.apiKey + ":" + Constants.secretKey)}");
            var request = new RestRequest("payout-links", Method.Post);
            request.AddJsonBody(requestModel);

            var response = await client.ExecuteAsync(request);
            if (response == null) return new() { Error = new Error() { Description = "Error!" } };
            return JsonConvert.DeserializeObject<PayoutsLinkResponseModel>(response.Content) ?? new();
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


    }
}
