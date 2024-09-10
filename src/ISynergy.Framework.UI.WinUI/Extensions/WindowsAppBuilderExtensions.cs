using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ISynergy.Framework.UI.Extensions;

public static class WindowsAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and adds loggingBuilder.
    /// </summary>
    /// <param name="windowsAppBuilder"></param>
    /// <param name="loggingBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureLogging(this IHostBuilder windowsAppBuilder, Action<ILoggingBuilder, IConfiguration> loggingBuilder)
    {
        windowsAppBuilder.ConfigureLogging((context, logger) =>
        {
#if DEBUG
            logger.AddDebug();
#endif
            logger.SetMinimumLevel(LogLevel.Trace);

            loggingBuilder.Invoke(logger, context.Configuration);
        });

        return windowsAppBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TExceptionHandler"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="windowsAppBuilder"></param>
    /// <param name="action"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureServices<TApplication, TContext, TExceptionHandler, TResource>(
        this IHostBuilder windowsAppBuilder,
        Action<IServiceCollection, IConfiguration> action,
        Func<AssemblyName, bool> assemblyFilter)
        where TApplication : Microsoft.UI.Xaml.Application
        where TContext : class, IContext
        where TExceptionHandler : class, IExceptionHandlerService
        where TResource : class
    {
        windowsAppBuilder.ConfigureServices((context, services) =>
        {
            services.AddLogging();
            services.AddOptions();

            // Register singleton windowsAppBuilder
            services.TryAddSingleton<ILogger>((s) => LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Trace);
            }).CreateLogger(AppDomain.CurrentDomain.FriendlyName));

            var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

            services.Configure<ConfigurationOptions>(context.Configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            var infoService = InfoService.Default;
            infoService.LoadAssembly(mainAssembly);

            var languageService = LanguageService.Default;
            languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
            languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
            languageService.AddResourceManager(typeof(TResource));

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

            services.RegisterAssemblies(mainAssembly, assemblyFilter);

            action.Invoke(services, context.Configuration);

            ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());
        });

        return windowsAppBuilder;
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
    /// <param name="assemblies"></param>
    private static void RegisterAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var viewTypes = assemblies.ToViewTypes();
        var windowTypes = assemblies.ToWindowTypes();
        var viewModelTypes = assemblies.ToViewModelTypes();

        services.RegisterViewModels(viewModelTypes);
        services.RegisterViews(viewTypes);
        services.RegisterWindows(windowTypes);
    }
}
