using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zevopay.Contracts;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Controllers.MVC
{
    public class MemberController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;
        private readonly IPayoutsService _payoutsService;

        public MemberController(UserManager<ApplicationUser> userManager, IMemberService memberService, IPayoutsService payoutsService)
        {
            _userManager = userManager;
            _memberService = memberService;
            _payoutsService = payoutsService;
        }

        #region UPIPayouts
        public IActionResult UPIPayouts()
        {
            return View();
        }

        public async Task<IActionResult> UPIPayoutsSaveAsync(UPIPayoutModel model)
        {
            ResponseModel response = new();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User) ?? new();
                response = await _payoutsService.UpiPayoutAsync(user, model);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }
            return new JsonResult(response);
        }
        #endregion UPIPayouts End

        #region MoneyTransfer

        public IActionResult MoneyTransfer()
        {
            return View();
        }

        public async Task<IActionResult> MoneyTransferSaveAsync(MoneyTransferModel model)
        {
            ResponseModel response = new();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User) ?? new();
                response = await _payoutsService.PayoutsUsingBankAccountAsync(user, model);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }
            return new JsonResult(response);
        }
        #endregion MoneyTransfer End

        #region PayoutsLink
        public IActionResult PayoutsLink()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayoutsLinkSaveAsync(PayoutsLinkModel model)
        {
            ResponseModel response = new();
            try
            {
                ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User) ?? new();
                response = await _payoutsService.PayoutsLinkAsync(user, model);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }
            return new JsonResult(response);

        }
        #endregion PayoutsLink End



        public async Task<IActionResult> Wallet()
        {
            try
            {
                string userId = _userManager.GetUserAsync(HttpContext.User).Result.Id;

                var result = await _memberService.GetWalletBalanceRecordAsync(userId);
                return View(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> WalletTransactions()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> WalletTransactionsPartial()
        {
            try
            {
                return PartialView(await _memberService.GetWalletTransactionsAsync(_userManager.GetUserAsync(HttpContext.User).Result.Id));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
