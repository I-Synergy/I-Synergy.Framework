using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.DevFlow.Agent;
using Microsoft.Maui.LifecycleEvents;
using Sample.Abstractions.Services;
using Sample.Extensions;
using Sample.OpenTelemetry;
using Sample.Services;
using Syncfusion.Maui.Core.Hosting;
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

        // Priority: command-line args > env var > Production default
        var environment = ApplicationExtensions.TryGetEnvironmentFromLaunchArgs()
            ?? Environment.GetEnvironmentVariable("Environment")
            ?? Environments.Production;

        var environment_stream = mainAssembly.GetManifestResourceStream($"Sample.appsettings.{environment}.json") ?? throw new InvalidOperationException("Unable to load appsettings resource");
        configBuilder.AddJsonStream(environment_stream);

        var config = configBuilder.Build();

        builder
            .Configuration
            .AddConfiguration(config);

        builder.Logging
            .AddTelemetry(
                builder,
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
                })
            .AddOtlpExporter();

        builder
            // The framework picks the hosting backend for the current head.
            .UseMauiAppForCurrentHead<App>()
#if !MACOS
            // Package limitation, not platform logic: the AppKit head has no MediaElement handler.
            .UseMauiCommunityToolkitMediaElement(false)
#endif
            .UseFonts(fonts =>
            {
                fonts.AddFont("FontAwesomeRegular.otf", "fontawesome");
                fonts.AddFont("FluentSystemIconsRegular.ttf", FluentUI.FontFamily);
                fonts.AddFont("Figtree-Regular.ttf", "Figtree");
                fonts.AddFont("Figtree-Medium.ttf", "FigtreeMedium");
                fonts.AddFont("Figtree-SemiBold.ttf", "FigtreeSemiBold");
                fonts.AddFont("Figtree-Bold.ttf", "FigtreeBold");
            })
            //.UseThemeColors(colors =>
            //{
            //    colors.Add("#0078D4");
            //    colors.Add("#800000");
            //    colors.Add("#5B92E5");
            //})
            .ConfigureServices<App, Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(appBuilder =>
            {
                appBuilder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                appBuilder.Services.TryAddSingleton<ICameraService, CameraService>();
                appBuilder.Services.TryAddSingleton<ApplicationMonitor>();
                appBuilder.Services.AddSingleton<ShellAccentTokens>();
            },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(App).Namespace!))
            // Windows-only activation plumbing (single instance, protocol/launch arguments); the
            // framework no-ops on the other heads.
            .ConfigureActivationDiagnostics()
            .ConfigureSupportedCultures(cultures =>
            {
                cultures.AddCulture("nl");
                cultures.AddCulture("de");
                cultures.AddCulture("fr");
                cultures.AddCulture("ar");
            })
#if !MACOS
            // Syncfusion.Maui.Core ships no AppKit asset, so the AppKit head runs without its handlers.
            .ConfigureSyncfusionCore()
#endif
            ;


#if DEBUG
        builder.AddMauiDevFlowAgent();
#endif

        return builder.Build();
    }

}
