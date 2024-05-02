using Microsoft.AspNetCore.Identity;
using Zevopay.Contracts;
using Zevopay.Data.Entity;

namespace Zevopay.Services
{
    public class CommonService : ICommonService
    {
        public string RandamNumber(int digit)
        {
            Random generator = new Random();
            return generator.Next(0, 1000000).ToString($"D{digit}");
        }

    }
}
