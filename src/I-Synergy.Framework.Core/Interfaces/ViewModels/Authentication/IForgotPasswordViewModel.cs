using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public interface IForgotPasswordViewModel
    {
        ISynergyService SynergyService { get; }
        string EmailAddress { get; set; }
        bool Mail_Valid { get; set; }
        string Title { get; }

        Task<bool> ResetPasswordAsync();
        Task SubmitAsync(object e);
    }
}
