using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Attributes;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Sample.ViewModels;

[Lifetime(Lifetimes.Singleton)]
public class SignInViewModel : ViewModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IBaseSettingsService _settingsService;
    private readonly ICredentialLockerService _credentialLockerService;

    public override string Title { get { return BaseCommonServices.LanguageService.GetString("Login"); } }

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
    /// Gets or sets the AutoLogin property value.
    /// </summary>
    public bool AutoLogin
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public AsyncRelayCommand SignInCommand { get; private set; }
    public AsyncRelayCommand SignUpCommand { get; private set; }

    public SignInViewModel(
        IContext context,
        IBaseCommonServices commonServices,
        IAuthenticationService authenticationService,
        IBaseSettingsService settingsService,
        ICredentialLockerService credentialLockerService,
        ILogger logger,
        bool automaticValidation = false)
        : base(context, commonServices, logger, automaticValidation)
    {
        _authenticationService = authenticationService;
        _credentialLockerService = credentialLockerService;
        _settingsService = settingsService;

        SignInCommand = new AsyncRelayCommand(SignInAsync);
        SignUpCommand = new AsyncRelayCommand(SignUpAsync);

        Validator = new Action<IObservableClass>(_ =>
        {
            if (string.IsNullOrEmpty(Username) || (!string.IsNullOrEmpty(Username) && Username.Length <= 3))
                AddValidationError(nameof(Username), BaseCommonServices.LanguageService.GetString("WarningUsernameSize"));

            if (string.IsNullOrEmpty(Password) || (!string.IsNullOrEmpty(Password) && !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100))))
                AddValidationError(nameof(Password), BaseCommonServices.LanguageService.GetString("WarningPasswordSize"));
        });

        Usernames = [];
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (!IsInitialized)
        {
            AutoLogin = _settingsService.LocalSettings.IsAutoLogin;
            var users = await _credentialLockerService.GetUsernamesFromCredentialLockerAsync();
            Usernames = new ObservableCollection<string>();
            Usernames.AddRange(users);

            if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser))
                Username = _settingsService.LocalSettings.DefaultUser;
            if (string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser) && Usernames.Count > 0)
                Username = Usernames[0];

            IsInitialized = true;
        }
    }

    private Task SignUpAsync() =>
        BaseCommonServices.NavigationService.NavigateModalAsync<SignUpViewModel>();

    /// <summary>
    /// Forgots the password asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public Task ForgotPasswordAsync()
    {
        ForgotPasswordViewModel forgotPasswordVM = new ForgotPasswordViewModel(Context, BaseCommonServices, _authenticationService, Logger);
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

            if (BaseCommonServices.NavigationService.CanGoBack)
                await BaseCommonServices.NavigationService.GoBackAsync();
        }
    }

    private async Task SignInAsync()
    {
        if (Validate())
            await _authenticationService.AuthenticateWithUsernamePasswordAsync(Username, Password, AutoLogin);
    }

    protected override void Dispose(bool disposing)
    {
        SignInCommand?.Cancel();
        SignInCommand = null;
        SignUpCommand?.Cancel();
        SignUpCommand = null;

        base.Dispose(disposing);
    }
}
