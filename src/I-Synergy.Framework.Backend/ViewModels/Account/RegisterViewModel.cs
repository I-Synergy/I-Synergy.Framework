using ISynergy.Models.Accounts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISynergy.ViewModels.Account
{
    public class RegisterViewModel
    {
        public int ApplicationId
        {
            get
            {
                return 1;
            }
        }

        [Required]
        [Display(Name = "Modules")]
        public List<Module> Modules { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Value should be between 1 and 100.")]
        [Display(Name = "Number of users")]
        public int Users { get; set; }

        [Required]
        [StringLength(128, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Licensename")]
        public string Licensename { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}