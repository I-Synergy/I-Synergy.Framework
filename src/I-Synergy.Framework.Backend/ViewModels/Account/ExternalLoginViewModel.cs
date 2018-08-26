using System.ComponentModel.DataAnnotations;

namespace ISynergy.ViewModels.Account
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}