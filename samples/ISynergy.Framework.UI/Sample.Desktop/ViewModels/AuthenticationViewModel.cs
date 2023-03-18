using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Sample.ViewModels
{
    public class AuthenticationViewModel : ViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IBaseApplicationSettingsService _applicationSettingsService;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Login"); } }

        /// <summary>
        /// Gets or sets the Usernames property value.
        /// </summary>
        /// <value>The usernames.</value>
        public ObservableConcurrentCollection<string> Usernames
        {
            get { return GetValue<ObservableConcurrentCollection<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Username property value.
        /// </summary>
        public string Username
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        public string Password
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        /// <value>The name of the registration.</value>
        public string Registration_Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mail property value.
        /// </summary>
        /// <value>The registration mail.</value>
        public string Registration_Mail
        {
            get { return GetValue<string>()?.ToLowerInvariant(); }
            set { SetValue(value?.ToLowerInvariant()); }
        }

        /// <summary>
        /// Gets or sets the TimeZones property value.
        /// </summary>
        /// <value>The time zones.</value>
        public List<string> TimeZones
        {
            get { return GetValue<List<string>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedTimeZone property value.
        /// </summary>
        /// <value>The registration time zone.</value>
        public string Registration_TimeZone
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        /// <value>The registration password.</value>
        public string Registration_Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PasswordCheck property value.
        /// </summary>
        /// <value>The registration password check.</value>
        public string Registration_PasswordCheck
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Modules property value.
        /// </summary>
        public List<Module> Modules
        {
            get => GetValue<List<Module>>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SelectedModules property value.
        /// </summary>
        /// <value>The registration modules.</value>
        public List<Module> Registration_Modules
        {
            get { return GetValue<List<Module>>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Countries property value.
        /// </summary>
        public List<Country> Countries
        {
            get => GetValue<List<Country>>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the SelectedCountry property value.
        /// </summary>
        public Country Registration_Country
        {
            get => GetValue<Country>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the ShowLogin property value.
        /// </summary>
        /// <value><c>true</c> if [login visible]; otherwise, <c>false</c>.</value>
        public bool LoginVisible
        {
            get { return GetValue<bool>(); }
            set
            {
                SetValue(value);
                ClearErrors();
            }
        }

        public RelayCommand ShowSignIn_Command { get; set; }
        public AsyncRelayCommand SignIn_Command { get; set; }
        public AsyncRelayCommand SignUp_Command { get; set; }
        public AsyncRelayCommand ForgotPassword_Command { get; set; }

        public AuthenticationViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILocalizationService localizationService,
            IAuthenticationService authenticationService,
            IBaseApplicationSettingsService applicationSettingsService,
            ILogger logger,
            bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation)
        {
            _authenticationService = authenticationService;
            _localizationService = localizationService;

            _applicationSettingsService = applicationSettingsService;
            _applicationSettingsService.LoadSettings();

            ShowSignIn_Command = new RelayCommand(SetLoginVisibility);
            SignIn_Command = new AsyncRelayCommand(SignInAsync);
            SignUp_Command = new AsyncRelayCommand(SignUpAsync);
            ForgotPassword_Command = new AsyncRelayCommand(ForgotPasswordAsync);

            Validator = new Action<IObservableClass>(_ =>
            {
                if (LoginVisible)
                {
                    if (string.IsNullOrEmpty(Username) || (Username.Length <= 3))
                    {
                        Properties[nameof(Username)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningUsernameSize"));
                    }

                    if (string.IsNullOrEmpty(Password) || !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    {
                        Properties[nameof(Password)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningPasswordSize"));
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Registration_Name) || (Registration_Name.Length <= 3))
                    {
                        Properties[nameof(Registration_Name)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningLicenseNameSize"));
                    }

                    if (string.IsNullOrEmpty(Registration_Mail) || !NetworkUtility.IsValidEMail(Registration_Mail))
                    {
                        Properties[nameof(Registration_Mail)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningInvalidEmail"));
                    }

                    if (string.IsNullOrEmpty(Registration_TimeZone))
                    {
                        Properties[nameof(Registration_TimeZone)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningNoTimeZoneSelected"));
                    }

                    if (string.IsNullOrEmpty(Registration_Password) || (Registration_Password.Length <= 6))
                    {
                        Properties[nameof(Registration_Password)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningPasswordSize"));
                    }

                    if (string.IsNullOrEmpty(Registration_Password) || !Regex.IsMatch(Registration_Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    {
                        Properties[nameof(Registration_Password)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningPasswordSize"));
                    }

                    if (string.IsNullOrEmpty(Registration_PasswordCheck) || (Registration_PasswordCheck.Length <= 6))
                    {
                        Properties[nameof(Registration_PasswordCheck)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningPasswordSize"));
                    }

                    if (!string.IsNullOrEmpty(Registration_Password) && !string.IsNullOrEmpty(Registration_PasswordCheck) && !Registration_Password.Equals(Registration_PasswordCheck))
                    {
                        Properties[nameof(Registration_Password)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningPasswordMatch"));
                        Properties[nameof(Registration_PasswordCheck)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningPasswordMatch"));
                    }

                    if (Registration_Modules.Count < 1)
                    {
                        Properties[nameof(Registration_Modules)].Errors.Add(BaseCommonServices.LanguageService.GetString("WarningNoModulesSelected"));
                    }
                }
            });

            Usernames = new ObservableConcurrentCollection<string>();
            Registration_TimeZone = null;
            Registration_Modules = new List<Module>();
            LoginVisible = true;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Modules = await _authenticationService.GetModulesAsync();

            Registration_Modules.Add(Modules.First());

            if (_applicationSettingsService.Settings.Users is List<string> users)
                Usernames = new ObservableConcurrentCollection<string>(users);

            Username = _applicationSettingsService.Settings.DefaultUser;
        }

        /// <summary>
        /// Sets the login visibility.
        /// </summary>
        private void SetLoginVisibility()
        {
            LoginVisible = !LoginVisible;
        }

        /// <summary>
        /// Forgots the password asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task ForgotPasswordAsync()
        {
            var forgotPasswordVM = new ForgotPasswordViewModel(Context, BaseCommonServices, _authenticationService, Logger);
            forgotPasswordVM.Submitted += ForgotPasswordVM_Submitted;
            return BaseCommonServices.DialogService.ShowDialogAsync(typeof(IForgotPasswordWindow), forgotPasswordVM);
        }

        /// <summary>
        /// Forgots the password vm submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void ForgotPasswordVM_Submitted(object sender, SubmitEventArgs<bool> e)
        {
            if (sender is ForgotPasswordViewModel vm)
                vm.Submitted -= ForgotPasswordVM_Submitted;

            if (e.Result)
            {
                await BaseCommonServices.DialogService
                        .ShowInformationAsync(BaseCommonServices.LanguageService.GetString("Warning_Reset_Password"));

                if (BaseCommonServices.NavigationService is INavigationService navigationService && navigationService.CanGoBack)
                    navigationService.GoBack();
            }
        }

        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Registration_Country))
            {
                if (Registration_Country is not null)
                {
                    TimeZones = _localizationService.GetTimeZoneIds(Registration_Country.ISO2Code);

                    if (TimeZones.Count == 1)
                        Registration_TimeZone = TimeZones[0];
                }
                else
                {
                    TimeZones = null;
                    Registration_TimeZone = null;
                }
            }
        }

        private Task SignUpAsync()
        {
            if (Validate())
            {
            }

            return Task.CompletedTask;
        }

        private Task SignInAsync()
        {
            if (Validate()) 
            {
                return BaseCommonServices.NavigationService.ReplaceMainWindowAsync<IShellView>();
            }

            return Task.CompletedTask;
        }
    }
}
