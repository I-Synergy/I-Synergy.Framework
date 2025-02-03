using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
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
        : base(ServiceLocator.Default.GetService<ICommonServices>())
    {
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        RaiseApplicationLoaded();
    }

    protected override async void OnApplicationLoaded(object sender, ReturnEventArgs<bool> e)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            _logger.LogTrace("Retrieve default user and check for auto login");

            if (!string.IsNullOrEmpty(_commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.DefaultUser) && _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.IsAutoLogin)
            {
                string username = _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.DefaultUser;
                string password = await _commonServices.ScopedContextService.GetService<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

                if (!string.IsNullOrEmpty(password))
                {
                    await _commonServices.AuthenticationService.AuthenticateWithUsernamePasswordAsync(username, password, _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.IsAutoLogin);
                    navigateToAuthentication = false;
                }
            }

            if (navigateToAuthentication)
            {
                _logger.LogTrace("Navigate to SignIn page");
                await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    protected override async void OnAuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        // Suppress backstack change event during sign out
        await _commonServices.NavigationService.CleanBackStackAsync(suppressEvent: !e.Value);

        if (e.Value)
        {
            _logger.LogTrace("Navigate to Shell");
            await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
        }
        else
        {
            _logger.LogTrace("Navigate to SignIn page");
            await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
    }

    public override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

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

            RaiseApplicationInitialized();
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }
}
