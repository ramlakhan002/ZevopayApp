using Zevopay.Models;

namespace Zevopay.Contracts
{
    public interface IApiService
    {
        Task<PayoutsMoneyTransferResponseModel> PayoutsMoneyTransferResponseAsync(PayoutsMoneyTransferRequestModel requestModel);
        Task<UpiPayoutResponseModel> UPIPayoutsAsync(UpiPayoutRequestModel requestModel);
        Task<PayoutsLinkResponseModel> PayoutsLinkAsync(PayoutsLinkRequestModel requestModel);

    }
}
