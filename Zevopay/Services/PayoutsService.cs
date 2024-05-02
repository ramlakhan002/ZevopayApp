using Dapper;
using System.Data;
using Zevopay.Contracts;
using Zevopay.Data;
using Zevopay.Data.Entity;
using Zevopay.HelperClasses;
using Zevopay.Models;

namespace Zevopay.Services
{
    public class PayoutsService : IPayoutsService
    {
        #region GlobalVariables
        private readonly IAdminService _adminService;
        private readonly ICommonService _commonService;
        private readonly IApiService _apiService;
        private readonly IDapperDbContext _context;
        #endregion GlobalVariables End

        #region Constructor
        public PayoutsService(IAdminService adminService, ICommonService commonService, IApiService apiService, IDapperDbContext context)
        {
            _adminService = adminService;
            _commonService = commonService;
            _apiService = apiService;
            _context = context;
        }
        #endregion Constructor End

        #region PayoutsBankAccount
        public async Task<ResponseModel> PayoutsUsingBankAccountAsync(ApplicationUser user, MoneyTransferModel model)
        {
            ResponseModel response = new();
            SurchargeModel surcharge = new();
            string referenceId = _commonService.RandamNumber(10);
            decimal surchargeAmount = 0;

            if (surcharge.SurchargeAmount > 0)
                surchargeAmount = surcharge.IsFlat ? surcharge.SurchargeAmount : (model.Amount * surcharge.SurchargeAmount) / 100;

            model.Amount += surchargeAmount;
            var updateWalletResult = await UpdateWalletAsync(user.Id, user.MemberId, model.Amount);

            if (updateWalletResult != null && updateWalletResult.ResultFlag == 0) 
                return updateWalletResult;

            var requestPayouts = new PayoutsMoneyTransferRequestModel
            {
                Mode = model.PaymentMode ?? string.Empty,
                Fund_account = new FundAccount
                {
                    Account_type = "bank_account",
                    Bank_account = new BankAccount
                    {
                        Name = model.FullName ?? string.Empty,
                        Ifsc = model.IFSCCode,
                        Account_number = model.AccountNumber.ToString()
                    },
                    Contact = new Contact
                    {
                        Name = model.FullName ?? string.Empty,
                        Email = "Ramlakhan@example.com",
                        contact = "9876543210",
                        Type = "vendor",
                        Reference_id = "Acme Contact ID 12345"
                    }
                },
                Account_number = Constants.PayoutAccountNumber,
                Amount = (int)model.Amount,
                Currency = "INR",
                Purpose = "refund",
                Queue_if_low_balance = true,
                Reference_id = referenceId,
                Narration = "Acme Corp Fund Transfer"
            };

            var apiResponse = await _apiService.PayoutsMoneyTransferResponseAsync(requestPayouts);

            if (apiResponse?.Error != null && !string.IsNullOrEmpty(apiResponse?.Error?.Description))
            {
                response.Message = apiResponse?.Error?.Description ?? string.Empty;
                response.ResultFlag = 0;
                return response;
            }

            await _context.ConnectDb.ExecuteAsync(
               "[dbo].[SP_PayoutTransaction]",
               new
               {
                   Action = 1,
                   Account_number = apiResponse.Fund_account.Bank_account?.Account_number,
                   Amount = model.Amount - surchargeAmount,
                   Fund_account_id = apiResponse.Fund_account_id,
                   Ifsc = apiResponse.Fund_account.Bank_account?.Ifsc,
                   MemeberId = user.MemberId,
                   Mode = apiResponse.Mode,
                   Narration = apiResponse.Narration,
                   Name = apiResponse.Fund_account.Bank_account?.Name,
                   PayoutId = apiResponse.Id,
                   Purpose = apiResponse.Purpose,
                   Status = "Pending",
                   Surcharge = surchargeAmount,
                   Reference_id = referenceId,
                   TotalAmount = model.Amount,
                   Utr = apiResponse.Utr
               }
, commandType: CommandType.StoredProcedure);

            return new ResponseModel() { ResultFlag = 1, Message = "Money successfully! Transferred" };
        }
        #endregion PayoutsBankAccount End

