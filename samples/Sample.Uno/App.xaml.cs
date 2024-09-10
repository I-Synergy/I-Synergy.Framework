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



#if WINDOWS
using ISynergy.Framework.Update.Extensions;
#endif

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
        : base(() => ServiceLocator.Default.GetInstance<ILoadingView>())
    {
        InitializeComponent();

        MessageService.Default.Register<ApplicationLoadedMessage>(this, async (m) => await ApplicationLoadedAsync(m));
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        var hostbuilder = new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                Assembly mainAssembly = Assembly.GetAssembly(typeof(App));
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
            })
            .UseLogging(configure: (context, logBuilder) =>
            {
                // Configure log levels for different categories of logging
                logBuilder
                    .SetMinimumLevel(
                        context.HostingEnvironment.IsDevelopment() ?
                            LogLevel.Information :
                            LogLevel.Warning)

                    // Default filters for core Uno Platform namespaces
                    .CoreLogLevel(LogLevel.Warning);

                // Uno Platform namespace filter groups
                // Uncomment individual methods to see more detailed logging
                // Generic Xaml events
                logBuilder.XamlLogLevel(LogLevel.Debug);
                // Layout specific messages
                logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                // Storage messages
                logBuilder.StorageLogLevel(LogLevel.Debug);
                // Binding related messages
                logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                // Binder memory references tracking
                logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                // DevServer and HotReload related
                logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                // Debug JS interop
                logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

            }, enableUnoLogging: true)
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources>((services, configuration) =>
            {
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseSettingsService, SettingsService>());

                services.TryAddSingleton<CommonServices>();
                services.TryAddSingleton<IBaseCommonServices>(s => s.GetRequiredService<CommonServices>());
                services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<CommonServices>());
            }, f => f.Name.StartsWith(typeof(App).Namespace));
#if WINDOWS
        hostbuilder.ConfigureStoreUpdateIntegration();
#endif

        return hostbuilder;
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

            if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser) && _settingsService.LocalSettings.IsAutoLogin)
            {
                string username = _settingsService.LocalSettings.DefaultUser;
                string password = await ServiceLocator.Default.GetInstance<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

                if (!string.IsNullOrEmpty(password))
                {
                    await _authenticationService.AuthenticateWithUsernamePasswordAsync(username, password, _settingsService.LocalSettings.IsAutoLogin);
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
