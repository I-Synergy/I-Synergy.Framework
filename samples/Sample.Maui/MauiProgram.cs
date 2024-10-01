using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions;
using Sample.Abstractions.Services;
using Sample.Extensions;
using Sample.Models;
using Sample.Services;
using Sample.Views;
using Syncfusion.Maui.Core.Hosting;
using System.Diagnostics;
using System.Reflection;




#if WINDOWS
using ISynergy.Framework.Update.Extensions;
#endif

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var mainAssembly = Assembly.GetAssembly(typeof(App));

        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonStream(mainAssembly.GetManifestResourceStream("Sample.appsettings.json"));
        var config = configBuilder.Build();

        builder
            .Configuration
            .AddConfiguration(config);

        var settingsService = new SettingsService<LocalSettings, RoamingSettings, GlobalSettings>();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Font Awesome 6 Pro-Regular-400.otf", "fontawesome");
            })
            .ConfigureLogging((logging, configuration) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                //logging.AddApplicationInsightsLogging(config);
            })
            .ConfigureServices<App, Context, ExceptionHandlerService, Properties.Resources, LoadingView>(appBuilder =>
            {
                appBuilder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                appBuilder.Services.TryAddSingleton<ICredentialLockerService, CredentialLockerService>();
                appBuilder.Services.TryAddSingleton<ISettingsService>(s => settingsService);

                appBuilder.Services.TryAddSingleton<CommonServices>();
                appBuilder.Services.TryAddSingleton<IBaseCommonServices>(s => s.GetRequiredService<CommonServices>());
                appBuilder.Services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<CommonServices>());

                appBuilder.Services.TryAddSingleton<ICameraService, CameraService>();
            })
#if WINDOWS
            //.ConfigureStoreUpdateIntegration()
            .ConfigureSingleInstanceApp(async e =>
            {
                foreach (var arg in e.EnsureNotNull())
                {
                    if (!string.IsNullOrEmpty(arg))
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync(arg, "Environment");
                }
            }, async p =>
            {
                if (!string.IsNullOrEmpty(p.Uri?.Query))
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{p.Uri.Query}", "ProtocolActivatedEventArgs");
            }, async l =>
            {
                if (!string.IsNullOrEmpty(l.Arguments))
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowMessageAsync($"{l.Arguments}", "LaunchActivatedEventArgs");
            })
#endif
            .ConfigureOfflineSynchronization(builder =>
            {
                Debug.WriteLine("Configuring offline synchronization");
                Debug.WriteLine(settingsService.RoamingSettings.IsSynchronizationEnabled);
            })
            .ConfigureSyncfusionCore();

        return builder.Build();
    }
}
