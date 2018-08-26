﻿using System.ComponentModel.DataAnnotations;

namespace ISynergy.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}