using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Zevopay.Data.Entity;

namespace Zevopay.Models
{
    public class SubAdminModel
    {
        public string Id { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Enter First Name")]
        public string FirstName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; }
        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Phone")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$", ErrorMessage = "Invalid phone number.")]
        public string MemberId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }


        [Display(Name = "Role")]
        [Required(ErrorMessage = "Select Role")]
        public string ApplicationRoleId { get; set; }

        public List<SelectListItem> ApplicationRoles { get; set; }

        [Required(ErrorMessage = "Select Role")]
        public List<ApplicationUser> UsersList { get; set; }
        public List<IdentityError> IdentityError { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalCount { get; set; }
        public string? SearchText { get; set; }
        public int TotalRecord { get; set; }
        public int AadharNumber { get; set; }

        public string? Role { get; set; }
        public string? PanNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }



    }
   

}
