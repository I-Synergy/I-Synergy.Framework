using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
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
            await _navigationService.NavigateModalAsync<SignInViewModel>(absolute: true);
    }

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        if (e.Value)
            await _navigationService.NavigateModalAsync<IShellViewModel>(absolute: true);
        else
            await _navigationService.NavigateModalAsync<SignInViewModel>(absolute: true);
    }

    public override IList<ResourceDictionary> GetAdditionalResourceDictionaries() => new List<ResourceDictionary>
    {
        new Sample.Resources.Styles.Colors(),
        new Sample.Resources.Styles.Style(),
        new Sample.Resources.Styles.Images()
    };

    protected override void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
        base.CurrentDomain_FirstChanceException(sender, e);
    }
}
