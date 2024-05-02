using Zevopay.Models;

namespace Zevopay.Contracts
{
    public interface ITwoFactorAuthService
    {
        LoginModel GenerateQrCode(string email);
        bool VerifyCode(string code);
    }
}
