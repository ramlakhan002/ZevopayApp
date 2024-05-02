using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Zevopay.Contracts;
using Zevopay.Data;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Services
{
    public class AdminService : IAdminService
    {
        #region GlobalVariable
        private readonly IDapperDbContext _dapperDbContext;
        private readonly string _connectionString;
        private readonly string AdminFundCreditDebit_SP = "AdminFundCreditDebit";
        private readonly string Surcharge_SP = "SP_Surcharge";
        private readonly string Package_SP = "SP_Package";
        #endregion GlobalVariable End

        #region Constructor
        public AdminService(IDapperDbContext dapperDbContext, IConfiguration configuration)
        {
            _dapperDbContext = dapperDbContext;
            _connectionString = configuration.GetConnectionString("ZevopayDb") ?? throw new ArgumentNullException("Connection string");
        }
        #endregion Constructor End

        #region FundManage
        public async Task<ResponseModel> FundManageAsync(FundManageModel model)
        {
            return await SqlMapper.QueryFirstAsync<ResponseModel>(new SqlConnection(_connectionString), AdminFundCreditDebit_SP,
                new
                {
                    Action = 1,
                    ReceiverMemberID = model.MemberId,
                    Factor = model.Factor,
                    Description = model.Description,
                    Amount = model.Amount
                }, commandType: CommandType.StoredProcedure);

        }
        #endregion FundManage End

        #region GetWalletTransactions
        public async Task<IEnumerable<WalletTransactions>> GetWalletTransactionsAsync()
        {
            return await SqlMapper.QueryAsync<WalletTransactions>(new SqlConnection(_connectionString), "SP_CreditDebitTransaction",
            new
            {
                Action = 2,
            }, commandType: CommandType.StoredProcedure);


        }
        public async Task<IEnumerable<WalletTransactions>> GetCeditDebitTransactions()
        {
            try
            {
                return await SqlMapper.QueryAsync<WalletTransactions>(new SqlConnection(_connectionString), "SP_CreditDebitTransaction",
                new
                {
                    Action = 1,
                }, commandType: CommandType.StoredProcedure);

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion GetWalletTransactions End

        #region Package
        public async Task<IEnumerable<Packages>> GetPackagesAsync()
        {
            return await _dapperDbContext.QueryAsync<Packages>(Package_SP,
            new
            {
                Action = 1
            }, type: CommandType.StoredProcedure);

        }

        public async Task<ResponseModel> SavePackageAsync(Packages packages)
        {
            var action = packages?.PackageId == 0 ? 4 : 3;

            return await _dapperDbContext.QueryFirstOrDefaultAsync<ResponseModel>(Package_SP,
                new
                {
                    Action = action,
                    PackageName = packages?.PackageName,
                    PackageId = packages?.PackageId
                }, type: CommandType.StoredProcedure);
        }

        public async Task<Packages> GetPackageByIdAsync(int Id)
        {
            return await _dapperDbContext.QueryFirstOrDefaultAsync<Packages>(
                Package_SP,
                new
                {
                    Action = 2,
                    PackageId = Id
                }, type: CommandType.StoredProcedure);
        }
        #endregion Package End

        #region SurchargeList
        public async Task<IEnumerable<Surcharge>> GetSurchagesAsync()
        {
            return await SqlMapper.QueryAsync<Surcharge>(new SqlConnection(_connectionString), Surcharge_SP,
            new
            {
                Action = 1,
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<ResponseModel> SaveSurchargeAsync(Surcharge model)
        {
            var action = model?.Id == 0 ? 2 : 3;
            return await SqlMapper.QueryFirstAsync<ResponseModel>(new SqlConnection(_connectionString), Surcharge_SP,
               new
               {
                   Action = action,
                   Id = model.Id,
                   TransactionType = model.TransactionType,
                   RangeFrom = model.RangeFrom,
                   RangeTo = model.RangeTo,
                   IsFlat = model.IsFlat,
                   SurchargeAmount = model.SurchargeAmount,
                   PackageId = model.PackageId,
                   CreateDate = model.CreateDate

               }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Surcharge> GetSurchagesByIdAsync(int Id)
        {
            return await _dapperDbContext.QueryFirstOrDefaultAsync<Surcharge>(Surcharge_SP,
              new
              {
                  Action = 4,
                  Id = Id,
              }, type: CommandType.StoredProcedure);
        }
        #endregion SurchargeList End

        public async Task<WalletTransactions> GetBalanceByUser(string Id)
        {
            var query = $"select Balance  from WalletBalance where Id='{Id}'";
            var data = await _dapperDbContext.QueryFirstOrDefaultAsync<WalletTransactions>(query);
            return data;
        }
        public async Task<WalletTransactions> GetTotalBalanceOfAllMembersAsync()
        {
            var query = $"select Sum(Balance) as Balance from WalletBalance";
            var data = await _dapperDbContext.QueryFirstOrDefaultAsync<WalletTransactions>(query);
            return data;
        }
    }
}
