using CommunityToolkit.Maui;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using FileResult = ISynergy.Framework.Core.Models.Results.FileResult;

namespace ISynergy.Framework.UI.Extensions;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="MauiAppBuilder"/> and adds logging with OpenTelemetry support.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="infoService"></param>
    /// <param name="tracerProviderBuilderAction"></param>
    /// <param name="meterProviderBuilderAction"></param>
    /// <param name="loggerProviderBuilderAction"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureLogging(
        this MauiAppBuilder appBuilder,
        IInfoService infoService,
        Action<TracerProviderBuilder>? tracerProviderBuilderAction = null,
        Action<MeterProviderBuilder>? meterProviderBuilderAction = null,
        Action<LoggerProviderBuilder>? loggerProviderBuilderAction = null)
    {
        appBuilder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);

#if DEBUG
            loggingBuilder.AddDebug();
#endif

            // Configure OpenTelemetry
            loggingBuilder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(
                            serviceName: infoService.ProductName,
                            serviceVersion: infoService.ProductVersion.ToString())
                        .AddTelemetrySdk()
                        .AddEnvironmentVariableDetector());

#if DEBUG
                options.AddConsoleExporter();
#endif
            });
        });

        // Configure tracing
        appBuilder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: infoService.ProductName,
                                serviceVersion: infoService.ProductVersion.ToString())
                            .AddTelemetrySdk()
                            .AddEnvironmentVariableDetector());

                // Add HTTP client instrumentation
                tracerProviderBuilder.AddHttpClientInstrumentation();

#if DEBUG
                tracerProviderBuilder.AddConsoleExporter();
#endif

                tracerProviderBuilderAction?.Invoke(tracerProviderBuilder);
            })
            .WithMetrics(meterProviderBuilder =>
            {
                meterProviderBuilder
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: infoService.ProductName,
                                serviceVersion: infoService.ProductVersion.ToString())
                            .AddTelemetrySdk()
                            .AddEnvironmentVariableDetector());

                // Add HTTP client instrumentation
                meterProviderBuilder.AddHttpClientInstrumentation();

                // Add runtime instrumentation
                meterProviderBuilder.AddRuntimeInstrumentation();

#if DEBUG
                meterProviderBuilder.AddConsoleExporter();
#endif

                meterProviderBuilderAction?.Invoke(meterProviderBuilder);
            });

        return appBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="MauiAppBuilder"/> and adds logging with a custom configuration action.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="logging"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureLogging(this MauiAppBuilder appBuilder, Action<ILoggingBuilder, IConfiguration> logging)
    {
        appBuilder.Services.AddLogging();

        // Register singleton action
        appBuilder.Services.TryAddSingleton<ILogger>((s) => LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        }).CreateLogger(AppDomain.CurrentDomain.FriendlyName));

#if DEBUG
        appBuilder.Logging.AddDebug();
#endif

        logging.Invoke(appBuilder.Logging, appBuilder.Configuration);

        return appBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all services.
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TExceptionHandlerService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="appBuilder"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureServices<TApplication, TContext, TCommonServices, TExceptionHandlerService, TSettingsService, TResource>(
        this MauiAppBuilder appBuilder,
        Action<MauiAppBuilder> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TApplication : class, Microsoft.Maui.IApplication
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TExceptionHandlerService : class, IExceptionHandlerService
        where TSettingsService : class, ISettingsService
        where TResource : class
    {
        appBuilder.Services.AddOptions();
        appBuilder.Services.AddPageResolver();

        appBuilder.Services.Configure<ApplicationFeatures>(appBuilder.Configuration.GetSection(nameof(ApplicationFeatures)).BindWithReload);
        appBuilder.Services.Configure<ClientApplicationOptions>(appBuilder.Configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));
        if (mainAssembly is not null)
        {
            var infoService = InfoService.Default;
            infoService.LoadAssembly(mainAssembly);
        }

        var languageService = new LanguageService();
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        appBuilder.Services.TryAddSingleton<IInfoService>(s => InfoService.Default);
        appBuilder.Services.TryAddSingleton<ILanguageService>(s => languageService);
        appBuilder.Services.TryAddSingleton<IMessengerService, MessengerService>();
        appBuilder.Services.TryAddSingleton<IPreferences>(s => Preferences.Default);
        appBuilder.Services.TryAddSingleton<IMigrationService, MigrationService>();

        appBuilder.Services.TryAddSingleton<TContext>();
        appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddSingleton<TCommonServices>();
        appBuilder.Services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();

        appBuilder.Services.TryAddScoped<ISettingsService, TSettingsService>();
        appBuilder.Services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();

        appBuilder.Services.TryAddSingleton<ITokenStorageService, TokenStorageService>();
        appBuilder.Services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
        appBuilder.Services.TryAddSingleton<IDispatcherService, DispatcherService>();
        appBuilder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<IDialogService, DialogService>();
        appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        appBuilder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(appBuilder);

        appBuilder
            .UseMauiCommunityToolkitMediaElement(false)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("segoeui.ttf", "SegoeUI");
                fonts.AddFont("segoesb.ttf", "SegoeSemiBold");
                fonts.AddFont("segoeuib.ttf", "SegoeUIBold");
                fonts.AddFont("segoeuil.ttf", "SegoeUILight");
                fonts.AddFont("segoeuisl.ttf", "SegoeUISemiLight");
                fonts.AddFont("segoeuisb.ttf", "SegoeUISemiBold");
                fonts.AddFont("segoemdl2.ttf", "SegoeMdl2");
                fonts.AddFont("opensans-medium.ttf", "OpenSansMedium");
                fonts.AddFont("opensans-regular.ttf", "OpenSansRegular");
                fonts.AddFont("opensans-semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("opendyslexic3-bold.ttf", "OpenDyslexic3-Bold");
                fonts.AddFont("opendyslexic3-regular.ttf", "OpenDyslexic3-Regular");
            });

        // Is not needed. Is resolved via MauiInitializeService
        // ServiceLocator.SetLocatorProvider(appBuilder.Services.BuildServiceProvider());

        return appBuilder;
    }

    /// <summary>
    /// Registers the action in the service collection with the page resolver
    /// </summary>
    /// <param name="services"></param>
    public static void AddPageResolver(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, MauiInitializerService>());
    }
}
