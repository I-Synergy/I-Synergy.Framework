using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Models.Accounts;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.ViewModels;

public class AuthenticationViewModel : ViewModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IBaseApplicationSettingsService _applicationSettingsService;

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
    /// Gets or sets the AutoLogin property value.
    /// </summary>
    public bool AutoLogin
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public AsyncRelayCommand LoginCommand { get; private set; }
    public AsyncRelayCommand RegisterCommand { get; private set; }

    public AuthenticationViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        IAuthenticationService authenticationService,
        IBaseApplicationSettingsService applicationSettingsService,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        _authenticationService = authenticationService;

        _applicationSettingsService = applicationSettingsService;
        _applicationSettingsService.LoadSettings();

        LoginCommand = new AsyncRelayCommand(SignInAsync);
        RegisterCommand = new AsyncRelayCommand(SignUpAsync);

        Validator = new Action<IObservableClass>(_ =>
        {
            if (string.IsNullOrEmpty(Username) || (!string.IsNullOrEmpty(Username) && Username.Length <= 3))
                AddValidationError(nameof(Username), BaseCommonServices.LanguageService.GetString("WarningUsernameSize"));

            if (string.IsNullOrEmpty(Password) || (!string.IsNullOrEmpty(Password) && !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100))))
                AddValidationError(nameof(Password), BaseCommonServices.LanguageService.GetString("WarningPasswordSize"));
        });
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        AutoLogin = _applicationSettingsService.Settings.IsAutoLogin;

        if (!string.IsNullOrEmpty(_applicationSettingsService.Settings.DefaultUser))
            Username = _applicationSettingsService.Settings.DefaultUser;
    }

    private Task SignUpAsync() =>
        BaseCommonServices.NavigationService.NavigateModalAsync<RegistrationViewModel>();

    private async Task SignInAsync()
    {
        if (Validate())
        {
            await _authenticationService.AuthenticateWithUsernamePasswordAsync(Username, Password, AutoLogin);

            if (Context.Profile is not null && Context.IsAuthenticated)
            {
                // Save Username in preferences if authentication is successfull.
                if (Preferences.ContainsKey(nameof(Profile.Username)))
                    Preferences.Remove(nameof(Profile.Username));

                Preferences.Set(nameof(Profile.Username), Username);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        LoginCommand?.Cancel();
        LoginCommand = null;
        RegisterCommand?.Cancel();
        RegisterCommand = null;

        base.Dispose(disposing);
    }
}
