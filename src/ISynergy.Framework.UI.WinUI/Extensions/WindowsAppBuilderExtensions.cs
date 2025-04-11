using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
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
            logger.AddConfiguration(context.Configuration.GetSection("Logging"));
#if DEBUG
            logger.AddDebug();
#endif
            loggingBuilder.Invoke(logger, context.Configuration);
        });

        return windowsAppBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TAuthenticationService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="windowsAppBuilder"></param>
    /// <param name="action"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureServices<TApplication, TContext, TCommonServices, TAuthenticationService, TSettingsService, TResource>(
        this IHostBuilder windowsAppBuilder,
        Action<IServiceCollection, IConfiguration> action,
        Func<AssemblyName, bool> assemblyFilter)
        where TApplication : Microsoft.UI.Xaml.Application
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TSettingsService : class, ISettingsService
        where TAuthenticationService : class, IAuthenticationService
        where TResource : class
    {
        windowsAppBuilder.ConfigureServices((context, services) =>
        {
            services.AddOptions();

            var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

            if (mainAssembly is null)
                throw new ArgumentNullException(nameof(mainAssembly));

            services.Configure<Features>(context.Configuration.GetSection(nameof(Features)).BindWithReload);
            services.Configure<ConfigurationOptions>(context.Configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            var infoService = new InfoService();
            infoService.LoadAssembly(mainAssembly);

            var languageService = LanguageService.Default;
            languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
            languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
            languageService.AddResourceManager(typeof(TResource));

            services.TryAddSingleton<IInfoService>(s => infoService);
            services.TryAddSingleton<ILanguageService>(s => languageService);
            services.TryAddSingleton<IMessageService>(s => MessageService.Default);

            services.TryAddScoped<TContext>();
            services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());

            services.TryAddScoped<ISettingsService, TSettingsService>();
            services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();
            services.TryAddScoped<ICredentialLockerService, CredentialLockerService>();

            services.TryAddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
            services.TryAddSingleton<IScopedContextService, ScopedContextService>();
            services.TryAddSingleton<INavigationService, NavigationService>();
            services.TryAddSingleton<IBusyService, BusyService>();
            services.TryAddSingleton<IDialogService, DialogService>();
            services.TryAddSingleton<IClipboardService, ClipboardService>();
            services.TryAddSingleton<IFileService<FileResult>, FileService>();
            services.TryAddSingleton<IAuthenticationService, TAuthenticationService>();
            services.TryAddSingleton<ICommonServices, TCommonServices>();
            services.TryAddSingleton<IUpdateService, UpdateService>();

            services.RegisterAssemblies(mainAssembly, assemblyFilter);

            action.Invoke(services, context.Configuration);

            services.BuildServiceProviderWithLocator(true);
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
            x.Name!.StartsWith("ISynergy.Framework.UI") ||
            x.Name!.StartsWith("ISynergy.Framework.Mvvm")).EnsureNotNull())
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

    public static ResourceDictionary AddToResourceDictionary<T>(this ResourceDictionary resources, IScopedContextService scopedContextService, string? key = null, Func<T>? implementation = null)
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        if (resources.ContainsKey(key))
            resources.Remove(key);

        if (implementation is not null)
            resources.Add(key, implementation.Invoke());
        else
            resources.Add(key, scopedContextService.GetService<T>());

        return resources;
    }
}
