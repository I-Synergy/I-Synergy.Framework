using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Migrations;
using Sample.ViewModels;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Sample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : BaseApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
        : base()
    {
        InitializeComponent();

        MessageService.Default.Register<ApplicationLoadedMessage>(this, async (m) => await ApplicationLoadedAsync(m));
    }

    public override async Task InitializeApplicationAsync()
    {
        await base.InitializeApplicationAsync();

        try
        {
            _commonServices.BusyService.BusyMessage = "Start doing important stuff";
            await Task.Delay(5000);
            _commonServices.BusyService.BusyMessage = "Done doing important stuff";
        }
        catch (Exception)
        {
            await _commonServices.DialogService.ShowErrorAsync("Failed doing important stuff", "Fake error message");
            _commonServices.BusyService.EndBusy();
        }

        try
        {
            if (ServiceLocator.Default.GetInstance<IMigrationService>() is IMigrationService migrationService)
            {
                _commonServices.BusyService.BusyMessage = "Applying migrations";
                await migrationService.ApplyMigrationAsync<_001>();
                _commonServices.BusyService.BusyMessage = "Done applying migrations";
            }
        }
        catch (Exception)
        {
            await _commonServices.DialogService.ShowErrorAsync("Failed to apply migrations", "Fake error message");
            _commonServices.BusyService.EndBusy();
        }

        MessageService.Default.Send(new ApplicationInitializedMessage());
    }

    private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
    {
        try
        {
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

            _commonServices.BusyService.StartBusy();

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
                await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        finally
        {
            _commonServices.BusyService.EndBusy();
        }
    }

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        await _navigationService.CleanBackStackAsync();

        if (e.Value)
        {
            _logger.LogInformation("Navigate to Shell");
            await _navigationService.NavigateModalAsync<IShellViewModel>();
        }
        else
        {
            _logger.LogInformation("Navigate to SignIn page");
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
    }

    public override Task HandleProtocolActivationAsync(string e)
    {
        Debug.WriteLine(e);
        return Task.CompletedTask;
    }

    public override Task HandleLaunchActivationAsync(string e)
    {
        Debug.WriteLine(e);
        return Task.CompletedTask;
    }

    public override Task HandleCommandLineArgumentsAsync(string[] e)
    {
        foreach (var arg in e.EnsureNotNull())
            Debug.WriteLine(arg, "Environment");
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
            MessageService.Default.Unregister<ApplicationLoadedMessage>(this);
    }

    public override void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
        base.CurrentDomain_FirstChanceException(sender, e);
    }
}
