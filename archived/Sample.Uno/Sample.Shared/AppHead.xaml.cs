using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Abstractions.Services;
using Sample.Services;
using Sample.ViewModels;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using HostBuilder = Microsoft.Extensions.Hosting.HostBuilder;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

namespace Sample;

public sealed partial class AppHead : BaseApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public AppHead()
        : base()
    {
        InitializeComponent();

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = ServiceLocator.Default.GetInstance<ILoggerFactory>();

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
    }


    protected override IHostBuilder CreateHostBuilder()
    {
        return new HostBuilder()
            .ConfigureHostConfiguration(builder =>
            {
                Assembly mainAssembly = Assembly.GetAssembly(typeof(Context));
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json"));
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureServices<AppHead, Context, ExceptionHandlerService, Sample.Properties.Resources>(context.Configuration, x => x.Name.StartsWith(typeof(AppHead).Namespace));

                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseApplicationSettingsService, LocalSettingsService>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IGlobalSettingsService, GlobalSettingsService>());

                services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

                //services.AddUpdatesIntegration();
            })
            .ConfigureLogging((context, logging) =>
            {
                //logging.AddAppCenterLogging(context.Configuration);
                //logging.AddSentryLogging(context.Configuration);

#if __WASM__
                logging.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
                logging.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                logging.AddDebug();
#else
                logging.AddConsole();
#endif

                // Exclude logs below this level
                logging.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                logging.AddFilter("Uno", LogLevel.Warning);
                logging.AddFilter("Windows", LogLevel.Warning);
                logging.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // DevServer and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
    {
        base.OnLaunched(e);

        //try
        //{
        //    ServiceLocator.Default.GetInstance<IBusyService>().StartBusy(ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateCheckForUpdates"));

        //    if (await ServiceLocator.Default.GetInstance<IUpdateService>().CheckForUpdateAsync() && await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync(
        //        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateFoundNewUpdate") + System.Environment.NewLine + ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateExecuteNow"),
        //        "Update",
        //        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //    {
        //        ServiceLocator.Default.GetInstance<IBusyService>().BusyMessage = ServiceLocator.Default.GetInstance<ILanguageService>().GetString("UpdateDownloadAndInstall");
        //        await ServiceLocator.Default.GetInstance<IUpdateService>().DownloadAndInstallUpdateAsync();
        //        Exit();
        //    }
        //}
        //finally
        //{
        //    ServiceLocator.Default.GetInstance<IBusyService>().EndBusy();
        //}

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

    protected override async void AuthenticationChanged(object sender, ReturnEventArgs<bool> e)
    {
        if (ServiceLocator.Default.GetInstance<INavigationService>() is NavigationService navigationService)
        {
            await navigationService.CleanBackStackAsync();

            if (e.Value)
            {
                await navigationService.NavigateModalAsync<ShellViewModel>();
            }
            else
            {
                await navigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
    }

    private async Task HandleProtocolActivationAsync(ProtocolActivatedEventArgs e)
    {
        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{e.Uri}", "ProtocolActivatedEventArgs");
    }

    private async Task HandleLaunchActivationAsync(LaunchActivatedEventArgs e)
    {
        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{e.Arguments}", "LaunchActivatedEventArgs");
    }

    protected override void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
        base.CurrentDomain_FirstChanceException(sender, e);
    }
}