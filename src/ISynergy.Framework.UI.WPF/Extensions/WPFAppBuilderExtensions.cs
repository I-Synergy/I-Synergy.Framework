using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace ISynergy.Framework.UI.Extensions;

public static class WPFAppBuilderExtensions
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
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureServices<TApplication, TContext, TExceptionHandler, TResource>(this IServiceCollection services, IConfiguration configuration, Func<AssemblyName, bool> assemblyFilter)
        where TApplication : Application
        where TContext : class, IContext
        where TExceptionHandler : class, IExceptionHandlerService
        where TResource : class
    {
        services.AddLogging();
        services.AddOptions();

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

        services.Configure<ConfigurationOptions>(configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

        var infoService = InfoService.Default;
        infoService.LoadAssembly(mainAssembly);

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));

        // Register singleton services
        services.TryAddSingleton<ILogger>((s) => LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        }).CreateLogger(AppDomain.CurrentDomain.FriendlyName));

        services.TryAddSingleton<IInfoService>(s => InfoService.Default);
        services.TryAddSingleton<ILanguageService>(s => LanguageService.Default);
        services.TryAddSingleton<IMessageService>(s => MessageService.Default);

        services.TryAddSingleton<TContext>();
        services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        services.TryAddSingleton<IExceptionHandlerService, TExceptionHandler>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        services.TryAddSingleton<ILocalizationService, LocalizationService>();
        services.TryAddSingleton<IConverterService, ConverterService>();
        services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        services.TryAddSingleton<IBusyService, BusyService>();
        services.TryAddSingleton<IDialogService, DialogService>();
        services.TryAddSingleton<IDispatcherService, DispatcherService>();
        services.TryAddSingleton<IClipboardService, ClipboardService>();
        services.TryAddSingleton<IToastMessageService, ToastMessageService>();
        services.TryAddSingleton<IThemeService, ThemeService>();
        services.TryAddSingleton<IFileService<FileResult>, FileService>();

        languageService.AddResourceManager(typeof(TResource));

        services.RegisterAssemblies(mainAssembly, assemblyFilter);

        return services;
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="mainAssembly">The main assembly.</param>
    /// <param name="assemblyFilter">The assembly filter.</param>
    private static void RegisterAssemblies(this IServiceCollection services, Assembly mainAssembly, Func<AssemblyName, bool> assemblyFilter)
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

        services.RegisterAssemblies(assemblies);
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies">The assemblies.</param>
    private static void RegisterAssemblies(this IServiceCollection services, List<Assembly> assemblies)
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
                    && (q.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(q.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
                    && q.Name != GenericConstants.ViewModel
                    && !q.IsAbstract
                    && !q.IsInterface)
                .ToList());

            ViewTypes.AddRange(assembly.GetTypes()
                .Where(q =>
                    q.GetInterfaces().Any(a => a != null && a.FullName != null && a.FullName.Equals(typeof(IView).FullName))
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

            if (abstraction is not null && !viewmodel.IsGenericType)
                services.TryAddScoped(abstraction, viewmodel);

            services.TryAddScoped(viewmodel);
        }

        foreach (var view in ViewTypes.Distinct())
        {
            var abstraction = view
                .GetInterfaces()
                .FirstOrDefault(q =>
                    q.GetInterfaces().Contains(typeof(IView))
                    && q.Name != nameof(IView));

            if (abstraction is not null)
                services.TryAddTransient(abstraction, view);

            services.TryAddTransient(view);
        }

        foreach (var window in WindowTypes.Distinct())
        {
            var abstraction = window
                .GetInterfaces()
                .FirstOrDefault(q =>
                    q.GetInterfaces().Contains(typeof(IWindow))
                    && q.Name != nameof(IWindow));

            if (abstraction is not null)
                services.TryAddTransient(abstraction, window);

            services.TryAddTransient(window);
        }

        foreach (var bootstrapper in BootstrapperTypes.Distinct())
        {
            services.TryAddSingleton(bootstrapper);
        }
    }
}
