using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Windows;

namespace ISynergy.Framework.UI.Extensions;

public static class WpfAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IHostBuilder"/> and configures all services.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="wpfAppBuilder"></param>
    /// <param name="infoService"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureServices<TContext, TCommonServices, TSettingsService, TResource>(
        this IHostBuilder wpfAppBuilder,
        IInfoService infoService,
        Action<IConfiguration, IHostEnvironment, IServiceCollection> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TSettingsService : class, ISettingsService
        where TResource : class
    {
        wpfAppBuilder.ConfigureServices((context, services) =>
        {
            services.AddOptions();

            services.Configure<ApplicationOptions>(context.Configuration.GetSection(nameof(ApplicationOptions)).BindWithReload);

            var languageService = LanguageService.Default;
            languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
            languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
            languageService.AddResourceManager(typeof(TResource));

            services.TryAddSingleton<IInfoService>(s => infoService);
            services.TryAddSingleton<ILanguageService>(s => languageService);
            services.TryAddSingleton<IMessengerService>(s => MessengerService.Default);

            services.TryAddScoped<TContext>();
            services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());

            services.TryAddScoped<ISettingsService, TSettingsService>();
            services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();


            services.TryAddSingleton<IExceptionHandlerService, ExceptionHandlerService>();

            services.TryAddSingleton<IScopedContextService, ScopedContextService>();
            services.TryAddSingleton<IBusyService, BusyService>();

            services.TryAddSingleton<IClipboardService, ClipboardService>();

            services.TryAddScoped<ICredentialLockerService, CredentialLockerService>();
            services.TryAddSingleton<INavigationService, NavigationService>();
            services.TryAddSingleton<IDialogService, DialogService>();
            services.TryAddSingleton<IToastMessageService, ToastMessageService>();
            services.TryAddSingleton<IFileService<FileResult>, FileService>();

            services.TryAddSingleton<ICommonServices, TCommonServices>();

            services.RegisterAssemblies(assembly, assemblyFilter);

            action.Invoke(context.Configuration, context.HostingEnvironment, services);
        });

        return wpfAppBuilder;
    }

    /// <summary>
    /// Adds update integration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddUpdatesIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UpdateOptions>(configuration.GetSection(nameof(UpdateOptions)).BindWithReload);
        services.TryAddSingleton<IUpdateService, UpdateService>();
        return services;
    }

    public static ResourceDictionary AddToResourceDictionary<T>(this ResourceDictionary resources, IScopedContextService scopedContextService, string? key = null, Func<T>? implementation = null)
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        if (resources[key] is not null)
            resources.Remove(key);

        if (implementation is not null)
            resources.Add(key, implementation.Invoke());
        else
            resources.Add(key, scopedContextService.GetService<T>());

        return resources;
    }
}
