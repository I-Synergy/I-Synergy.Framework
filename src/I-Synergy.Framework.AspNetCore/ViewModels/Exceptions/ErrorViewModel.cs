using System.ComponentModel.DataAnnotations;

namespace ISynergy.Framework.AspNetCore.ViewModels.Exceptions
{
    public class ErrorViewModel
    {
        [Display(Name = "Error")]
        public string Error { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string ErrorDescription { get; set; } = string.Empty;

        public string RequestId { get; set; } = string.Empty;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
