using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Utilities;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Sample.ViewModels;

public class AuthenticationViewModel : ViewModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("Login"); } }

    /// <summary>
    /// Gets or sets the Usernames property value.
    /// </summary>
    /// <value>The usernames.</value>
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
    public string? Registration_Mail
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
    public string? Registration_TimeZone
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

    /// <summary>
    /// Gets or sets the AutoLogin property value.
    /// </summary>
    public bool AutoLogin
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public RelayCommand ShowSignInCommand { get; private set; }
    public AsyncRelayCommand SignInCommand { get; private set; }
    public AsyncRelayCommand SignUpCommand { get; private set; }
    public AsyncRelayCommand ForgotPasswordCommand { get; private set; }

    public AuthenticationViewModel(
        ICommonServices commonServices,
        IAuthenticationService authenticationService,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<AuthenticationViewModel> logger)
        : base(commonServices, logger)
    {
        _authenticationService = authenticationService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        ShowSignInCommand = new RelayCommand(SetLoginVisibility);
        SignInCommand = new AsyncRelayCommand(SignInAsync);
        SignUpCommand = new AsyncRelayCommand(SignUpAsync);
        ForgotPasswordCommand = new AsyncRelayCommand(ForgotPasswordAsync);

        Validator = new Action<IObservableValidatedClass>(_ =>
        {
            if (LoginVisible)
            {
                if (string.IsNullOrEmpty(Username) || (Username.Length <= 3))
                {
                    AddValidationError(nameof(Username), LanguageService.Default.GetString("WarningUsernameSize"));
                }

                if (string.IsNullOrEmpty(Password) || !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                {
                    AddValidationError(nameof(Password), LanguageService.Default.GetString("WarningPasswordSize"));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Registration_Name) || (Registration_Name.Length <= 3))
                {
                    AddValidationError(nameof(Registration_Name), LanguageService.Default.GetString("WarningLicenseNameSize"));
                }

                if (string.IsNullOrEmpty(Registration_Mail) || !NetworkUtility.IsValidEMail(Registration_Mail))
                {
                    AddValidationError(nameof(Registration_Mail), LanguageService.Default.GetString("WarningInvalidEmail"));
                }

                if (string.IsNullOrEmpty(Registration_TimeZone))
                {
                    AddValidationError(nameof(Registration_TimeZone), LanguageService.Default.GetString("WarningNoTimeZoneSelected"));
                }

                if (string.IsNullOrEmpty(Registration_Password) || (Registration_Password.Length <= 6))
                {
                    AddValidationError(nameof(Registration_Password), LanguageService.Default.GetString("WarningPasswordSize"));
                }

                if (string.IsNullOrEmpty(Registration_Password) || !Regex.IsMatch(Registration_Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                {
                    AddValidationError(nameof(Registration_Password), LanguageService.Default.GetString("WarningPasswordSize"));
                }

                if (string.IsNullOrEmpty(Registration_PasswordCheck) || (Registration_PasswordCheck.Length <= 6))
                {
                    AddValidationError(nameof(Registration_PasswordCheck), LanguageService.Default.GetString("WarningPasswordSize"));
                }

                if (!string.IsNullOrEmpty(Registration_Password) && !string.IsNullOrEmpty(Registration_PasswordCheck) && !Registration_Password.Equals(Registration_PasswordCheck))
                {
                    AddValidationError(nameof(Registration_Password), LanguageService.Default.GetString("WarningPasswordMatch"));
                    AddValidationError(nameof(Registration_PasswordCheck), LanguageService.Default.GetString("WarningPasswordMatch"));
                }

                if (Registration_Modules.Count < 1)
                {
                    AddValidationError(nameof(Registration_Modules), LanguageService.Default.GetString("WarningNoModulesSelected"));
                }
            }
        });

        Usernames = [];
        Registration_TimeZone = null;
        Registration_Modules = [];
        LoginVisible = true;
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (!IsInitialized)
        {
            Modules = new List<Module>();

            if (Modules.FirstOrDefault() is { } module)
                Registration_Modules.Add(module);

            AutoLogin = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.IsAutoLogin;

            var users = await _commonServices.ScopedContextService.GetService<ICredentialLockerService>().GetUsernamesFromCredentialLockerAsync();
            Usernames = new ObservableCollection<string>();
            Usernames.AddRange(users);

            if (!string.IsNullOrEmpty(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser))
                Username = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser;

            if (string.IsNullOrEmpty(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser) && Usernames.Count > 0)
                Username = Usernames[0];

            IsInitialized = true;
        }
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
        ForgotPasswordViewModel forgotPasswordVM = _commonServices.ScopedContextService.GetRequiredService<ForgotPasswordViewModel>();
        forgotPasswordVM.Submitted += ForgotPasswordVM_Submitted;
        return _dialogService.ShowDialogAsync(typeof(IForgotPasswordWindow), forgotPasswordVM);
    }

    /// <summary>
    /// Forgots the password vm submitted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void ForgotPasswordVM_Submitted(object? sender, SubmitEventArgs<bool> e)
    {
        if (sender is ForgotPasswordViewModel vm)
            vm.Submitted -= ForgotPasswordVM_Submitted;

        if (e.Result)
        {
            await _dialogService
                    .ShowInformationAsync(LanguageService.Default.GetString("Warning_Reset_Password"));

            if (_navigationService.CanGoBack)
                await _navigationService.GoBackAsync();
        }
    }

    public override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Registration_Country))
        {
            if (Registration_Country is not null)
            {
                TimeZones = Registration_Country.ISO2Code!.ToTimeZoneIds();

                if (TimeZones.Count == 1)
                    Registration_TimeZone = TimeZones[0];
            }
            else
            {
                TimeZones = new List<string>();
                Registration_TimeZone = null;
            }
        }
    }

    private Task SignUpAsync()
    {
        if (Validate())
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private async Task SignInAsync()
    {
        _commonServices.BusyService.StartBusy();

        await Task.Delay(1000);

        if (Validate())
            await _authenticationService.AuthenticateWithUsernamePasswordAsync(Username, Password, AutoLogin);

        _commonServices.BusyService.StopBusy();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ShowSignInCommand?.Dispose();
            SignInCommand?.Dispose();
            SignUpCommand?.Dispose();
            ForgotPasswordCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
