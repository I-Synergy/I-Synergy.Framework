using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Logging.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
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
using OpenTelemetry.Logs;
using Sample.Models;
using Sample.Processors;
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
        : base()
    //: base(() => ServiceLocator.Default.GetService<ILoadingView>(), new LoadingViewOptions
    //{
    //    AssetStreamProvider = () => Task.FromResult(
    //        Assembly.GetAssembly(typeof(App))?.GetManifestResourceStream("Sample.Assets.gta.mp4")),
    //    ContentType = "video/mp4",
    //    ViewType = LoadingViewTypes.Video
    //})
    {
        InitializeComponent();
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        var mainAssembly = Assembly.GetAssembly(typeof(App));
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        return new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
            })
            .ConfigureLogging((logging, configuration) =>
            {
                logging.AddOpenTelemetryLogging(configuration);
            })
            .ConfigureServices<App, Context, CommonServices, AuthenticationService, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>((services, configuration) =>
            {
                services.TryAddScoped<TenantProcessor>();

                services.TryAddSingleton<ICameraService, CameraService>();

                services.AddUpdatesIntegration();
            }, f => f.Name.StartsWith(typeof(App).Namespace))
            .ConfigureOpenTelemetryLogging(
                infoService,
                tracing => { },
                metrics => { },
                logging =>
                {
                    logging.AddProcessor(s =>
                    {
                        var scopedContextService = s.GetRequiredService<IScopedContextService>();
                        return scopedContextService.GetService<TenantProcessor>();
                    });
                });
    }

    protected override async void OnApplicationInitialized(object sender, ReturnEventArgs<bool> e)
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

    public override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            try
            {
                _commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateCheckForUpdates");

                var updateService = ServiceLocator.Default.GetService<IUpdateService>();

                if (await updateService?.CheckForUpdateAsync() &&
                    await _commonServices.DialogService.ShowMessageAsync(
                    LanguageService.Default.GetString("UpdateFoundNewUpdate") + Environment.NewLine + LanguageService.Default.GetString("UpdateExecuteNow"),
                    "Update",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateDownloadAndInstall");
                    await updateService?.DownloadAndInstallUpdateAsync();
                    Environment.Exit(Environment.ExitCode);
                }
            }
            catch (Exception)
            {
                await _commonServices.DialogService.ShowErrorAsync("Failed to check for updates");
            }

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

        try
        {
            _commonServices.BusyService.StartBusy();

            if (e.Value)
            {
                _logger.LogTrace("Saving refresh token");

                _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.RefreshToken = _commonServices.ScopedContextService.GetService<IContext>().ToEnvironmentalRefreshToken();
                _commonServices.ScopedContextService.GetService<ISettingsService>().SaveLocalSettings();

                _logger.LogTrace("Setting culture");
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

                _logger.LogTrace("Navigate to Shell");
                await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
            }
            else
            {
                _logger.LogTrace("Navigate to SignIn page");
                await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        catch (Exception)
        {
            // Do nothing!
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    public override void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
        base.CurrentDomain_FirstChanceException(sender, e);
    }
}
