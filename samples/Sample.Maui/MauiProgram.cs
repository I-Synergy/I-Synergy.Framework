using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Extensions;
using Sample.Services;
using Syncfusion.Maui.Core.Hosting;
using System.Diagnostics;
using System.Reflection;

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var mainAssembly = Assembly.GetAssembly(typeof(App)) ?? throw new InvalidOperationException("Unable to load main assembly");
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        var configBuilder = new ConfigurationBuilder();
        var stream = mainAssembly.GetManifestResourceStream("Sample.appsettings.json") ?? throw new InvalidOperationException("Unable to load appsettings resource");
        configBuilder.AddJsonStream(stream);
        var config = configBuilder.Build();

        builder
            .Configuration
            .AddConfiguration(config);

        builder
            .ConfigureLogging(
                infoService,
                tracerProviderBuilderAction: (tracing) =>
                {
                    // Additional custom tracer provider configuration can be added here
                },
                meterProviderBuilderAction: (metrics) =>
                {
                    // Additional custom meter provider configuration can be added here
                },
                loggerProviderBuilderAction: (logger) =>
                {
                    // Additional custom logger provider configuration can be added here
                });
        //.AddOtlpExporter();
        //.AddApplicationInsightsExporter()
        //.AddSentryExporter(
        //    options =>
        //    {
        //        options.Environment = builder.Environment.EnvironmentName;
        //        options.Debug = builder.Environment.IsDevelopment();
        //    });

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("FontAwesomeRegular.otf", "fontawesome");
                fonts.AddFont("FluentSystemIconsRegular.ttf", FluentUI.FontFamily);
            })
            .ConfigureServices<App, Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(appBuilder =>
            {
                appBuilder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();

                appBuilder.Services.TryAddSingleton<ICameraService, CameraService>();
            },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(App).Namespace!))
#if WINDOWS
            //.ConfigureStoreUpdateIntegration()
            .ConfigureSingleInstanceApp(async e =>
            {
                foreach (var arg in e.EnsureNotNull())
                {
                    if (!string.IsNullOrEmpty(arg))
                        await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync(arg, "Environment");
                }
            }, async p =>
            {
                if (!string.IsNullOrEmpty(p.Uri?.Query))
                    await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync($"{p.Uri.Query}", "ProtocolActivatedEventArgs");
            }, async l =>
            {
                if (!string.IsNullOrEmpty(l.Arguments))
                    await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync($"{l.Arguments}", "LaunchActivatedEventArgs");
            })
#endif
            .ConfigureOfflineSynchronization(builder =>
            {
                Debug.WriteLine("Configuring offline synchronization");
            })
        .ConfigureSyncfusionCore();

        return builder.Build();
    }
}
