using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Windows;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.ViewModels
{
    public class RegistrationViewModel : ViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Gets or sets the Name property value.
        /// </summary>
        /// <value>The name of the registration.</value>
        public string Name
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mail property value.
        /// </summary>
        /// <value>The registration mail.</value>
        public string Mail
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
        public string SelectedTimeZone
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Password property value.
        /// </summary>
        /// <value>The registration password.</value>
        public string Password
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the PasswordCheck property value.
        /// </summary>
        /// <value>The registration password check.</value>
        public string PasswordCheck
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
        public List<Module> SelectedModules
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
        public Country SelectedCountry
        {
            get => GetValue<Country>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the ArePickersAvailable property value.
        /// </summary>
        public bool ArePickersAvailable
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public AsyncRelayCommand RegisterCommand { get; set; }
        public AsyncRelayCommand ValidateMailCommand { get; set; }
        public AsyncRelayCommand LoginCommand { get; set; }
        public AsyncRelayCommand SelectModulesCommand { get; set; }

        public RegistrationViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            ILogger logger,
            bool automaticValidation = false)
            : base(context, commonServices, logger, automaticValidation)
        {
            _authenticationService = authenticationService;
            _localizationService = localizationService;

            this.Validator = new Action<IObservableClass>(_ =>
            {
                if (string.IsNullOrEmpty(Name) || (Name.Length <= 3))
                {
                    AddValidationError(nameof(Name), commonServices.LanguageService.GetString("WarningLicenseNameSize"));
                }

                if (string.IsNullOrEmpty(Mail) || !NetworkUtility.IsValidEMail(Mail))
                {
                    AddValidationError(nameof(Mail), commonServices.LanguageService.GetString("WarningInvalidEmail"));
                }

                if (string.IsNullOrEmpty(SelectedTimeZone))
                {
                    AddValidationError(nameof(SelectedTimeZone), commonServices.LanguageService.GetString("WarningNoTimeZoneSelected"));
                }

                if (string.IsNullOrEmpty(Password) || (Password.Length <= 6))
                {
                    AddValidationError(nameof(Password), commonServices.LanguageService.GetString("WarningPasswordSize"));
                }

                if (string.IsNullOrEmpty(Password) || !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                {
                    AddValidationError(nameof(Password), commonServices.LanguageService.GetString("WarningPasswordSize"));
                }

                if (string.IsNullOrEmpty(PasswordCheck) || (PasswordCheck.Length <= 6))
                {
                    AddValidationError(nameof(PasswordCheck), commonServices.LanguageService.GetString("WarningPasswordSize"));
                }

                if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(PasswordCheck) && !Password.Equals(PasswordCheck))
                {
                    AddValidationError(nameof(Password), commonServices.LanguageService.GetString("WarningPasswordMatch"));
                    AddValidationError(nameof(PasswordCheck), commonServices.LanguageService.GetString("WarningPasswordMatch"));
                }

                if (SelectedModules.Count < 1)
                {
                    AddValidationError(nameof(SelectedModules), commonServices.LanguageService.GetString("WarningNoModulesSelected"));
                }

                if (SelectedCountry is null)
                    AddValidationError(nameof(SelectedCountry), commonServices.LanguageService.GetString("WarningNoCountrySelected"));
            });

            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            ValidateMailCommand = new AsyncRelayCommand(ValidateMailAsync);
            LoginCommand = new AsyncRelayCommand(SignInAsync);
            SelectModulesCommand = new AsyncRelayCommand(SelectModulesAsync);

            ArePickersAvailable = false;
        }

        private Task SelectModulesAsync()
        {
            var selectionVM = new ViewModelSelectionDialog<Module>(Context, BaseCommonServices, Logger, Modules, SelectedModules, SelectionModes.Multiple);
            selectionVM.Submitted += SelectionVM_Submitted;
            return BaseCommonServices.DialogService.ShowDialogAsync(typeof(SelectionWindow), selectionVM);
        }

        /// <summary>
        /// Editors the vm submitted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SelectionVM_Submitted(object sender, SubmitEventArgs<List<Module>> e)
        {
            List<Module> selectedItems = new();

            foreach (object item in e.Result)
            {
                if (item is Module module)
                    selectedItems.Add(module);
            }

            SelectedModules = selectedItems;

            if (sender is ViewModelSelectionDialog<Module> vm)
                vm.Submitted -= SelectionVM_Submitted;
        }

        private async Task ValidateMailAsync()
        {
            if (!string.IsNullOrEmpty(Mail) && NetworkUtility.IsValidEMail(Mail.GetNormalizedCredentials()))
            {
                ArePickersAvailable = true;

                List<Module> modules = await _authenticationService.GetModulesAsync();
                List<Country> countries = await _authenticationService.GetCountriesAsync();

                Modules = modules.OrderBy(o => o.ModuleId).ToList();

                SelectedModules = new List<Module>
                {
                    Modules.First()
                };

                Countries = countries.OrderBy(o => o.CountryISO).ToList();
            }
            else
                ArePickersAvailable = false;
        }

        private Task SignInAsync() =>
            BaseCommonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();

        public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedCountry))
            {
                if (SelectedCountry is not null)
                {
                    TimeZones = _localizationService.GetTimeZoneIds(SelectedCountry.ISO2Code);

                    if (TimeZones.Count == 1)
                    {
                        SelectedTimeZone = TimeZones[0];
                    }
                }
                else
                {
                    TimeZones = null;
                    SelectedTimeZone = null;
                }
            }
        }

        private async Task RegisterAsync()
        {
            if (Validate())
            {
                string emailaddress;

                // if email starts with "test:" or "local:"
                // remove this prefix and set environment to test.
                if (Mail.StartsWith(GenericConstants.UsernamePrefixTest, StringComparison.InvariantCultureIgnoreCase))
                {
                    emailaddress = Mail.ToLower().Replace(GenericConstants.UsernamePrefixTest, "");
                    Context.Environment = SoftwareEnvironments.Test;
                }
                // remove this prefix and set environment to local.
                else if (Mail.StartsWith(GenericConstants.UsernamePrefixLocal, StringComparison.InvariantCultureIgnoreCase))
                {
                    emailaddress = Mail.ToLower().Replace(GenericConstants.UsernamePrefixLocal, "");
                    Context.Environment = SoftwareEnvironments.Local;
                }
                else
                {
                    emailaddress = Mail;
                    Context.Environment = SoftwareEnvironments.Production;
                }

                if (!HasErrors &&
                    await _authenticationService.CheckRegistrationNameAsync(Name) &&
                    await _authenticationService.CheckRegistrationEmailAsync(emailaddress) &&
                    PasswordCheck is not null && Password is not null &&
                    PasswordCheck.Equals(Password) &&
                    Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                {
                    RegistrationData registrationData = new()
                    {
                        ApplicationId = 1,
                        LicenseName = Name,
                        Email = emailaddress,
                        Password = Password,
                        Modules = Modules,
                        UsersAllowed = 1,
                        CountryCode = SelectedCountry.ISO2Code,
                        TimeZoneId = SelectedTimeZone
                    };

                    if (await _authenticationService.RegisterNewAccountAsync(registrationData))
                    {
                        await BaseCommonServices.DialogService.ShowInformationAsync(BaseCommonServices.LanguageService.GetString("WarningRegistrationConfirmEmail"));
                        await SignInAsync();
                    }
                }
            }
        }
    }
}
