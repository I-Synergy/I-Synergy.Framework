using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ISynergy.ViewModels.Exceptions
{
    public class ErrorViewModel
    {
        [Display(Name = "Error")]
        public string Error { get; set; }

        [Display(Name = "Description")]
        public string ErrorDescription { get; set; }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
