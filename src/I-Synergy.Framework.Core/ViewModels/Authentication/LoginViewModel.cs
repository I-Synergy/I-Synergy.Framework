using ISynergy.Common.Handlers;
using ISynergy.Models.Accounts;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using ISynergy.Views.Authentication;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CommonServiceLocator;
using System;
using ISynergy.Events;

namespace ISynergy.ViewModels.Authentication
{
    public class LoginViewModel : ViewModelNavigation<object>
    {
        public RelayCommand ShowLogin_Command { get; set; }
        public RelayCommand Register_Command { get; set; }
        public RelayCommand ForgotPassword_Command { get; set; }

        public LoginViewModel(IContext context, IBusyService busy)
            : base(context, busy)
        {
            TimeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

            LoginVisible = true;

            ShowLogin_Command = new RelayCommand(SetLoginVisibility);

            Register_Command = new RelayCommand(async () => await RegisterAsync());
            ForgotPassword_Command = new RelayCommand(async () => await ForgotPasswordAsync());

            Usernames = JsonConvert.DeserializeObject<List<string>>(ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Users);

            Username = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_User;
            AutoLogin = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin;
        }

        public Task CheckAutoLogin()
        {
            if (ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin && !string.IsNullOrEmpty(ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_RefreshToken))
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

        public override string Title { get { return ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Generic_Login"); } }
        
        /// <summary>
        /// Gets or sets the Usernames property value.
        /// </summary>
        public List<string> Usernames   
        {
            get { return GetValue<List<string>>(); }
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

        private async Task<bool> Check_Registration_Name(string e)
        {
            bool? result = false;

            if (e != null && e != "" && e.Length >= 3)
            {
                /* Call service asynchronously */
                result = await ServiceLocator.Current.GetInstance<IBaseRestService>()?.CheckIfLicenseIsAvailableAsync(e);

                if (result.Value == false)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_LICENSE_NAME"));
                }
            }
            else
            {
                await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_LicenseNameSize"));
            }

            return result.Value;
        }

        /// <summary>
        /// Gets or sets the Registration_Mail property value.
        /// </summary>
        public string Registration_Mail
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        private async Task<bool> Check_Registration_Mail(string e)
        {
            bool? result = false;

            if (e != null && e != "" && NetworkHandler.IsValidEMail(e))
            {
                result = await ServiceLocator.Current.GetInstance<IBaseRestService>()?.CheckIfEmailIsAvailableAsync(e);

                if (result.Value == false)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_LICENSE_EMAIL"));
                }
            }
            else
            {
                await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Invalid_Email"));
            }

            return result.Value;
        }

        /// <summary>
        /// Gets or sets the TimeZones property value.
        /// </summary>
        public List<TimeZoneInfo> TimeZones
        {
            get { return GetValue<List<TimeZoneInfo>>(); }
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
        public List<Module> Registration_Modules
        {
            get { return GetValue<List<Module>>(); }
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
        
        private async Task<bool> CheckFields()
        {
            bool result = true;

            if (LoginVisible)
            {
                if (Username == null || Username.Length < 3)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_UsernameSize"));
                    result = false;
                }

                if (Password == null || Password.Length < 6)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_PasswordSize"));
                    result = false;
                }

                ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin = AutoLogin;
            }
            else
            {
                if (Registration_Name == null || Registration_Name.Length < 3)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_LicenseNameSize"));
                    result = false;
                }

                if (Registration_Mail == null || !NetworkHandler.IsValidEMail(Registration_Mail))
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Invalid_Email"));
                    result = false;
                }

                if(string.IsNullOrEmpty(Registration_TimeZone))
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_NoTimeZone_Selected"));
                    result = false;
                }

                if (Registration_Password == null || Registration_Password.Length < 6)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_PasswordSize"));
                    result = false;
                }

                Match passwordMatch = Regex.Match(Registration_Password, Constants.PasswordRegEx, RegexOptions.None);

