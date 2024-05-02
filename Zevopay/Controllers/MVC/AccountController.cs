using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Zevopay.Contracts;
using Zevopay.Data.Entity;
using Zevopay.Models;
using static QRCoder.PayloadGenerator;


namespace Zevopay.Controllers.MVC
{

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ISubAdminService _subAdminService;
        private readonly IAdminService _adminService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, IAccountService accountService, RoleManager<ApplicationRole> roleManager, ISubAdminService subAdminService,
             IAdminService adminService, ITwoFactorAuthService twoFactorAuthService, IConfiguration configuration)
        {
            _userManager = userManager;
            _accountService = accountService;
            _roleManager = roleManager;
            _subAdminService = subAdminService;
            _adminService = adminService;
            _twoFactorAuthService = twoFactorAuthService;
            _configuration = configuration;
        }

        #region Login/Logout Action 
        [AllowAnonymous]
        public IActionResult Login()
        {
            var IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated");

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ResponseModel response = new();
            try
            {
                bool isValid = _twoFactorAuthService.VerifyCode(model.AuthenticatorCode);
                if (isValid)
                {
                    response = await _accountService.Login(model);
                    if (response.ResultFlag == 1)
                    {
                        var user = _userManager.GetUserAsync(HttpContext.User).Result;
                        if (!user.isTwoFactorEnabled)
                            await _accountService.SetUserTwoFactorTrue(user.Id);
                    }
                }
                else
                {
                    response.ResultFlag = 0;
                    response.Message = "Google Two Factor PIN is expired or wrong!";
                }

            }
            catch (Exception ex)
            {
                response.ResultFlag = 0;
                response.Message = ex.Message;
            }
            return Json(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CheckCredentials(LoginModel model)
        {
            ResponseModel response = new();
            try
            {
                var result = await _accountService.CheckCredentialsAsync(model);
                if (result.ResultFlag == 1)
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(model.Email.Trim());
                    response.ResultFlag = 1;
                    if (!user.isTwoFactorEnabled)
                    {
                        response.ResultFlag = 2;
                        response.Data = _twoFactorAuthService.GenerateQrCode(user.Email);
                    }
                    else
                    {
                        HttpContext.Session.SetString("SecretKey", $"{user.Email}{_configuration["GoogleAuthKey"]}");
                    }

                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.ResultFlag = 0;
            }

            return Json(response);

        }

        public async Task<IActionResult> Logout()
        {
            _accountService.Logout();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        [HttpGet]
        public IActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();
            model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();
            return View(model);
            //  return PartialView("_AddUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            ApplicationUser user = new ApplicationUser
            {
                Name = model.Name,
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = model.Password,
                Address = "XYZ,",
                FirstName = model.Name,
                LastName = model.Name,
                Role = "Admin"

            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleId);
                if (applicationRole != null)
                {
                    IdentityResult roleResult = await _userManager.AddToRoleAsync(user, applicationRole.Name);
                    if (roleResult.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                }
            }

            return View(model);
        }

        #region Sub Admin 

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SubAdminList()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SubAdminListPrtial(SubAdminDto model)
        {
            try
            {
                model = await _subAdminService.GetSubAdminList(model);
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubAdminStatus(bool status, string Id)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                result = await _subAdminService.UpdateSubAdminStatus(status, Id);
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(result);
        }

        public async Task<ActionResult> SubAdmin(string id)
        {
            SubAdminModel model = new SubAdminModel();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Id
                    }).Where(x => x.Text != "Admin").ToList();
                    ApplicationUser user = await _userManager.FindByIdAsync(id);

                    if (user != null)
                    {
                        model.Id = user.Id;
                        model.FirstName = user.FirstName ?? string.Empty;
                        model.LastName = user.LastName ?? string.Empty;
                        model.Email = user.Email ?? string.Empty;
                        model.PhoneNumber = user.PhoneNumber ?? string.Empty;
                        model.ApplicationRoleId = _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == user.Role).Result.Id;
                        model.Address = user.Address ?? string.Empty;
                        model.UserName = user.UserName ?? string.Empty;

                    }
                }
                else
                {
                    model.ApplicationRoles = _roleManager.Roles.Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.Id
                    }).Where(x => x.Text != "Admin").ToList();
                }
            }
            catch (Exception ex)
            {
                throw;

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubAdmin(SubAdminModel model)
        {
            try
            {
                if (model.Id == null && model != null)
                {
                    ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleId);

                    ApplicationUser user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        UserName = model.UserName,
                        Name = model.FirstName + " " + model.LastName,
                        Address = model.Address,
                        Role = applicationRole?.Name,
                        CreateDate = model.CreateDate,
                        MemberId = $"RT{RandamNumber(6)}"
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        if (applicationRole != null)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, applicationRole.Name);
                            if (roleResult.Succeeded)
                            {
                                var UpdateUser = await _userManager.FindByEmailAsync(model.Email);
                                if (UpdateUser != null)
                                {

                                    FundManageModel FundManageMode = new FundManageModel()
                                    {
                                        MemberId = UpdateUser.MemberId,
                                        Factor = "Cr",
                                        Amount = 0,
                                        Description = "Registration",
                                    };
                                    _ = await _adminService.FundManageAsync(FundManageMode);

                                    UpdateUser.Role = applicationRole.Name;
                                }
                                var results = await _userManager.UpdateAsync(UpdateUser);

                                _ = await _subAdminService.UpdateSubAdminStatus(false, UpdateUser.Id);

                                return new JsonResult(new ResponseModel { ResultFlag = 1, Message = "Sub Admin is added successfully" });
                            }
                        }
                    }

                    return new JsonResult(new ResponseModel { ResultFlag = 0, Message = "Error! while Adding Sub Admin" });

                }
                else
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                    var result = new IdentityResult();
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;
                        user.Address = model.Address;
                        user.UserName = model.UserName;
                        user.PhoneNumber = model.PhoneNumber;
                        user.Name = model.FirstName + " " + model.LastName;
                        result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            return new JsonResult(new ResponseModel { ResultFlag = 1, Message = "Sub Admin is Updated successfully!" });
                        }

                    }
                    return new JsonResult(new ResponseModel { ResultFlag = 0, Data = result.Errors });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel { ResultFlag = 0, Message = ex.Message });
            }
        }

        #endregion


        public async Task<IActionResult> UserProfile(SubAdminModel model)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(GetCurrentUserAsync().Result.Id);

            if (user != null)
            {
                model.Id = user.Id;
                model.UserName = user.UserName;
                model.FirstName = user.FirstName ?? string.Empty;
                model.LastName = user.LastName ?? string.Empty;
                model.Email = user.Email ?? string.Empty;
                model.PhoneNumber = user.PhoneNumber ?? string.Empty;
                model.Address = user.Address ?? string.Empty;

            }
            return View(model);
        }

        private string RandamNumber(int digit)
        {
            Random generator = new Random();
            return generator.Next(0, 1000000).ToString($"D{digit}");
        }

        #region Forgot/Reset Password
        /* [AllowAnonymous]
         public IActionResult ForgotPassword()
         {
             return View();
         }*/

        /*[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                    if (user == null) return Json(new ResponseModel { ResultFlag = 0, Message = "User Not Found" });
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account",
                        new { email = user.Email, token = token }, protocol: HttpContext.Request.Scheme);

                    // Prepare the email content here, including the button with the callbackUrl
                    var emailContent = $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.";

                    await _emailService.SendEmailAsync(user.Email, "Reset Password", emailContent);

                    return Json(new ResponseModel { ResultFlag = 1, Message = "Please check your email to reset your password." });
                }
                else
                {
                    return Json(new ResponseModel { ResultFlag = 0, Message = "Invalid Email ID" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error! while excuting action {nameof(ForgotPassword)} errormessage '{ex.Message}'");
            }
            return Json(new ResponseModel { ResultFlag = 0, Message = "User Not Found" });
        }*/

        /*[HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }*/

        /*[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Json(await _accountService.ResetPassword(resetPasswordModel));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error! while excuting action {nameof(ResetPassword)} errormessage '{ex.Message}'");
            }
            return Json(new ResponseModel { ResultFlag = 0, Message = "Failed" });
        }*/
        #endregion

        public async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(HttpContext.User);
    }


}
