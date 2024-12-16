using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
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
        : base()
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
                logging.AddApplicationInsightsLogging(configuration);
            })
            .ConfigureServices<App, Context, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, ExceptionHandlerService, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();

                services.TryAddSingleton<CommonServices>();
                services.TryAddSingleton<IBaseCommonServices>(s => s.GetRequiredService<CommonServices>());
                services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<CommonServices>());

                services.TryAddSingleton<ICameraService, CameraService>();

                services.AddUpdatesIntegration();
            }, f => f.Name.StartsWith(typeof(App).Namespace));
    }

    public override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            try
            {
                _commonServices.BusyService.BusyMessage = _commonServices.LanguageService.GetString("UpdateCheckForUpdates");

                if (await ServiceLocator.Default.GetService<IUpdateService>().CheckForUpdateAsync() && await _commonServices.DialogService.ShowMessageAsync(
                    _commonServices.LanguageService.GetString("UpdateFoundNewUpdate") + Environment.NewLine + _commonServices.LanguageService.GetString("UpdateExecuteNow"),
                    "Update",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _commonServices.BusyService.BusyMessage = _commonServices.LanguageService.GetString("UpdateDownloadAndInstall");
                    await ServiceLocator.Default.GetService<IUpdateService>().DownloadAndInstallUpdateAsync();
                    Environment.Exit(Environment.ExitCode);
                }
            }
            catch (Exception)
            {
                await _commonServices.DialogService.ShowErrorAsync("Failed to check for updates");
            }

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
            {
                _logger.LogInformation("Navigate to SignIn page");
                await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        finally
        {
            _commonServices.BusyService.EndBusy();
        }
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
