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
        : base(ServiceLocator.Default.GetService<IScopedContextService>())
    {
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        bool navigateToAuthentication = true;

        if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser) && _settingsService.LocalSettings.IsAutoLogin)
        {
            string username = _settingsService.LocalSettings.DefaultUser;
            string password = await _scopedContextService.GetService<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

            if (!string.IsNullOrEmpty(password))
            {
                await _authenticationService.AuthenticateWithUsernamePasswordAsync(username, password, _settingsService.LocalSettings.IsAutoLogin);
                navigateToAuthentication = false;
            }
        }

        if (navigateToAuthentication)
            await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
    }

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        // Suppress backstack change event during sign out
        await _commonServices.NavigationService.CleanBackStackAsync(suppressEvent: !e.Value);

        if (e.Value)
        {
            _logger.LogInformation("Set application title by environment");
            _commonServices.InfoService.SetTitle(_context.Environment);

            _logger.LogInformation("Navigate to Shell");
            await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
        }
        else
        {
            _logger.LogInformation("Clear application title");
            _commonServices.InfoService.SetTitle(default);

            _logger.LogInformation("Navigate to SignIn page");
            await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
    }
}
