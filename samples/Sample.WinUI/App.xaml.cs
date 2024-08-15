using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Reflection;
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
        : base() //(() => ServiceLocator.Default.GetInstance<ILoadingView>())
    {
        InitializeComponent();

        MessageService.Default.Register<ApplicationLoadedMessage>(this, async (m) => await ApplicationLoadedAsync(m));
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                Assembly mainAssembly = Assembly.GetAssembly(typeof(App));
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
            })
            .ConfigureLogging((logging, configuration) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IApplicationSettingsService, LocalSettingsService>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService<GlobalSettings>, GlobalSettingsService>());

                services.TryAddSingleton<CommonServices>();
                services.TryAddSingleton<IBaseCommonServices>(s => s.GetRequiredService<CommonServices>());
                services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<CommonServices>());

                services.AddUpdatesIntegration();
            });
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
            _commonServices.BusyService.EndBusy();
        }
    }

    private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
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
