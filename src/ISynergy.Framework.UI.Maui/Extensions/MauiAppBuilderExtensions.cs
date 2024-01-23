using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.Extensions;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Gets the shellView model types.
    /// </summary>
    /// <value>The shellView model types.</value>
    public static List<Type> ViewModelTypes { get; private set; }

    /// <summary>
    /// Gets the shellView types.
    /// </summary>
    /// <value>The shellView types.</value>
    public static List<Type> ViewTypes { get; private set; }

    /// <summary>
    /// Gets the window types.
    /// </summary>
    /// <value>The window types.</value>
    public static List<Type> WindowTypes { get; private set; }

    /// <summary>
    /// Bootstrapper types
    /// </summary>
    /// <value>The bootstrapper types.</value>
    public static List<Type> BootstrapperTypes { get; private set; }

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
            .UseMauiCommunityToolkitMarkup();
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
    private static void RegisterAssemblies(this MauiAppBuilder appBuilder, List<Assembly> assemblies)
    {
        ViewTypes = new List<Type>();
        WindowTypes = new List<Type>();
        ViewModelTypes = new List<Type>();
        BootstrapperTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            ViewModelTypes.AddRange(assembly.GetTypes()
                .Where(q =>
                    q.GetInterface(nameof(IViewModel), false) is not null
                    && !q.Name.Equals(GenericConstants.ShellViewModel)
                    && (q.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(q.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    && q.Name != GenericConstants.ViewModel
                    && !q.IsAbstract
                    && !q.IsInterface)
                .ToList());

            ViewTypes.AddRange(assembly.GetTypes()
                .Where(q =>
                    q.GetInterfaces().Any(a => a != null && a.FullName != null && a.FullName.Equals(typeof(IView).FullName))
                    && !q.Name.Equals(GenericConstants.ShellView)
                    && (q.Name.EndsWith(GenericConstants.View) || q.Name.EndsWith(GenericConstants.Page))
                    && q.Name != GenericConstants.View
                    && q.Name != GenericConstants.Page
                    && !q.IsAbstract
                    && !q.IsInterface)
                .ToList());

            WindowTypes.AddRange(assembly.GetTypes()
                .Where(q =>
                    q.GetInterfaces().Any(a => a != null && a.FullName != null && a.FullName.Equals(typeof(IWindow).FullName))
                    && q.Name.EndsWith(GenericConstants.Window)
                    && q.Name != GenericConstants.Window
                    && !q.IsAbstract
                    && !q.IsInterface)
                .ToList());

            BootstrapperTypes.AddRange(assembly.GetTypes()
                .Where(q =>
                    q.GetInterface(nameof(IBootstrap), false) is not null
                    && !q.IsAbstract
                    && !q.IsInterface)
                .ToList());
        }

        foreach (var viewmodel in ViewModelTypes.Distinct())
        {
            var abstraction = viewmodel
                .GetInterfaces()
                .FirstOrDefault(q =>
                    q.GetInterfaces().Contains(typeof(IViewModel))
                    && q.Name != nameof(IViewModel));

            if (abstraction is not null && !viewmodel.IsGenericType && abstraction != typeof(IQueryAttributable))
                appBuilder.Services.TryAddTransient(abstraction, viewmodel);

            appBuilder.Services.TryAddTransient(viewmodel);
        }

        foreach (var view in ViewTypes.Distinct())
        {
            var abstraction = view
                .GetInterfaces()
                .FirstOrDefault(q =>
                    q.GetInterfaces().Contains(typeof(IView))
                    && q.Name != nameof(IView));

            if (abstraction is not null)
                appBuilder.Services.TryAddTransient(abstraction, view);

            appBuilder.Services.TryAddTransient(view);
        }

        foreach (var window in WindowTypes.Distinct())
        {
            var abstraction = window
                .GetInterfaces()
                .FirstOrDefault(q =>
                    q.GetInterfaces().Contains(typeof(IWindow))
                    && q.Name != nameof(IWindow));

            if (abstraction is not null)
                appBuilder.Services.TryAddTransient(abstraction, window);

            appBuilder.Services.TryAddTransient(window);
        }

        foreach (var bootstrapper in BootstrapperTypes.Distinct())
        {
            appBuilder.Services.TryAddSingleton(bootstrapper);
        }
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