        #region UPIPayouts
        public async Task<ResponseModel> UpiPayoutAsync(ApplicationUser user, UPIPayoutModel model)
        {
            ResponseModel response = new();
            SurchargeModel surcharge = new();
            string referenceId = _commonService.RandamNumber(10);
            decimal surchargeAmount = 0;

            if (surcharge.SurchargeAmount > 0)
                surchargeAmount = surcharge.IsFlat ? surcharge.SurchargeAmount : (model.Amount * surcharge.SurchargeAmount) / 100;


            model.Amount += surchargeAmount;
            var updateWalletResult = await UpdateWalletAsync(user.Id, user.MemberId, model.Amount);

            if (updateWalletResult != null && updateWalletResult.ResultFlag == 0) return updateWalletResult;

            var upiPayoutmodel = new UpiPayoutRequestModel
            {
                Account_number = Constants.PayoutAccountNumber,
                Amount = (int)model.Amount,
                Currency = "INR",
                Mode = "UPI",
                Purpose = "refund",
                Fund_account = new FundAccountUPI
                {
                    Account_type = "vpa",
                    Vpa = new Vpa
                    {
                        Address = model.UPIId
                    },
                    Contact = new Contact
                    {
                        Name = "Gaurav Kumar",
                        Email = "gaurav.kumar@example.com",
                        contact = "9876543210",
                        Type = "self",
                        Reference_id = "Acme Contact ID 12345"
                    }
                },
                Queue_if_low_balance = true,
                Reference_id = referenceId,
                Narration = "Acme Corp Fund Transfer"
            };

            var apiResponse = await _apiService.UPIPayoutsAsync(upiPayoutmodel);

            if (apiResponse?.Error != null && !string.IsNullOrEmpty(apiResponse?.Error?.Description))
            {
                response.Message = apiResponse?.Error?.Description ?? string.Empty;
                response.ResultFlag = 0;
                return response;
            }

            await _context.ConnectDb.ExecuteAsync(
               "[dbo].[SP_PayoutTransaction]",
               new
               {
                   Action = 1,
                   Account_number = apiResponse.Fund_account?.Vpa.Address,
                   Amount = model.Amount - surchargeAmount,
                   Fund_account_id = apiResponse.Fund_account_id,
                   MemeberId = user.MemberId,
                   Mode = apiResponse.Mode,
                   Narration = apiResponse.Narration,
                   PayoutId = apiResponse.Id,
                   Purpose = apiResponse.Purpose,
                   Status = "Pending",
                   Surcharge = surchargeAmount,
                   Reference_id = referenceId,
                   TotalAmount = model.Amount,
                   Utr = apiResponse.Utr
               }
, commandType: CommandType.StoredProcedure);

            return new ResponseModel() { ResultFlag = 1, Message = "Money successfully! Transferred" };
        }
        #endregion UPIPayouts End

        #region PayoutsLink
        public async Task<ResponseModel> PayoutsLinkAsync(ApplicationUser user, PayoutsLinkModel model)
        {
            ResponseModel response = new();

            SurchargeModel surcharge = new();
            string referenceId = _commonService.RandamNumber(10);
            decimal surchargeAmount = 0;

            if (surcharge.SurchargeAmount > 0)
                surchargeAmount = surcharge.IsFlat ? surcharge.SurchargeAmount : (model.Amount * surcharge.SurchargeAmount) / 100;

            model.Amount += surchargeAmount;

            var updateWalletResult = await UpdateWalletAsync(user.Id, user.MemberId, model.Amount);

            if (updateWalletResult != null && updateWalletResult.ResultFlag == 0) return updateWalletResult;

            var payoutsLinkModel = new PayoutsLinkRequestModel
            {
                Account_number = Constants.PayoutAccountNumber,
                Contact = new Contact
                {
                    Name = "Ramlakhan",
                    contact = model.PhoneNumber,
                    Email = model.Email,
                    Type = "customer"
                },
                Amount = (int)model.Amount,
                Currency = "INR",
                Purpose = "refund",
                Description = "send Payout link to User",
                Receipt = "Receipt No. 1",
                Send_sms = true,
                Send_email = true
            };

            var apiResponse = await _apiService.PayoutsLinkAsync(payoutsLinkModel);

            if (apiResponse?.Error != null && !string.IsNullOrEmpty(apiResponse?.Error?.Description))
            {
                response.Message = apiResponse?.Error?.Description ?? string.Empty;
                response.ResultFlag = 0;
                return response;
            }

            await _context.ConnectDb.ExecuteAsync(
               "[dbo].[SP_PayoutTransaction]",
               new
               {
                   Action = 1,
                   Amount = model.Amount - surchargeAmount,
                   Fund_account_id = apiResponse.Fund_account_id,
                   MemeberId = user.MemberId,
                   PayoutId = apiResponse.Id,
                   Purpose = apiResponse.Purpose,
                   Status = "Pending",
                   Surcharge = surchargeAmount,
                   Reference_id = referenceId,
                   TotalAmount = model.Amount,
                   ContactNumber=model.PhoneNumber,
                   Email=model.Email
               }
, commandType: CommandType.StoredProcedure);


            return new ResponseModel() { ResultFlag = 1, Message = "Payouts links successfully! sent" };
        }
        #endregion PayoutsLink End

        #region UpdateWallet
        public async Task<ResponseModel> UpdateWalletAsync(string userId, string memberId, decimal amount)
        {
            var walletBalance = _adminService.GetBalanceByUser(userId).Result;

            if (walletBalance.Balance < amount) return new ResponseModel() { Message = "Insuficiance Balance!", ResultFlag = 0 };

            var updateFundModel = new FundManageModel()
            {
                Amount = amount,
                Factor = "Dr",
                MemberId = memberId,
                Description = "Amount Debited for upi payouts"
            };
            return await _adminService.FundManageAsync(updateFundModel);
        }
        #endregion UpdateWallet End

    }
}
