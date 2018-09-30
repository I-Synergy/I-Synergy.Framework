using GalaSoft.MvvmLight.Command;
using ISynergy.Events;
using ISynergy.Models.Accounts;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public interface ILoginViewModel
    {
        IBaseService BaseService { get; }
        bool AutoLogin { get; set; }
        RelayCommand ForgotPassword_Command { get; set; }
        RelayCommand<object> Submit_Command { get; set; }
        bool LoginVisible { get; set; }
        string Password { get; set; }
        RelayCommand Register_Command { get; set; }
        string Registration_Mail { get; set; }
        List<Module> Registration_Modules { get; set; }
        string Registration_Name { get; set; }
        string Registration_Password { get; set; }
        string Registration_Password_Check { get; set; }
        string Registration_TimeZone { get; set; }
        RelayCommand ShowLogin_Command { get; set; }
        List<TimeZoneInfo> TimeZones { get; set; }
        string Title { get; }
        string Username { get; set; }
        List<string> Usernames { get; set; }

        Task<bool> AuthenticateAsync();
        Task CheckAutoLogin();
        Task<bool> ForgotPasswordAsync();
        Task OnCancellationAsync(OnCancellationMessage e);
        Task OnSubmittanceAsync(OnSubmittanceMessage e);
        Task<bool> RegisterAsync();
        Task SubmitAsync(object e);
    }
}
