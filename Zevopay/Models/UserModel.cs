using Zevopay.Data.Entity;

namespace Zevopay.Models
{
    public class UserModel : ApplicationUser
    {
        public IFormFile? PanCardFrontImage { get; set; }
        public IFormFile? AadharCardFrontImage { get; set; }
        public IFormFile? PanCardBackImage { get; set; }
        public IFormFile? AadharCardBackImage { get; set; }
        public IFormFile? UserProfileImage { get; set; }
    }
}
