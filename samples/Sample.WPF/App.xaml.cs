using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
using Microsoft.Extensions.Logging;
using Sample.ViewModels;
using System.Windows;

namespace Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : BaseApplication
{
    public App()
        : base(ServiceLocator.Default.GetService<IScopedContextService>())
    {
        MessageService.Default.Register<ApplicationLoadedMessage>(this, async (m) => await ApplicationLoadedAsync(m));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MessageService.Default.Send(new ApplicationLoadedMessage());
    }

    private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            _logger.LogInformation("Retrieve default user and check for auto login");

            var settingsService = _scopedContextService.GetService<ISettingsService>();

            if (!string.IsNullOrEmpty(settingsService.LocalSettings.DefaultUser) && settingsService.LocalSettings.IsAutoLogin)
            {
                string username = settingsService.LocalSettings.DefaultUser;
                string password = await _scopedContextService.GetService<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

                if (!string.IsNullOrEmpty(password))
                {
                    await _authenticationService.AuthenticateWithUsernamePasswordAsync(username, password, settingsService.LocalSettings.IsAutoLogin);
                    navigateToAuthentication = false;
                }
            }

            if (navigateToAuthentication)
            {
                _logger.LogInformation("Navigate to SignIn page");
                await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        // Suppress backstack change event during sign out
        await _commonServices.NavigationService.CleanBackStackAsync(suppressEvent: !e.Value);

        if (e.Value)
        {
            _logger.LogInformation("Navigate to Shell");
            await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
        }
        else
        {
            _logger.LogInformation("Navigate to SignIn page");
            await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
    }

    public override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            await base.InitializeApplicationAsync();

            try
            {
                _commonServices.BusyService.BusyMessage = "Start doing important stuff";
                await Task.Delay(2000);
                _commonServices.BusyService.BusyMessage = "Done doing important stuff";
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _commonServices.DialogService.ShowErrorAsync("Failed doing important stuff", "Fake error message");
            }

            try
            {
                _commonServices.BusyService.BusyMessage = "Applying migrations";
                //await _migrationService.ApplyMigrationAsync<_001>();
                await Task.Delay(2000);
                _commonServices.BusyService.BusyMessage = "Done applying migrations";
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _commonServices.DialogService.ShowErrorAsync("Failed to apply migrations", "Fake error message");
            }

            MessageService.Default.Send(new ApplicationInitializedMessage());
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }
}
