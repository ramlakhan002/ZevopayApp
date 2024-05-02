using Azure;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zevopay.Contracts;
using Zevopay.Data;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDapperDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICommonService _commonService;
        private readonly IAdminService _adminService;
        private readonly ISubAdminService _subAdminService;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IDapperDbContext context, RoleManager<ApplicationRole> roleManager, ICommonService commonService, IAdminService adminService, ISubAdminService subAdminService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
            _commonService = commonService;
            _adminService = adminService;
            _subAdminService = subAdminService;
        }
        public async Task<ResponseModel> Login(LoginModel model)
        {
            ApplicationUser user = new();
            ResponseModel response = new ResponseModel();

            user = await _userManager.FindByEmailAsync(model.Email.Trim());

            if (user == null)
            {
                response.ResultFlag = 0; response.Message = "User not found!";
                return response;
            }
            else if (await _userManager.CheckPasswordAsync(user, model.Password.Trim()) == false)
            {
                response.ResultFlag = 0; response.Message = "Invalid credentials!";
                return response;
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password.Trim(), model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                response.ResultFlag = 1; response.Message = "success!";
            }
            else if (result.IsLockedOut)
            {
                response.ResultFlag = 0; response.Message = "Your account is not active!";
            }
            else
            {
                response.ResultFlag = 0; response.Message = "Invalid login attempt!";
            }
            return response;
        }


        public async Task<ResponseModel> CheckCredentialsAsync(LoginModel model)
        {
            ResponseModel response = new ResponseModel();

            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email.Trim());

            if (user == null)
            {
                response.ResultFlag = 0; response.Message = "User not found!";
            }
            else if (await _userManager.CheckPasswordAsync(user, model.Password.Trim()) == false)
            {
                response.ResultFlag = 0; response.Message = "Invalid credentials!";
            }
            else if (user.LockoutEnabled)
            {
                response.ResultFlag = 0; response.Message = "Your account is not active!!";
            }
            else
            {
                response.ResultFlag = 1;
            }
            return response;
        }
        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task SetUserTwoFactorTrue(string userId)
        {
            string query = "UPDATE AspNetUsers SET isTwoFactorEnabled = @IsTwoFactorEnabled WHERE Id = @UserId";
            await _context.ConnectDb.ExecuteAsync(query, new { IsTwoFactorEnabled = true, UserId = userId });

        }

        public async Task<ResponseModel> SaveMemberAsync(SubAdminModel model)
        {
            if (model.Id == null && model != null)
            {
                ApplicationUser isExistUser = await _userManager.FindByEmailAsync(model.Email);
                if (isExistUser != null)
                {
                    return new ResponseModel { ResultFlag = 2, Message = "Email already Exist !!" };
                }

                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(model.ApplicationRoleId);

                ApplicationUser user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Email,
                    Name = model.FirstName + " " + model.LastName,
                    Address = model.Address,
                    Role = applicationRole?.Name,
                    CreateDate = model.CreateDate,
                    MemberId = $"RT{_commonService.RandamNumber(6)}"
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

                            return new ResponseModel { ResultFlag = 1, Message = "Sub Admin is added successfully" };
                        }
                    }
                }

                return new ResponseModel { ResultFlag = 0, Data = result.Errors.ToList() };

            }
            else
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);


                var result = new IdentityResult();
                if (user != null)
                {
                    if (user.Email != model.Email)
                    {
                        ApplicationUser isExistUser = await _userManager.FindByEmailAsync(model.Email);
                        if (isExistUser != null)
                        {
                            return new ResponseModel { ResultFlag = 2, Message = "Email already Exist !!" };
                        }
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Address = model.Address;
                    user.UserName = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Name = model.FirstName + " " + model.LastName;
                    result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return new ResponseModel { ResultFlag = 1, Message = "Sub Admin is Updated successfully!" };
                    }

                }

                return new ResponseModel { ResultFlag = 2, Message = "User not found!" };
            }
        }

        /*public async Task<ResponseModel> ResetPassword(ResetPasswordModel model)
        {
            ResponseModel response = new ResponseModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                response = new ResponseModel { ResultFlag = 0, Message = "Invalid Eamil!" };
            }
            else
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (!resetPassResult.Succeeded)
                {
                    var errorMessages = new List<string>();
                    foreach (var error in resetPassResult.Errors)
                    {
                        errorMessages.Add(error.Description);
                    }
                    response = new ResponseModel
                    {
                        ResultFlag = 0, // Assuming 0 indicates failure
                        Message = string.Join(", ", errorMessages)
                    };
                }
                else
                {
                    response = new ResponseModel { ResultFlag = 1, Message = "Password is updated successfully " };
                }
            }
            return response;
        }*/

    }






}
