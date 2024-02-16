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
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.Extensions;

public static class WindowsAppBuilderExtensions
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
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureServices<TApplication, TContext, TExceptionHandler, TResource>(this IServiceCollection services, IConfiguration configuration, Func<AssemblyName, bool> assemblyFilter)
        where TApplication : Microsoft.UI.Xaml.Application
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
        services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        services.TryAddSingleton<IConverterService, ConverterService>();
        services.TryAddSingleton<IBusyService, BusyService>();
        services.TryAddSingleton<IDialogService, DialogService>();
        services.TryAddSingleton<IDispatcherService, DispatcherService>();
        services.TryAddSingleton<IClipboardService, ClipboardService>();
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
    private static void RegisterAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        ViewTypes = assemblies.ToViewTypes();
        WindowTypes = assemblies.ToWindowTypes();
        ViewModelTypes = assemblies.ToViewModelTypes();

        services.RegisterViewModels(ViewModelTypes);
        services.RegisterViews(ViewTypes);
        services.RegisterWindows(WindowTypes);
    }
}
