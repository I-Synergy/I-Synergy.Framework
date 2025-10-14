using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FileResult = ISynergy.Framework.Core.Models.Results.FileResult;

namespace ISynergy.Framework.UI.Extensions;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="MauiAppBuilder"/> and adds logging.
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
    /// <typeparam name="TExceptionHandler"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <typeparam name="TLoadingView"></typeparam>
    /// <param name="appBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureServices<TApplication, TContext, TCommonServices, TSettingsService, TResource>(
        this MauiAppBuilder appBuilder,
        Action<MauiAppBuilder> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
    where TApplication : class, Microsoft.Maui.IApplication
    where TContext : class, IContext
    where TCommonServices : class, ICommonServices
    where TSettingsService : class, ISettingsService
    where TResource : class
    {
        appBuilder.Services.AddOptions();
        appBuilder.Services.AddPageResolver();

        appBuilder.Services.Configure<ApplicationFeatures>(appBuilder.Configuration.GetSection(nameof(ApplicationFeatures)).BindWithReload);
        appBuilder.Services.Configure<ClientApplicationOptions>(appBuilder.Configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));
        var infoService = InfoService.Default;
        infoService.LoadAssembly(mainAssembly);

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        appBuilder.Services.TryAddSingleton<IInfoService>(s => InfoService.Default);
        appBuilder.Services.TryAddSingleton<ILanguageService>(s => LanguageService.Default);
        appBuilder.Services.TryAddSingleton<IMessengerService>(s => MessengerService.Default);
        appBuilder.Services.TryAddSingleton<IPreferences>(s => Preferences.Default);
        appBuilder.Services.TryAddSingleton<IMigrationService, MigrationService>();

        appBuilder.Services.TryAddSingleton<TContext>();
        appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddScoped<ISettingsService, TSettingsService>();
        appBuilder.Services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
        appBuilder.Services.TryAddSingleton<IDispatcherService, DispatcherService>();
        appBuilder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<IDialogService, DialogService>();
        appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();
        appBuilder.Services.TryAddSingleton<ICommonServices, TCommonServices>();

        appBuilder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(appBuilder);

        appBuilder
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

        ServiceLocator.SetLocatorProvider(appBuilder.Services.BuildServiceProvider());

        return appBuilder;
    }

    private static void RegisterViewModelRoutes(this IServiceCollection services, IEnumerable<Type> viewModelTypes, IEnumerable<Type> viewTypes)
    {
        foreach (var view in viewTypes.Distinct().EnsureNotNull())
        {
            if (viewModelTypes.FirstOrDefault(q => q.Name.Equals(view.GetRelatedViewModel())) is { } viewmodel)
                services.RegisterViewModelRoute(viewmodel, view);
        }
    }

    private static void RegisterViewModelRoute(this IServiceCollection services, Type viewmodel, Type view)
    {
        var abstraction = Array.Find(viewmodel
            .GetInterfaces(), q =>
                q.GetInterfaces().Contains(typeof(IViewModel))
                && !q.Name.StartsWith(nameof(IViewModel)));

        services.RegisterRoute(viewmodel, abstraction, view);
    }

    private static void RegisterRoute(this IServiceCollection services, Type type, Type abstraction, Type view)
    {
        if (abstraction is not null)
            Routing.RegisterRoute(abstraction.Name, view);

        Routing.RegisterRoute(type.Name, view);
    }

    /// <summary>
    /// Registers the action in the service collection with the page resolver
    /// </summary>
    /// <param name="services"></param>
    public static void AddPageResolver(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, ResolverService>());
    }
}
