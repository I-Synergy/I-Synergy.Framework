using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI;
using ISynergy.Framework.Update.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Services;
using Sample.ViewModels;

namespace Sample;

public partial class App : BaseApplication
{
    public App()
        : base()
    {
        InitializeComponent();
    }

    public override async Task InitializeApplicationAsync()
    {
        await base.InitializeApplicationAsync();

        // Delay to show loading screen on startup.
        await Task.Delay(5000);

        if (ServiceLocator.Default.GetInstance<ICommonServices>() is CommonServices commonServices)
        {
            try
            {
#if WINDOWS
                //commonServices.BusyService.StartBusy(commonServices.LanguageService.GetString("UpdateCheckForUpdates"));

                //if (await ServiceLocator.Default.GetInstance<IUpdateService>().CheckForUpdateAsync() && await commonServices.DialogService.ShowMessageAsync(
                //    commonServices.LanguageService.GetString("UpdateFoundNewUpdate") + System.Environment.NewLine + commonServices.LanguageService.GetString("UpdateExecuteNow"),
                //    "Update",
                //    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                //{
                //    commonServices.BusyService.BusyMessage = commonServices.LanguageService.GetString("UpdateDownloadAndInstall");
                //    await ServiceLocator.Default.GetInstance<IUpdateService>().DownloadAndInstallUpdateAsync();
                //    Environment.Exit(Environment.ExitCode);
                //}
#endif
                commonServices.BusyService.StartBusy();

                bool navigateToAuthentication = true;

                _logger.LogInformation("Retrieve default user and check for auto login");

                if (!string.IsNullOrEmpty(_applicationSettingsService.Settings.DefaultUser) && _applicationSettingsService.Settings.IsAutoLogin)
                {
                    string username = _applicationSettingsService.Settings.DefaultUser;
                    string password = await ServiceLocator.Default.GetInstance<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

                    if (!string.IsNullOrEmpty(password))
                    {
                        await _authenticationService.AuthenticateWithUsernamePasswordAsync(username, password, _applicationSettingsService.Settings.IsAutoLogin);
                        navigateToAuthentication = false;
                    }
                }

                if (navigateToAuthentication)
                {
                    _logger.LogInformation("Navigate to SignIn page");
                    await _navigationService.NavigateModalAsync<SignInViewModel>(absolute: true);
                }
            }
            finally
            {
                commonServices.BusyService.EndBusy();
            }
        }
    }

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        if (e.Value)
        {
            _logger.LogInformation("Navigate to Shell");
            await _navigationService.NavigateModalAsync<IShellViewModel>(absolute: true);
        }
        else
        {
            _logger.LogInformation("Navigate to SignIn page");
            await _navigationService.NavigateModalAsync<SignInViewModel>(absolute: true);
        }
    }
}
