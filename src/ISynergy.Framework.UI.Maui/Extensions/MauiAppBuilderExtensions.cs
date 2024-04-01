using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FileResult = ISynergy.Framework.Mvvm.Models.FileResult;

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
    public static MauiAppBuilder ConfigureServices<TApplication, TContext, TExceptionHandler, TResource, TLoadingView>(this MauiAppBuilder appBuilder, Action<IServiceCollection, IConfiguration> action)
    where TApplication : class, Microsoft.Maui.IApplication
    where TContext : class, IContext
    where TExceptionHandler : class, IExceptionHandlerService
    where TResource : class
    where TLoadingView : class, ILoadingView
    {
        appBuilder.Services.AddOptions();
        appBuilder.Services.AddPageResolver();

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

        appBuilder.Services.Configure<ConfigurationOptions>(appBuilder.Configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

        var infoService = InfoService.Default;
        infoService.LoadAssembly(mainAssembly);

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        appBuilder.Services.TryAddSingleton<IInfoService>(s => InfoService.Default);
        appBuilder.Services.TryAddSingleton<ILanguageService>(s => LanguageService.Default);
        appBuilder.Services.TryAddSingleton<IMessageService>(s => MessageService.Default);

        appBuilder.Services.TryAddSingleton<TContext>();
        appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, TExceptionHandler>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<ILocalizationService, LocalizationService>();
        appBuilder.Services.TryAddSingleton<IConverterService, ConverterService>();
        appBuilder.Services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<IDialogService, DialogService>();
        appBuilder.Services.TryAddSingleton<IDispatcherService, DispatcherService>();
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        appBuilder.RegisterAssemblies();

        action.Invoke(appBuilder.Services, appBuilder.Configuration);

        appBuilder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("opensans-medium.ttf", "OpenSansMedium");
                fonts.AddFont("opensans-regular.ttf", "OpenSansRegular");
                fonts.AddFont("opensans-semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("segoemdl2.ttf", "SegoeMdl2");
                fonts.AddFont("opendyslexic3-bold.ttf", "OpenDyslexic3-Bold");
                fonts.AddFont("opendyslexic3-regular.ttf", "OpenDyslexic3-Regular");
            });

        ServiceLocator.SetLocatorProvider(appBuilder.Services.BuildServiceProvider());

        return appBuilder;
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="appBuilder"></param>
    private static void RegisterAssemblies(this MauiAppBuilder appBuilder)
    {
        var viewTypes = ReflectionExtensions.GetViewTypes();
        var windowTypes = ReflectionExtensions.GetWindowTypes();
        var viewModelTypes = ReflectionExtensions.GetViewModelTypes();

        appBuilder.Services.RegisterViewModels(viewModelTypes);
        appBuilder.Services.RegisterViews(viewTypes);
        appBuilder.Services.RegisterWindows(windowTypes);

        appBuilder.Services.RegisterViewModelRoutes(viewModelTypes, viewTypes);
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
        var abstraction = viewmodel
            .GetInterfaces()
            .FirstOrDefault(q =>
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
