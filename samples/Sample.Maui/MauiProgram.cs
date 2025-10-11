using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
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
        var mainAssembly = Assembly.GetAssembly(typeof(App));
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonStream(mainAssembly.GetManifestResourceStream("Sample.appsettings.json"));
        var config = configBuilder.Build();

        builder
            .Configuration
            .AddConfiguration(config);

        builder
            .Logging
            .AddTelemetry(
                builder,
                infoService,
                tracerProviderBuilderAction: (tracing) =>
                {
                    tracing.AddHttpClientInstrumentation();
                },
                meterProviderBuilderAction: (metrics) =>
                {
                    metrics.AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                },
                loggerProviderBuilderAction: (logger) =>
                {
                })
                .AddOtlpExporter()
                .AddApplicationInsightsExporter()
                .AddSentryExporter(
                    options =>
                    {
                        options.Environment = builder.Environment.EnvironmentName;
                        options.Debug = builder.Environment.IsDevelopment();
                    });

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Font Awesome 6 Pro-Regular-400.otf", "fontawesome");
            })
            .ConfigureServices<App, Context, CommonServices, SettingsService, Properties.Resources>(appBuilder =>
            {
                appBuilder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();

                appBuilder.Services.TryAddSingleton<CommonServices>();
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
