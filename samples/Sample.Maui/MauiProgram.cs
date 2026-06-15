using CommunityToolkit.Maui;
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
using Sample.Abstractions.Services;
using Sample.Extensions;
using Sample.OpenTelemetry;
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
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitMediaElement(false)
            .UseFonts(fonts =>
            {
                fonts.AddFont("FontAwesomeRegular.otf", "fontawesome");
                fonts.AddFont("FluentSystemIconsRegular.ttf", FluentUI.FontFamily);
            })
            .ConfigureServices<App, Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(appBuilder =>
            {
                appBuilder.Services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                appBuilder.Services.TryAddSingleton<ICameraService, CameraService>();
                appBuilder.Services.TryAddSingleton<ApplicationMonitor>();
            },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(App).Namespace!))
#if WINDOWS
            //.ConfigureStoreUpdateIntegration()
            .ConfigureSingleInstance()
            .ConfigureActivationHandlers(async e =>
            {
                foreach (var arg in e.EnsureNotNull())
                {
                    if (!string.IsNullOrEmpty(arg))
                        await ServiceLocator.Default.GetRequiredService<IDialogService>().ShowMessageAsync(arg, "Environment");
                }
            }, async p =>
            {
                if (p.Uri is null)
                    return;

                var queryArgs = p.Uri.Query.ParseQueryString();

                // Skip --environment / -e equivalents — already consumed at startup
                queryArgs.Remove("environment");
                queryArgs.Remove("e");

                var parts = new List<string>();

                if (!string.IsNullOrEmpty(p.Uri.Host))
                    parts.Add($"Host: {p.Uri.Host}");

                if (!string.IsNullOrEmpty(p.Uri.AbsolutePath) && p.Uri.AbsolutePath != "/")
                    parts.Add($"Path: {p.Uri.AbsolutePath}");

                if (queryArgs.Count > 0)
                    parts.AddRange(queryArgs.Select(kvp => $"  {kvp.Key} = {kvp.Value}"));

                if (parts.Count == 0)
                    return;

                await ServiceLocator.Default.GetRequiredService<IDialogService>()
                    .ShowMessageAsync(string.Join(Environment.NewLine, parts), "Protocol Activation");
            }, async l =>
            {
                if (string.IsNullOrEmpty(l.Arguments))
                    return;

                var args = l.Arguments.ParseCommandLineArgs();

                // Skip --environment / -e — already consumed at startup by TryGetEnvironmentFromLaunchArgs()
                args.Remove("environment");
                args.Remove("e");

                if (args.Count == 0)
                    return;

                // Dispatch recognized commands to their handlers; show remaining as structured summary
                var message = string.Join(Environment.NewLine,
                    args.Select(kvp => $"--{kvp.Key} {kvp.Value}"));

                await ServiceLocator.Default.GetRequiredService<IDialogService>()
                    .ShowMessageAsync(message, "Launch Arguments");
            })
#endif
            .ConfigureSupportedCultures(cultures =>
            {
                cultures.AddCulture("nl");
                cultures.AddCulture("de");
                cultures.AddCulture("fr");
                cultures.AddCulture("ar");
            })
            .ConfigureOfflineSynchronization(builder =>
            {
                Debug.WriteLine("Configuring offline synchronization");
            })
        .ConfigureSyncfusionCore();

        return builder.Build();
    }






}
