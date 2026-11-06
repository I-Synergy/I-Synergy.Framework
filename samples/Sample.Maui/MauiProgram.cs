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
#if MACOS
using Microsoft.Maui.Platforms.MacOS.Essentials;
using Microsoft.Maui.Platforms.MacOS.Hosting;
#endif
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
#if MACOS
            // Mac Catalyst and the mobile heads use Microsoft.Maui.Controls' own hosting; the AppKit
            // backend ships its own (UseMauiAppMacOS + AddMacOSEssentials) and has no MediaElement
            // handler, so MediaElement is not registered on this target.
            .UseMauiAppMacOS<App>()
            .AddMacOSEssentials()
#else
            .UseMauiApp<App>()
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
#if WINDOWS
            //.ConfigureStoreUpdateIntegration()
            .ConfigureSingleInstance()
            .ConfigureActivationHandlers(async e =>
            {
                foreach (var arg in e.EnsureNotNull())
                {
                    if (!string.IsNullOrEmpty(arg))
                        await IPlatformApplication.Current!.Services.GetRequiredService<IDialogService>().ShowMessageAsync(arg, "Environment");
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

                await IPlatformApplication.Current!.Services.GetRequiredService<IDialogService>()
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

                await IPlatformApplication.Current!.Services.GetRequiredService<IDialogService>()
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
#if !MACOS
            // Syncfusion.Maui.Core ships no AppKit asset, so the AppKit head runs without its handlers.
            .ConfigureSyncfusionCore()
#endif
            ;

#if WINDOWS
        // The shell header is drawn by the root page and the window keeps its native title bar
        // (default window chrome). WinUI draws a focus rectangle around the first focusable control,
        // which reads as a permanent "selected" state on the header icon buttons; the rectangle is
        // suppressed while keyboard focus still moves between them.
        Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping(
            "HeaderIconFocusVisual",
            (handler, view) =>
            {
                if (view is Button { StyleId: "HeaderIconButton" })
                    handler.PlatformView.UseSystemFocusVisuals = false;
            });


        // Cursors: MAUI has no cursor API, so the buttons and the tappable rows/labels get the hand
        // cursor through their platform element.
        Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping(
            "HandCursor", (handler, view) => ApplyHandCursor(handler.PlatformView));
        Microsoft.Maui.Handlers.BorderHandler.Mapper.AppendToMapping(
            "HandCursor", (handler, view) =>
            {
                if (view is Border { StyleId: "SidebarRow" })
                    ApplyHandCursor(handler.PlatformView);
            });
        Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping(
            "HandCursor", (handler, view) =>
            {
                if (view is Label { StyleId: "LinkLabel" })
                    ApplyHandCursor(handler.PlatformView);
            });
#endif

#if DEBUG
        builder.AddMauiDevFlowAgent();
#endif

        return builder.Build();
    }

#if WINDOWS
    private static void ApplyHandCursor(Microsoft.UI.Xaml.UIElement? element)
    {
        try
        {
            var property = typeof(Microsoft.UI.Xaml.UIElement).GetProperty(
                "ProtectedCursor",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            property?.SetValue(
                element,
                Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand));
        }
        catch (Exception)
        {
            // A missing cursor is cosmetic; never fail the handler mapping over it.
        }
    }
#endif
}
