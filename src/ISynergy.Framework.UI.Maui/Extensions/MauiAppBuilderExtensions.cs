using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FileResult = ISynergy.Framework.Mvvm.Models.FileResult;

namespace ISynergy.Framework.UI.Extensions;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Gets the shellView model types.
    /// </summary>
    /// <value>The shellView model types.</value>
    public static IEnumerable<Type> ViewModelTypes { get; private set; }

    /// <summary>
    /// Gets the shellView types.
    /// </summary>
    /// <value>The shellView types.</value>
    public static IEnumerable<Type> ViewTypes { get; private set; }

    /// <summary>
    /// Gets the window types.
    /// </summary>
    /// <value>The window types.</value>
    public static IEnumerable<Type> WindowTypes { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TExceptionHandler"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="appBuilder"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureServices<TApplication, TContext, TExceptionHandler, TResource>(this MauiAppBuilder appBuilder, Func<AssemblyName, bool> assemblyFilter)
        where TApplication : class, Microsoft.Maui.IApplication
        where TContext : class, IContext
        where TExceptionHandler : class, IExceptionHandlerService
        where TResource : class
    {
        appBuilder.Services.AddLogging();
        appBuilder.Services.AddOptions();
        appBuilder.Services.AddPageResolver();

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

        appBuilder.Services.Configure<ConfigurationOptions>(appBuilder.Configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

        var infoService = InfoService.Default;
        infoService.LoadAssembly(mainAssembly);

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));

        // Register singleton services
        appBuilder.Services.TryAddSingleton<ILogger>((s) => LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        }).CreateLogger(AppDomain.CurrentDomain.FriendlyName));

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

        languageService.AddResourceManager(typeof(TResource));

        appBuilder.RegisterAssemblies(mainAssembly, assemblyFilter);

        return appBuilder
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ISynergy.ttf", "ISynergy");
                fonts.AddFont("OpenDyslexic3-Bold.ttf", "OpenDyslexic3-Bold");
                fonts.AddFont("OpenDyslexic3-Regular.ttf", "OpenDyslexic3-Regular");
                fonts.AddFont("SegMDL2.ttf", "SegoeMdl2");
                fonts.AddFont("segoeui.ttf", "SegoeUI");
            });
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="mainAssembly">The main assembly.</param>
    /// <param name="assemblyFilter">The assembly filter.</param>
    private static void RegisterAssemblies(this MauiAppBuilder appBuilder, Assembly mainAssembly, Func<AssemblyName, bool> assemblyFilter)
    {
        var referencedAssemblies = mainAssembly.GetAllReferencedAssemblyNames();
        var assemblies = new List<Assembly>();

        if (assemblyFilter is not null)
            foreach (var item in referencedAssemblies.Where(assemblyFilter).EnsureNotNull())
                assemblies.Add(Assembly.Load(item));

        foreach (var item in referencedAssemblies.Where(x =>
            x.Name.StartsWith("ISynergy.Framework.UI") ||
            x.Name.StartsWith("ISynergy.Framework.Mvvm")))
            assemblies.Add(Assembly.Load(item));

        appBuilder.RegisterAssemblies(assemblies);
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="assemblies">The assemblies.</param>
    private static void RegisterAssemblies(this MauiAppBuilder appBuilder, IEnumerable<Assembly> assemblies)
    {
        ViewTypes = assemblies.ToViewTypes();
        WindowTypes = assemblies.ToWindowTypes();
        ViewModelTypes = assemblies.ToViewModelTypes();

        appBuilder.Services.RegisterViewModels(ViewModelTypes);
        appBuilder.Services.RegisterViews(ViewTypes);
        appBuilder.Services.RegisterWindows(WindowTypes);

        appBuilder.Services.RegisterViewModelRoutes(ViewTypes);
    }

    private static void RegisterViewModelRoutes(this IServiceCollection services, IEnumerable<Type> views)
    {
        foreach (var view in views.Distinct())
        {
            if (ViewModelTypes.FirstOrDefault(q => q.Name.Equals(view.GetRelatedViewModel())) is Type viewmodel)
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
    /// Registers the services in the service collection with the page resolver
    /// </summary>
    /// <param name="services"></param>
    public static void AddPageResolver(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, ResolverService>());
    }
}
