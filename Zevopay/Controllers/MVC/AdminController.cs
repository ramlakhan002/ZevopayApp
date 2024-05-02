using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zevopay.Contracts;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Controllers.MVC
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminService _adminService;

        public AdminController(UserManager<ApplicationUser> userManager, IAdminService adminService)
        {
            _userManager = userManager;
            _adminService = adminService;
        }

        #region Credit Debit Transaction
        public IActionResult AdminCreditDebitTransactions()
        {
            return View();
        }

        public async Task<IActionResult> AdminCreditDebitTransactionsPartial()
        {
            return PartialView(await _adminService.GetCeditDebitTransactions());
        }
        #endregion Credit Debit Transaction

        #region Package
        public async Task<IActionResult> PackagesList()
        {
            return View(await _adminService.GetPackagesAsync());
        }

        public async Task<IActionResult> Packages(Packages packages)
        {
            if (packages?.PackageId != 0)
                packages = await _adminService.GetPackageByIdAsync(packages.PackageId);

            return View(packages);
        }

        public async Task<IActionResult> SavePackage(Packages package)
        {
            ResponseModel response = new();
            try
            {
                response = await _adminService.SavePackageAsync(package);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }
            return new JsonResult(response);

        }
        #endregion Package End

        #region FundManage
        public async Task<IActionResult> FundForm(FundManageModel model)
        {
            try
            {
                model.Users = await _userManager.Users.Where(u => u.Role != RolesConstants.AdminRole).Select(x => new SelectListItem()
                {
                    Text = $"{x.MemberId}  {x.FirstName} {x.LastName}",
                    Value = $"{x.MemberId},{x.Id}",
                }).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FundManage(FundManageModel model)
        {
            ResponseModel response = new();
            try
            {
                //return Ok();
                response = await _adminService.FundManageAsync(model);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }
            return new JsonResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetBalanceByUser(string Id)
        {
            WalletTransactions response = new();
            try
            {
                response = await _adminService.GetBalanceByUser(Id);
            }
            catch (Exception ex)
            {
            }
            return new JsonResult(response);
        }

        #endregion FundManage End

        #region WalletTransaction
        public IActionResult WalletTransactions()
        {
            return View();
        }

        public async Task<IActionResult> WalletTransactionsPartial()
        {
            return PartialView(await _adminService.GetWalletTransactionsAsync());

        }
        #endregion WalletTransaction End

        #region Surcharge
        public IActionResult SurchargeList()
        {
            return View();
        }
        public async Task<IActionResult> SurchargePartial()
        {
            return PartialView(await _adminService.GetSurchagesAsync());
        }

        public async Task<IActionResult> Surcharge(Surcharge model)
        {
            if (model?.Id != 0)
                model = await _adminService.GetSurchagesByIdAsync(model.Id);

            model.Packages = await _adminService.GetPackagesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSurcharge(Surcharge model)
        {
            ResponseModel response = new();
            try
            {
                response = await _adminService.SaveSurchargeAsync(model);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }
            return new JsonResult(response);
        }
        #endregion Surcharge End
    }
}