                if (!passwordMatch.Success)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_PasswordSize"));
                    result = false;
                }

                if (Registration_Password_Check == null || Registration_Password_Check.Length < 6)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_PasswordSize"));
                    result = false;
                }

                if (!Registration_Password.Equals(Registration_Password_Check))
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowErrorAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_PasswordMatch"));
                    result = false;
                }
            }

            return result;
        }

        public override async Task SubmitAsync(object e)
        {
            if (ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin && !string.IsNullOrEmpty(ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_RefreshToken))
            {
                await ServiceLocator.Current.GetInstance<IBaseRestService>().AuthenticateWithRefreshTokenAsync(ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_RefreshToken);

                if (Context.CurrentProfile?.Token != null)
                {
                    ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin = true;
                }
                else
                {
                    ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin = false;
                }
            }
            else
            {
                if (await CheckFields())
                    await ServiceLocator.Current.GetInstance<IBaseRestService>().AuthenticateWithTokenAsync(Username, Password);
            }

            if (Context.CurrentProfile != null && Context.CurrentProfile != null && Context.CurrentProfile.Token != null)
            {
                List<string> users = JsonConvert.DeserializeObject<List<string>>(ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Users);

                if (users == null) users = new List<string>();

                if (!users.Contains(Username))
                {
                    users.Add(Username.ToLower());

                    ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Users = JsonConvert.SerializeObject(users.Distinct().ToList());
                }

                ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_User = Username;
                ServiceLocator.Current.GetInstance<ISettingsServiceBase>().User_AutoLogin = AutoLogin;

                Messenger.Default.Send(new LoginAuthenticationMessage());
            }
        }

        private async Task<bool> CheckRegistrationIsAvailable()
        {
            if (await Check_Registration_Name(Registration_Name) &&
                await Check_Registration_Mail(Registration_Mail) &&
                Registration_Password_Check != null && Registration_Password != null &&
                Registration_Password_Check.Equals(Registration_Password) &&
                Regex.IsMatch(Registration_Password, Constants.PasswordRegEx))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task RegisterAsync()
        {
            if (await CheckFields() && await CheckRegistrationIsAvailable())
            {
                await Busy.StartBusyAsync();

                var result = await ServiceLocator.Current.GetInstance<IBaseRestService>()?.RegisterExternal(new RegistrationData
                {
                    ApplicationId = 1,
                    LicenseName = Registration_Name,
                    Email = Registration_Mail,
                    Password = Registration_Password,
                    Modules = Registration_Modules,
                    UsersAllowed = 1,
                    TimeZoneId = Registration_TimeZone
                });

                if (result)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowInformationAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Registration_ConfirmEmail"));

                    Username = Registration_Mail;
                    LoginVisible = true;
                }

                await Busy.EndBusyAsync();
            }
        }

        private async Task ForgotPasswordAsync()
        {
            var result = await ServiceLocator.Current.GetInstance<IUIVisualizerService>().ShowDialogAsync(
                typeof(IForgotPasswordWindow),
                new ForgotPasswordViewModel(Context, Busy));

            if (result.HasValue && result.Value)
            {
                await ServiceLocator.Current.GetInstance<IDialogService>()
                        .ShowInformationAsync(ServiceLocator.Current.GetInstance<ILanguageService>().GetString("Warning_Reset_Password"));
            }
        }

        public override Task OnSubmittanceAsync(OnSubmittanceMessage e)
        {
            if (!e.Handled)
            {
                if (ServiceLocator.Current.GetInstance<INavigationService>().CanGoBack)
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();

                e.Handled = true;
            }

            return Task.CompletedTask;
        }

        public override Task OnCancellationAsync(OnCancellationMessage e)
        {
            if (!e.Handled)
            {
                IsCancelled = true;

                if (ServiceLocator.Current.GetInstance<INavigationService>().CanGoBack)
                    ServiceLocator.Current.GetInstance<INavigationService>().GoBack();

                e.Handled = true;
            }

            return Task.CompletedTask;
        }
    }
}