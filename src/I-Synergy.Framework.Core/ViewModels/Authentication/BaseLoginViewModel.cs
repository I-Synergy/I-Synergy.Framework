using ISynergy.Handlers;
using ISynergy.Models.Accounts;
using ISynergy.Services;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System;
using ISynergy.Events;
using ISynergy.Mvvm;
using System.Collections.ObjectModel;

namespace ISynergy.ViewModels.Authentication
{
    public abstract class BaseLoginViewModel : ViewModelNavigation<object>, ILoginViewModel
    {
        public RelayCommand ShowLogin_Command { get; set; }
        public RelayCommand Register_Command { get; set; }
        public RelayCommand ForgotPassword_Command { get; set; }

        protected BaseLoginViewModel(
            IContext context,
            IBaseService baseService)
            : base(context, baseService)
        {
            this.Validator = new Action<IObservableClass>(_ =>
            {
                if(LoginVisible)
                {
                    if (!string.IsNullOrEmpty(Username) && !(Username.Length > 3))
                    {
                        Properties[nameof(Username)].Errors.Add(BaseService.LanguageService.GetString("Warning_UsernameSize"));
                    }

                    if (!string.IsNullOrEmpty(Password) && !Regex.IsMatch(Password, Constants.PasswordRegEx))
                    {
                        Properties[nameof(Password)].Errors.Add(BaseService.LanguageService.GetString("Warning_PasswordSize"));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Registration_Name) && !(Registration_Name.Length > 3))
                    {
                        Properties[nameof(Registration_Name)].Errors.Add(BaseService.LanguageService.GetString("Warning_LicenseNameSize"));
                    }

                    if (!string.IsNullOrEmpty(Registration_Mail) && !Network.IsValidEMail(Registration_Mail))
                    {
                        Properties[nameof(Registration_Mail)].Errors.Add(BaseService.LanguageService.GetString("Warning_Invalid_Email"));
                    }

                    if (string.IsNullOrEmpty(Registration_TimeZone))
                    {
                        Properties[nameof(Registration_TimeZone)].Errors.Add(BaseService.LanguageService.GetString("Warning_NoTimeZone_Selected"));
                    }

                    if (!string.IsNullOrEmpty(Registration_Password) && !(Registration_Password.Length > 6))
                    {
                        Properties[nameof(Registration_Password)].Errors.Add(BaseService.LanguageService.GetString("Warning_PasswordSize"));
                    }

                    if (!string.IsNullOrEmpty(Registration_Password) && !Regex.IsMatch(Registration_Password, Constants.PasswordRegEx))
                    {
                        Properties[nameof(Registration_Password)].Errors.Add(BaseService.LanguageService.GetString("Warning_PasswordSize"));
                    }

                    if (!string.IsNullOrEmpty(Registration_Password_Check) && !(Registration_Password_Check.Length > 6))
                    {
                        Properties[nameof(Registration_Password_Check)].Errors.Add(BaseService.LanguageService.GetString("Warning_PasswordSize"));
                    }

                    if (!string.IsNullOrEmpty(Registration_Password) && !string.IsNullOrEmpty(Registration_Password_Check) && !Registration_Password.Equals(Registration_Password_Check))
                    {
                        Properties[nameof(Registration_Password)].Errors.Add(BaseService.LanguageService.GetString("Warning_PasswordMatch"));
                        Properties[nameof(Registration_Password_Check)].Errors.Add(BaseService.LanguageService.GetString("Warning_PasswordMatch"));
                    }
                }
            });

            Usernames = new ObservableCollection<string>();
            TimeZones = new ObservableCollection<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones());
            Registration_TimeZone = "W. Europe Standard Time";

            LoginVisible = true;

            ShowLogin_Command = new RelayCommand(SetLoginVisibility);

            Register_Command = new RelayCommand(async () => await RegisterAsync());
            ForgotPassword_Command = new RelayCommand(async () => await ForgotPasswordAsync());

            if(BaseService.BaseSettingsService.Application_Users != null)
            {
                Usernames = JsonConvert.DeserializeObject<ObservableCollection<string>>(BaseService.BaseSettingsService.Application_Users);
            }

