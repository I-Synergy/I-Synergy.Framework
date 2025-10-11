using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Windows;
using ISynergy.Framework.Mvvm.Commands;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Sample.ViewModels;

public class SignInViewModel : ViewModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

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
        ICommonServices commonServices,
        IAuthenticationService authenticationService,
        IDialogService dialogService,
        INavigationService navigationService,
        ILogger<SignInViewModel> logger)
        : base(commonServices, logger)
    {
        _authenticationService = authenticationService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        SignInCommand = new AsyncRelayCommand(SignInAsync);
        SignUpCommand = new AsyncRelayCommand(SignUpAsync);

        Validator = new Action<IObservableValidatedClass>(_ =>
        {
            if (string.IsNullOrEmpty(Username) || (!string.IsNullOrEmpty(Username) && Username.Length <= 3))
                AddValidationError(nameof(Username), LanguageService.Default.GetString("WarningUsernameSize"));

            if (string.IsNullOrEmpty(Password) || (!string.IsNullOrEmpty(Password) && !Regex.IsMatch(Password, GenericConstants.PasswordRegEx, RegexOptions.None, TimeSpan.FromMilliseconds(100))))
                AddValidationError(nameof(Password), LanguageService.Default.GetString("WarningPasswordSize"));
        });

        Usernames = [];
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        if (!IsInitialized)
        {
            AutoLogin = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.IsAutoLogin;
            Usernames = new ObservableCollection<string>();

            if (!string.IsNullOrEmpty(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser))
                Username = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser;
            if (string.IsNullOrEmpty(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser) && Usernames.Count > 0)
                Username = Usernames[0];

            IsInitialized = true;
        }
    }

    private Task SignUpAsync() =>
        _navigationService.NavigateModalAsync<SignUpViewModel>();

    /// <summary>
    /// Forgots the password asynchronous.
    /// </summary>
    /// <returns>Task.</returns>
    public Task ForgotPasswordAsync()
    {
        var forgotPasswordVM = _commonServices.ScopedContextService.GetRequiredService<ForgotPasswordViewModel>();
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

    private async Task SignInAsync()
    {
        _commonServices.BusyService.StartBusy();

        await Task.Delay(5000);

        if (Validate())
            await _authenticationService.AuthenticateWithUsernamePasswordAsync(Username, Password, AutoLogin);

        _commonServices.BusyService.StopBusy();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SignInCommand?.Dispose();
            SignUpCommand?.Dispose();

            base.Dispose(disposing);
        }
    }
}
