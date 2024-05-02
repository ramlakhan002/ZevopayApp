using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Contracts
{
    public interface IPayoutsService
    {
        Task<ResponseModel> UpdateWalletAsync(string userId, string memberId, decimal amount);
        Task<ResponseModel> PayoutsUsingBankAccountAsync(ApplicationUser user, MoneyTransferModel model);
        Task<ResponseModel> UpiPayoutAsync(ApplicationUser user, UPIPayoutModel model);
        Task<ResponseModel> PayoutsLinkAsync(ApplicationUser user, PayoutsLinkModel model);
    }
}
