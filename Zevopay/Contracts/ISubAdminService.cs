using Zevopay.Models;

namespace Zevopay.Contracts
{
    public interface ISubAdminService
    {
        Task<SubAdminDto> GetSubAdminList(SubAdminDto model);
        Task<ResponseModel> UpdateSubAdminStatus(bool status, string Id);



    }
}
