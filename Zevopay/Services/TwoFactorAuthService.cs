using Google.Authenticator;
using Zevopay.Contracts;
using static QRCoder.PayloadGenerator;
using Zevopay.Models;
using System.Text;

namespace Zevopay.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public TwoFactorAuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public LoginModel GenerateQrCode(string email)
        {
            LoginModel model = new();
            string googleAuthKey = _configuration["GoogleAuthKey"];
            string UserUniqueKey = $"{email}{googleAuthKey}";

            //Two Factor Authentication Setup
            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            var setupInfo = TwoFacAuth.GenerateSetupCode("Zevopay", email, ConvertSecretToBytes(UserUniqueKey, false), 300);
            _httpContextAccessor.HttpContext.Session.SetString("SecretKey", UserUniqueKey);

            model.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            model.Email = email;
            return model;
        }

        public bool VerifyCode(string code)
        {
            string UserUniqueKey = _httpContextAccessor.HttpContext.Session.GetString("SecretKey").ToString();

            TwoFactorAuthenticator TwoFacAuth = new TwoFactorAuthenticator();
            return TwoFacAuth.ValidateTwoFactorPIN(UserUniqueKey, code, false);
        }

        private static byte[] ConvertSecretToBytes(string secret, bool secretIsBase32) =>
           secretIsBase32 ? Base32Encoding.ToBytes(secret) : Encoding.UTF8.GetBytes(secret);
    }

}
