using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.UI.ViewModels;
using Sample.Abstractions;
using Sample.ViewModels;
using System.Runtime.ExceptionServices;

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

        bool navigateToAuthentication = true;

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
            await ServiceLocator.Default.GetInstance<INavigationService>().NavigateModalAsync<AuthenticationViewModel>();
    }

    public override IList<ResourceDictionary> GetAdditionalResourceDictionaries() => new List<ResourceDictionary>()
    {
        new Styles.Colors(),
        new Styles.Style()
    };

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        if (ServiceLocator.Default.GetInstance<INavigationService>() is NavigationService navigationService)
        {
            if (e.Value)
            {
                await navigationService.NavigateModalAsync<AppShellViewModel>();
            }
            else
            {
                await navigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
    }

    protected override void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
        base.CurrentDomain_FirstChanceException(sender, e);
    }
}