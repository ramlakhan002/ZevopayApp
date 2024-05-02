using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Zevopay.Contracts;
using Zevopay.Data;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Services
{
    public class MemberService : IMemberService
    {
        private readonly IDapperDbContext _context;

        public MemberService(IDapperDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel> CheckWalletBalanceAndUpdateAsync(decimal amount, string userId)
        {

            throw new NotImplementedException();
        }

        public async Task<Surcharge> GetSurchargeBasedPackageAsync()
        {
            return new();
        }

        public async Task<WalletModel> GetWalletBalanceRecordAsync(string userId)
        {
            return await _context.QueryFirstOrDefaultAsync<WalletModel>("SP_SubAdmin",
            new
            {
                Action = 2,
            }, type: CommandType.StoredProcedure);

        }

        public async Task<IEnumerable<MemberWalletTransactions>> GetWalletTransactionsAsync(string userId)
        {
            try
            {
                var data = await _context.QueryAsync<MemberWalletTransactions>("SP_SubAdmin",
                new
                {
                    Action = 3,
                    Id = userId
                }, type: CommandType.StoredProcedure);
                return data;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