            Username = BaseService.BaseSettingsService.Application_User;
            AutoLogin = BaseService.BaseSettingsService.User_AutoLogin;
        }

        public Task CheckAutoLoginAsync()
        {
            if (BaseService.BaseSettingsService.User_AutoLogin && !string.IsNullOrEmpty(BaseService.BaseSettingsService.User_RefreshToken))
            {
                if (Submit_Command.CanExecute(null)) Submit_Command.Execute(null);
            }

            return Task.CompletedTask;
        }

        private void SetLoginVisibility()
        {
            if (LoginVisible)
            {
                LoginVisible = false;
            }
            else
            {
                LoginVisible = true;
            }
        }

        public override string Title { get { return BaseService.LanguageService.GetString("Generic_Login"); } }

        /// <summary>
        /// Gets or sets the Usernames property value.
        /// </summary>
        public ObservableCollection<string> Usernames
        {
            get { return GetValue<ObservableCollection<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Username property value.
        /// </summary>
        public string Username
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        public string Password
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets or sets the AutoLogin property value.
        /// </summary>
        public bool AutoLogin
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Name property value.
        /// </summary>
        public string Registration_Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Mail property value.
        /// </summary>
        public string Registration_Mail
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the TimeZones property value.
        /// </summary>
        public ObservableCollection<TimeZoneInfo> TimeZones
        {
            get { return GetValue<ObservableCollection<TimeZoneInfo>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_TimeZone property value.
        /// </summary>
        public string Registration_TimeZone
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Password property value.
        /// </summary>
        public string Registration_Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Password_Check property value.
        /// </summary>
        public string Registration_Password_Check
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Registration_Modules property value.
        /// </summary>
        public ObservableCollection<Module> Registration_Modules
        {
            get { return GetValue<ObservableCollection<Module>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the ShowLogin property value.
        /// </summary>
        public bool LoginVisible
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Override to implement authentication
        /// </summary>
        /// <example>
        /// <code>
        /// public override Task<bool> AuthenticateAsync()
        /// {
        ///     bool result = false;
        /// 
        ///     if (SettingsService.User_AutoLogin && !string.IsNullOrEmpty(SettingsService.User_RefreshToken))
        ///     {
        ///         await RestService.AuthenticateWithRefreshTokenAsync(SettingsService.User_RefreshToken);
        /// 
        ///         if (Context.CurrentProfile?.Token != null)
        ///         {
        ///             SettingsService.User_AutoLogin = true;
        ///         }
        ///         else
        ///         {
        ///             SettingsService.User_AutoLogin = false;
        ///         }
        ///     }
        ///     else
        ///     {
        ///         if (await CheckFields())
        ///              RestService.AuthenticateWithTokenAsync(Username, Password);
        ///     }
        /// 
        ///     if (Context.CurrentProfile != null && Context.CurrentProfile != null && Context.CurrentProfile.Token != null)
        ///     {
        ///         result = true;
        ///     }
        /// 
        ///     return result;
        /// }
        /// </code>
        /// </example>
        /// <returns>
        /// Context.CurrentProfile != null && Context.CurrentProfile != null && Context.CurrentProfile.Token != null
        /// </returns>
        public abstract Task<bool> AuthenticateAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// public override async Task<bool> RegisterAsync()
        /// {
        ///     bool result = false;
        /// 
        ///     if (await CheckFields() && await CheckRegistrationIsAvailable())
        ///     {
        ///         try
        ///         {
        ///             await SynergyService.BusyService.StartBusyAsync();
        /// 
        ///             result = await RestService?.RegisterExternal(new RegistrationData
        ///             {
        ///                 ApplicationId = 1,
        ///                 LicenseName = Registration_Name,
        ///                 Email = Registration_Mail,
        ///                 Password = Registration_Password,
        ///                 Modules = Registration_Modules,
        ///                 UsersAllowed = 1,
        ///                 TimeZoneId = Registration_TimeZone
        ///             });
        /// 
        ///             if (result)
        ///             {
        ///                 await DialogService
        ///                     .ShowInformationAsync(LanguageService.GetString("Warning_Registration_ConfirmEmail"));
        /// 
        ///                 Username = Registration_Mail;
        ///                 LoginVisible = true;
        ///             }
        ///         }
        ///         finally
        ///         {
        ///             await SynergyService.BusyService.EndBusyAsync();
        ///         }
        ///     }
        /// 
        ///     return result;
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public abstract Task<bool> RegisterAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// <code>
        /// public override async Task ForgotPasswordAsync()
        /// {
        ///     var result = await UIVisualizerService.ShowDialogAsync(
        ///         typeof(IForgotPasswordWindow),
        ///         new ForgotPasswordViewModel(
        ///             Context, 
        ///             Busy,
        ///             LanguageService,
        ///             SettingsService,
        ///             TelemetryService,
        ///             DialogService));
        /// 
        ///     if (result.HasValue && result.Value)
        ///     {
        ///         await DialogService
        ///                 .ShowInformationAsync(LanguageService.GetString("Warning_Reset_Password"));
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public abstract Task ForgotPasswordAsync();

        public override Task OnSubmitAsync(OnSubmitMessage e)
        {
            if (!e.Handled && e.Sender != null)
            {
                if (e.Sender.GetType().BaseType.Equals(this))
                {
                    if (BaseService.NavigationService.CanGoBack)
                        BaseService.NavigationService.GoBack();

                    e.Handled = true;
                }
            }

            return Task.CompletedTask;
        }

        public override Task OnCancelAsync(OnCancelMessage e)
        {
            if (!e.Handled && e.Sender != null)
            {
                if (e.Sender.GetType().BaseType.Equals(this))
                {
                    IsCancelled = true;

                    if (BaseService.NavigationService.CanGoBack)
                        BaseService.NavigationService.GoBack();

                    e.Handled = true;
                }
            }

            return Task.CompletedTask;
        }
    }
}