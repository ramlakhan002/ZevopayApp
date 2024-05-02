using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Zevopay.Contracts;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Controllers.MVC
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IAdminService adminService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _adminService = adminService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(DashboardModel model)
        {
            try
            {
               var role = _userManager.GetRolesAsync(GetCurrentUserAsync().Result).Result.FirstOrDefault();

                if (role != RolesConstants.AdminRole)
                {

                    model.Balance = _adminService.GetBalanceByUser(GetCurrentUserAsync().Result.Id).Result.Balance;
                }
                else
                {
                    model.Balance = _adminService.GetTotalBalanceOfAllMembersAsync().Result.Balance;
                }
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }

        public async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
