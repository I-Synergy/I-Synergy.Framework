using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Sample.ViewModels;

public class SignUpViewModel : ViewModel
{
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

    public AsyncRelayCommand SignUpCommand { get; private set; }
    public AsyncRelayCommand ValidateMailCommand { get; private set; }
    public AsyncRelayCommand SignInCommand { get; private set; }
    public AsyncRelayCommand SelectModulesCommand { get; private set; }

    public SignUpViewModel(
        IScopedContextService scopedContextService,
        ICommonServices commonServices,
        ILogger logger,
        bool automaticValidation = false)
        : base(scopedContextService, commonServices, logger, automaticValidation)
    {
        this.Validator = new Action<IObservableClass>(_ =>
        {
            if (string.IsNullOrEmpty(Name) || (Name.Length <= 3))
            {
                AddValidationError(nameof(Name), LanguageService.Default.GetString("WarningLicenseNameSize"));
            }

            if (string.IsNullOrEmpty(Mail) || !NetworkUtility.IsValidEMail(Mail))
            {
                AddValidationError(nameof(Mail), LanguageService.Default.GetString("WarningInvalidEmail"));
            }

            if (string.IsNullOrEmpty(SelectedTimeZone))
            {
                AddValidationError(nameof(SelectedTimeZone), LanguageService.Default.GetString("WarningNoTimeZoneSelected"));
            }

            if (string.IsNullOrEmpty(Password) || (Password.Length <= 6))
            {
                AddValidationError(nameof(Password), LanguageService.Default.GetString("WarningPasswordSize"));
            }

            if (string.IsNullOrEmpty(Password) || !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
            {
                AddValidationError(nameof(Password), LanguageService.Default.GetString("WarningPasswordSize"));
            }

            if (string.IsNullOrEmpty(PasswordCheck) || (PasswordCheck.Length <= 6))
            {
                AddValidationError(nameof(PasswordCheck), LanguageService.Default.GetString("WarningPasswordSize"));
            }

            if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(PasswordCheck) && !Password.Equals(PasswordCheck))
            {
                AddValidationError(nameof(Password), LanguageService.Default.GetString("WarningPasswordMatch"));
                AddValidationError(nameof(PasswordCheck), LanguageService.Default.GetString("WarningPasswordMatch"));
            }

            if (SelectedModules.Count < 1)
            {
                AddValidationError(nameof(SelectedModules), LanguageService.Default.GetString("WarningNoModulesSelected"));
            }

            if (SelectedCountry is null)
                AddValidationError(nameof(SelectedCountry), LanguageService.Default.GetString("WarningNoCountrySelected"));
        });

        SignUpCommand = new AsyncRelayCommand(SignUpAsync);
        ValidateMailCommand = new AsyncRelayCommand(ValidateMailAsync);
        SignInCommand = new AsyncRelayCommand(SignInAsync);
        SelectModulesCommand = new AsyncRelayCommand(SelectModulesAsync);

        ArePickersAvailable = false;
    }

    private Task SelectModulesAsync()
    {
        ViewModelSelectionDialog<Module> selectionVM = new ViewModelSelectionDialog<Module>(_scopedContextService, _commonServices, _logger, Modules, SelectedModules, SelectionModes.Multiple);
        selectionVM.Submitted += SelectionVM_Submitted;
        return _commonServices.DialogService.ShowDialogAsync(typeof(ISelectionWindow), selectionVM);
    }

    /// <summary>
    /// Editors the vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void SelectionVM_Submitted(object sender, SubmitEventArgs<List<Module>> e)
    {
        if (sender is ViewModelSelectionDialog<Module> vm)
            vm.Submitted -= SelectionVM_Submitted;

        List<Module> selectedItems = [];

        foreach (object item in e.Result.EnsureNotNull())
        {
            if (item is Module module)
                selectedItems.Add(module);
        }

        SelectedModules = selectedItems;
    }

    private async Task ValidateMailAsync()
    {
        var context = _scopedContextService.GetService<IContext>();

        if (!string.IsNullOrEmpty(Mail) && NetworkUtility.IsValidEMail(Mail.GetNormalizedCredentials(context)))
        {
            ArePickersAvailable = true;

            List<Module> modules = await _commonServices.AuthenticationService.GetModulesAsync();
            List<Country> countries = await _commonServices.AuthenticationService.GetCountriesAsync();

            Modules = modules.OrderBy(o => o.ModuleId).ToList();

            SelectedModules =
            [
                Modules[0]
            ];

            Countries = countries.OrderBy(o => o.CountryISO).ToList();
        }
        else
            ArePickersAvailable = false;
    }

    private Task SignInAsync() =>
        _commonServices.NavigationService.NavigateModalAsync<SignInViewModel>();

    public override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedCountry))
        {
            if (SelectedCountry is not null)
            {
                TimeZones = SelectedCountry.ISO2Code.ToTimeZoneIds();

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

    private async Task SignUpAsync()
    {
        if (Validate())
        {
            var context = _scopedContextService.GetService<IContext>();
            string emailaddress;

            // if email starts with "test:" or "local:"
            // remove this prefix and set environment to test.
            if (Mail.StartsWith(GenericConstants.UsernamePrefixTest, StringComparison.InvariantCultureIgnoreCase))
            {
                emailaddress = Mail.ToLower().Replace(GenericConstants.UsernamePrefixTest, "");
                context.Environment = SoftwareEnvironments.Test;
            }
            // remove this prefix and set environment to local.
            else if (Mail.StartsWith(GenericConstants.UsernamePrefixLocal, StringComparison.InvariantCultureIgnoreCase))
            {
                emailaddress = Mail.ToLower().Replace(GenericConstants.UsernamePrefixLocal, "");
                context.Environment = SoftwareEnvironments.Local;
            }
            else
            {
                emailaddress = Mail;
                context.Environment = SoftwareEnvironments.Production;
            }

            if (!HasErrors &&
                await _commonServices.AuthenticationService.CheckRegistrationEmailAsync(emailaddress) &&
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

                if (await _commonServices.AuthenticationService.RegisterNewAccountAsync(registrationData))
                {
                    await _commonServices.DialogService.ShowInformationAsync(LanguageService.Default.GetString("WarningRegistrationConfirmEmail"));
                    await SignInAsync();
                }
            }
        }
    }
}
