using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
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
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
    }

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        await _navigationService.CleanBackStackAsync();

        if (e.Value)
            await _navigationService.NavigateModalAsync<IShellViewModel>();
        else
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
    }
}
