using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;
using FileResult = ISynergy.Framework.Core.Models.Results.FileResult;

namespace ISynergy.Framework.UI.Extensions;

public static class MauiAppBuilderExtensions
{
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
    /// <typeparam name="TApplication">The MAUI application type.</typeparam>
    /// <typeparam name="TContext">The application context type.</typeparam>
    /// <typeparam name="TCommonServices">The common services type.</typeparam>
    /// <typeparam name="TExceptionHandlerService">The exception handler service type.</typeparam>
    /// <typeparam name="TSettingsService">The settings service type.</typeparam>
    /// <typeparam name="TResource">The resource type for localization.</typeparam>
    /// <param name="appBuilder">The MAUI app builder.</param>
    /// <param name="action">Additional service configuration action.</param>
    /// <param name="assembly">The assembly to scan for ViewModels and Views.</param>
    /// <param name="assemblyFilter">A filter function for assembly names.</param>
    /// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder ConfigureServices<TApplication, TContext, TCommonServices, TExceptionHandlerService, TSettingsService, TResource>( // NOSONAR
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
        appBuilder.Services.TryAddSingleton<IPreferences>(s => Preferences.Default);
        appBuilder.Services.TryAddSingleton<IMigrationService, MigrationService>();

        appBuilder.Services.TryAddSingleton<TContext>();
        appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddSingleton<TCommonServices>();
        appBuilder.Services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();

        appBuilder.Services.TryAddSingleton<ISettingsService, TSettingsService>();
        appBuilder.Services.TryAddSingleton<ILocalSettings>((s) => s.GetRequiredService<ISettingsService>().LocalSettings);
        appBuilder.Services.TryAddSingleton<IRoamingSettings>((s) => s.GetRequiredService<ISettingsService>().RoamingSettings);
        appBuilder.Services.TryAddSingleton<IGlobalSettings>((s) => s.GetRequiredService<ISettingsService>().GlobalSettings);
        appBuilder.Services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();

        appBuilder.Services.TryAddSingleton<ITokenStorageService, TokenStorageService>();
        appBuilder.Services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
        appBuilder.Services.TryAddSingleton<IDispatcherService, DispatcherService>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<IDialogService, DialogService>();
        appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        appBuilder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(appBuilder);

        return appBuilder;
    }

    /// <summary>
    /// Registers the framework's default fonts with the app builder, then invokes an optional
    /// delegate for the consumer's own fonts. Framework fonts are registered first, so consumer
    /// fonts passed via <paramref name="configureFonts"/> take priority when aliases overlap.
    /// </summary>
    /// <param name="appBuilder">The MAUI app builder.</param>
    /// <param name="configureFonts">Optional delegate to register additional consumer fonts.</param>
    /// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder UseFonts(
        this MauiAppBuilder appBuilder,
        Action<IFontCollection>? configureFonts = null)
    {
        appBuilder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("segoeui.ttf", "SegoeUI");
            fonts.AddFont("segoeuisb.ttf", "SegoeSemiBold");
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

        if (configureFonts is not null)
            appBuilder.ConfigureFonts(configureFonts);

        return appBuilder;
    }

    /// <summary>
    /// Registers the available theme colors. Each color is an option for the application's
    /// color picker or theme switcher. The full palette (secondary, tertiary, tinted grays)
    /// is always calculated dynamically from the color stored in <c>ILocalSettings.Color</c>,
    /// regardless of whether it appears in this list.
    /// </summary>
    /// <param name="appBuilder">The MAUI app builder.</param>
    /// <param name="configure">A delegate that adds hex color strings (e.g. "#0078D7") to the available list.</param>
    /// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder UseThemeColors(
        this MauiAppBuilder appBuilder,
        Action<IList<string>>? configure = null)
    {
        appBuilder.Services.Configure<ThemeColorOptions>(options =>
            configure?.Invoke(options.AvailableColors));
        return appBuilder;
    }

    /// <summary>
    /// Configures the list of cultures the application supports.
    /// English ("en") is always included automatically and serves as the default.
    /// </summary>
    /// <param name="appBuilder">The MAUI app builder.</param>
    /// <param name="configure">A delegate that adds cultures using the builder.</param>
    /// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder ConfigureSupportedCultures(
        this MauiAppBuilder appBuilder,
        Action<ISupportedCulturesBuilder> configure)
    {
        var builder = new SupportedCulturesBuilder();
        configure(builder);

        appBuilder.Services.Replace(ServiceDescriptor.Singleton<ISupportedCulturesService>(
            s => new SupportedCulturesService(builder.Build())));

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
