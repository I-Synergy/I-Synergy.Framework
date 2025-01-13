using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.Update.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Globalization;
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
        : base(() => ServiceLocator.Default.GetService<ILoadingView>())
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
            .ConfigureServices<App, Context, CommonServices, AuthenticationService, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<ICameraService, CameraService>();

                services.AddUpdatesIntegration();
            }, f => f.Name.StartsWith(typeof(App).Namespace));
    }

    public override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            //try
            //{
            //    _commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateCheckForUpdates");

            //    if (await ServiceLocator.Default.GetService<IUpdateService>().CheckForUpdateAsync() && await _commonServices.DialogService.ShowMessageAsync(
            //        LanguageService.Default.GetString("UpdateFoundNewUpdate") + Environment.NewLine + LanguageService.Default.GetString("UpdateExecuteNow"),
            //        "Update",
            //        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //    {
            //        _commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateDownloadAndInstall");
            //        await ServiceLocator.Default.GetService<IUpdateService>().DownloadAndInstallUpdateAsync();
            //        Environment.Exit(Environment.ExitCode);
            //    }
            //}
            //catch (Exception)
            //{
            //    await _commonServices.DialogService.ShowErrorAsync("Failed to check for updates");
            //}

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

    private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
    {
        try
        {
#if WINDOWS
            //commonServices.BusyService.StartBusy(LanguageService.Default.GetString("UpdateCheckForUpdates"));

            //if (await ServiceLocator.Default.GetInstance<IUpdateService>().CheckForUpdateAsync() && await commonServices.DialogService.ShowMessageAsync(
            //    LanguageService.Default.GetString("UpdateFoundNewUpdate") + System.Environment.NewLine + LanguageService.Default.GetString("UpdateExecuteNow"),
            //    "Update",
            //    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateDownloadAndInstall");
            //    await ServiceLocator.Default.GetInstance<IUpdateService>().DownloadAndInstallUpdateAsync();
            //    Environment.Exit(Environment.ExitCode);
            //}
#endif
            _commonServices.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            _logger.LogInformation("Retrieve default user and check for auto login");

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
            _logger.LogInformation("Saving refresh token");

            _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.RefreshToken = _commonServices.ScopedContextService.GetService<IContext>().ToEnvironmentalRefreshToken();
            _commonServices.ScopedContextService.GetService<ISettingsService>().SaveLocalSettings();

            _logger.LogInformation("Setting culture");
            if (_commonServices.ScopedContextService.GetService<ISettingsService>().GlobalSettings is not null)
            {
                var culture = CultureInfo.DefaultThreadCurrentCulture.Clone() as CultureInfo;

                culture.NumberFormat.CurrencySymbol = "€";

                culture.NumberFormat.CurrencyDecimalDigits = _commonServices.ScopedContextService.GetService<ISettingsService>().GlobalSettings.Decimals;
                culture.NumberFormat.NumberDecimalDigits = _commonServices.ScopedContextService.GetService<ISettingsService>().GlobalSettings.Decimals;

                culture.NumberFormat.CurrencyNegativePattern = 1;
                culture.NumberFormat.NumberNegativePattern = 1;
                culture.NumberFormat.PercentNegativePattern = 1;

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            _logger.LogInformation("Navigate to Shell");
            await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
        }
        else
        {
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
