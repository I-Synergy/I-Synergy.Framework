using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Models.Accounts;

namespace ISynergy.Framework.Mvvm.Abstractions.ViewModels
{
    public interface ILoginViewModel : IViewModelNavigation<bool>
    {
        bool AutoLogin { get; set; }
        bool LoginVisible { get; set; }
        string Password { get; set; }
        string Registration_Mail { get; set; }
        ObservableCollection<Module> Registration_Modules { get; set; }
        string Registration_Name { get; set; }
        string Registration_Password { get; set; }
        string Registration_Password_Check { get; set; }
        string Registration_TimeZone { get; set; }
        ObservableCollection<TimeZoneInfo> TimeZones { get; set; }
        string Username { get; set; }
        ObservableCollection<string> Usernames { get; set; }

        Task CheckAutoLoginAsync();
        Task ForgotPasswordAsync();
        Task<bool> RegisterAsync();
    }
}
