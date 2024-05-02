using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Contracts
{
    public interface IAdminService
    {
        Task<ResponseModel> FundManageAsync(FundManageModel model);
        Task<IEnumerable<WalletTransactions>> GetWalletTransactionsAsync();
        Task<IEnumerable<Surcharge>> GetSurchagesAsync();
        Task<Surcharge> GetSurchagesByIdAsync(int Id);
        Task<ResponseModel> SaveSurchargeAsync(Surcharge model);

        Task<IEnumerable<WalletTransactions>> GetCeditDebitTransactions();

        Task<WalletTransactions> GetBalanceByUser(string Id);
        Task<WalletTransactions> GetTotalBalanceOfAllMembersAsync();

        Task<IEnumerable<Packages>> GetPackagesAsync();
        Task<ResponseModel> SavePackageAsync(Packages packages);
        Task<Packages> GetPackageByIdAsync(int Id);

    }
}
