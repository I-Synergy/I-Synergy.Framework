using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Globalization;
using System.Reflection;

using Windows.ApplicationModel.Activation;

namespace ISynergy.Framework.UI.Extensions;

public static class WindowsAppBuilderExtensions
{
    /// <summary>
    /// Adds logging with OpenTelemetry support to an <see cref="IHostApplicationBuilder"/>.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="infoService"></param>
    /// <param name="tracerProviderBuilderAction"></param>
    /// <param name="meterProviderBuilderAction"></param>
    /// <param name="loggerProviderBuilderAction"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureLogging(
        this IHostApplicationBuilder appBuilder,
        IInfoService infoService,
        Action<TracerProviderBuilder>? tracerProviderBuilderAction = null,
        Action<MeterProviderBuilder>? meterProviderBuilderAction = null,
        Action<LoggerProviderBuilder>? loggerProviderBuilderAction = null)
    {
        appBuilder.Logging.SetMinimumLevel(LogLevel.Trace);

#if DEBUG
        appBuilder.Logging.AddDebug();
#endif

        appBuilder.Services.AddOpenTelemetry()
            .WithLogging(loggerProviderBuilder =>
            {
                loggerProviderBuilder.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(
                            serviceName: infoService.ProductName,
                            serviceVersion: infoService.ProductVersion.ToString())
                        .AddTelemetrySdk()
                        .AddEnvironmentVariableDetector());

#if DEBUG
                loggerProviderBuilder.AddConsoleExporter();
#endif

                loggerProviderBuilderAction?.Invoke(loggerProviderBuilder);
            })
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

                meterProviderBuilder.AddHttpClientInstrumentation();
                meterProviderBuilder.AddRuntimeInstrumentation();

#if DEBUG
                meterProviderBuilder.AddConsoleExporter();
#endif

                meterProviderBuilderAction?.Invoke(meterProviderBuilder);
            });

        return appBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all appBuilder.
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
    public static IHostApplicationBuilder ConfigureServices<TApplication, TContext, TCommonServices, TExceptionHandlerService, TSettingsService, TResource>(
        this IHostApplicationBuilder appBuilder,
        Action<IServiceCollection> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TApplication : Application
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TExceptionHandlerService : class, IExceptionHandlerService
        where TSettingsService : class, ISettingsService
        where TResource : class
    {
        appBuilder.Services.AddOptions();

        appBuilder.Services.Configure<ApplicationFeatures>(appBuilder.Configuration.GetSection(nameof(ApplicationFeatures)).BindWithReload);
        appBuilder.Services.Configure<ClientApplicationOptions>(appBuilder.Configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));
        if (mainAssembly is not null)
        {
            var infoService = InfoService.Default;
            infoService.LoadAssembly(mainAssembly);
        }

#pragma warning disable IL2026 // typeof() arguments are statically known; resource types are preserved at compile time
        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(TResource));
#pragma warning restore IL2026

        appBuilder.Services.TryAddSingleton<IInfoService>(s => InfoService.Default);
        appBuilder.Services.TryAddSingleton<ILanguageService>(s => languageService);

        // Fallback: English-only if ConfigureSupportedCultures was not called
        appBuilder.Services.TryAddSingleton<ISupportedCulturesService>(s =>
            new SupportedCulturesService([CultureInfo.GetCultureInfo(GlobalizationConstants.DefaultCultureInfoName)]));

        appBuilder.Services.TryAddSingleton<IMessengerService, MessengerService>();

        appBuilder.Services.TryAddSingleton<TContext>();
        appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddSingleton<TCommonServices>();
        appBuilder.Services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();

        appBuilder.Services.TryAddSingleton<ISettingsService, TSettingsService>();
        appBuilder.Services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        appBuilder.Services.TryAddSingleton<IMigrationService, MigrationService>();
        appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();

        appBuilder.Services.TryAddSingleton<ITokenStorageService, TokenStorageService>();
        appBuilder.Services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
        appBuilder.Services.TryAddSingleton<IDispatcherService, DispatcherService>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<DialogService>();
        appBuilder.Services.TryAddSingleton<IDialogService>(sp => sp.GetRequiredService<DialogService>());
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        appBuilder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(appBuilder.Services);

        return appBuilder;
    }

    /// <summary>
    /// Registers callbacks that are invoked when a second instance redirects its
    /// activation (protocol URI, command-line args, etc.) to this instance.
    /// <para>
    /// Separate from single-instance enforcement (handled by WindowsAppSdkHost).
    /// This method only handles WHAT to do with the activation arguments.
    /// </para>
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="defaultAction">Called with command-line arguments (fallback).</param>
    /// <param name="protocolAction">Called for protocol URI activations (myapp://...).</param>
    /// <param name="launchAction">Called for standard launch activations.</param>
    public static IHostApplicationBuilder ConfigureActivationHandlers(
        this IHostApplicationBuilder builder,
        Action<string[]> defaultAction,
        Action<ProtocolActivatedEventArgs> protocolAction,
        Action<LaunchActivatedEventArgs> launchAction)
    {
        AppActivationExtensions.SetCallbacks(defaultAction, protocolAction, launchAction);
        return builder;
    }

    /// <summary>
    /// Enables single-instance behavior. When a second instance is launched, its
    /// activation is redirected to the first instance and the second process exits.
    /// <para>
    /// Without this call, multiple instances can run simultaneously.
    /// </para>
    /// </summary>
    public static IHostApplicationBuilder ConfigureSingleInstance(this IHostApplicationBuilder builder)
    {
        AppActivationExtensions.IsSingleInstanceEnabled = true;
        return builder;
    }

    /// <summary>
    /// Configures the list of cultures the application supports.
    /// English ("en") is always included automatically and serves as the default.
    /// </summary>
    /// <param name="appBuilder">The host application builder.</param>
    /// <param name="configure">A delegate that adds cultures using the builder.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/>.</returns>
    public static IHostApplicationBuilder ConfigureSupportedCultures(
        this IHostApplicationBuilder appBuilder,
        Action<ISupportedCulturesBuilder> configure)
    {
        var builder = new SupportedCulturesBuilder();
        configure(builder);
        appBuilder.Services.Replace(ServiceDescriptor.Singleton<ISupportedCulturesService>(
            s => new SupportedCulturesService(builder.Build())));
        return appBuilder;
    }
}
