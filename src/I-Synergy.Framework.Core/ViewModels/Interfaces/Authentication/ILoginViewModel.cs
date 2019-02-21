using GalaSoft.MvvmLight.Command;
using ISynergy.Events;
using ISynergy.Models.Accounts;
using ISynergy.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ISynergy.ViewModels.Authentication
{
    public interface ILoginViewModel : IViewModelNavigation<object>
    {
        bool AutoLogin { get; set; }
        RelayCommand ForgotPassword_Command { get; set; }
        bool LoginVisible { get; set; }
        string Password { get; set; }
        RelayCommand Register_Command { get; set; }
        string Registration_Mail { get; set; }
        ObservableCollection<Module> Registration_Modules { get; set; }
        string Registration_Name { get; set; }
        string Registration_Password { get; set; }
        string Registration_Password_Check { get; set; }
        string Registration_TimeZone { get; set; }
        RelayCommand ShowLogin_Command { get; set; }
        ObservableCollection<TimeZoneInfo> TimeZones { get; set; }
        string Username { get; set; }
        ObservableCollection<string> Usernames { get; set; }

        Task<bool> AuthenticateAsync();
        Task CheckAutoLoginAsync();
        Task ForgotPasswordAsync();
        Task OnCancelAsync(OnCancelMessage e);
        Task OnSubmitAsync(OnSubmitMessage e);
        Task<bool> RegisterAsync();
        Task SubmitAsync(object e);
    }
}
