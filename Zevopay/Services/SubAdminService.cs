using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Data;
using Zevopay.Contracts;
using Zevopay.Data;
using Zevopay.Data.Entity;
using Zevopay.Models;

namespace Zevopay.Services
{

    public class SubAdminService : ISubAdminService
    {
        private readonly IDapperDbContext _dapperDbContext;
        private readonly string ConnectionString = string.Empty;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubAdminService(IDapperDbContext dapperDbContext, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _dapperDbContext = dapperDbContext;
            ConnectionString = configuration.GetConnectionString("ZevopayDb") ?? throw new ArgumentNullException("Connection string");
            _userManager = userManager;
        }

        public async Task<SubAdminDto> GetSubAdminList(SubAdminDto model)
        {
            try
            {

                var response = await SqlMapper.QueryMultipleAsync(new SqlConnection(ConnectionString),
               "SP_SubAdmin", new
               {
                   Action = 1,
                   //model.PageNumber,
                   //model.PageSize,
                   //model.SearchText,
               }, commandType: CommandType.StoredProcedure);

                model.SubAdminData = response.Read<SubAdminModel>().ToList();
                var totalRecord = response.Read<TotalRecords>().ToList();
                model.TotalRecord = totalRecord.FirstOrDefault().TotalRecord;
                return model;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ResponseModel> UpdateSubAdminStatus(bool status, string Id)
        {
            ResponseModel model = new ResponseModel();
            if (!string.IsNullOrEmpty(Id))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    IdentityResult result = new IdentityResult();
                    if (status)
                    {
                        await _userManager.SetLockoutEnabledAsync(user, status);
                        result = await _userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(100));
                        model.Message = "Account Deactivated Successfully!";
                    }
                    else
                    {
                        result = await _userManager.SetLockoutEnabledAsync(user, status);
                        model.Message = "Account Activated Successfully!";
                    }

                    model.Data = result.Errors.ToList();
                    model.ResultFlag = result.Succeeded ? 1 : 0;
                    model.Message = result.Succeeded ? model.Message : string.Empty;
                }
            }
            return model;
        }
    }



}
