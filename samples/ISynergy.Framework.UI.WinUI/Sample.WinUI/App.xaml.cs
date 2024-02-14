using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.Update.Abstractions.Services;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sample.Abstractions;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Reflection;
using Windows.ApplicationModel.Activation;

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
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                Assembly mainAssembly = Assembly.GetAssembly(typeof(App));
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>(context.Configuration, x => x.Name.StartsWith(typeof(App).Namespace));

                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, LocalSettingsService>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<GlobalSettings>, GlobalSettingsService>());

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

                services.AddUpdatesIntegration();
            })
            .ConfigureLogging((context, logging) =>
            {
                //logging.AddAppCenterLogging(context.Configuration);
                //logging.AddSentryLogging(context.Configuration);
            });
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
    {
        base.OnLaunched(e);

        try
        {
            ServiceLocator.Default.GetInstance<IBusyService>().StartBusy(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateCheckForUpdates"));

            if (await ServiceLocator.Default.GetInstance<IUpdateService>().CheckForUpdateAsync() && await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync(
                ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateFoundNewUpdate") + System.Environment.NewLine + ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateExecuteNow"),
                "Update",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ServiceLocator.Default.GetInstance<IBusyService>().BusyMessage = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateDownloadAndInstall");
                await ServiceLocator.Default.GetInstance<IUpdateService>().DownloadAndInstallUpdateAsync();
                Exit();
            }
        }
        finally
        {
            ServiceLocator.Default.GetInstance<IBusyService>().EndBusy();
        }

        //var activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        //if (activatedEventArgs.Kind == ExtendedActivationKind.Launch)
        //{
        //    await HandleLaunchActivationAsync(activatedEventArgs.Data as Windows.ApplicationModel.Activation.LaunchActivatedEventArgs);
        //}
        //else if (activatedEventArgs.Kind == ExtendedActivationKind.Protocol)
        //{
        //    await HandleProtocolActivationAsync(activatedEventArgs.Data as ProtocolActivatedEventArgs);
        //}
        //else if (Environment.GetCommandLineArgs().Length > 1)
        //{
        //    foreach (var arg in Environment.GetCommandLineArgs())
        //        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync(arg, "Environment");
        //}
        //else
        //{
        //    await ServiceLocator.Default.GetInstance<INavigationService>().NavigateModalAsync<AuthenticationViewModel>();
        //}

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

    public override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        if (ServiceLocator.Default.GetInstance<INavigationService>() is NavigationService navigationService)
        {
            await navigationService.CleanBackStackAsync();

            if (e.Value)
                await navigationService.NavigateModalAsync<IShellViewModel>();
            else
                await navigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
    }

    private async Task HandleProtocolActivationAsync(ProtocolActivatedEventArgs e)
    {
        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{e.Uri}", "ProtocolActivatedEventArgs");
    }

    private async Task HandleLaunchActivationAsync(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs e)
    {
        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{e.Arguments}", "LaunchActivatedEventArgs");
    }
}
