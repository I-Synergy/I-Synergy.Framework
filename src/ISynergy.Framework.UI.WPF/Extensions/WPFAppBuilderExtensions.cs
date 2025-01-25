using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
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
using System.Windows;

namespace ISynergy.Framework.UI.Extensions;

public static class WPFAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IHostBuilder"/> and adds loggingBuilder.
    /// </summary>
    /// <param name="wpfAppBuilder"></param>
    /// <param name="loggingBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureLogging(this IHostBuilder wpfAppBuilder, Action<ILoggingBuilder, IConfiguration> loggingBuilder)
    {
        wpfAppBuilder.ConfigureLogging((context, logger) =>
        {
#if DEBUG
            logger.AddDebug();
#endif
            logger.SetMinimumLevel(LogLevel.Trace);

            loggingBuilder.Invoke(logger, context.Configuration);
        });

        return wpfAppBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="IHostBuilder"/> and configures all services.
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TAuthenticationService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="wpfAppBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureServices<TApplication, TContext, TCommonServices, TAuthenticationService, TSettingsService, TResource>(this IHostBuilder wpfAppBuilder, Action<IServiceCollection, IConfiguration> action)
        where TApplication : Application
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TSettingsService : class, ISettingsService
        where TAuthenticationService : class, IAuthenticationService
        where TResource : class
    {
        wpfAppBuilder.ConfigureServices((context, services) =>
        {
            services.AddOptions();

            // Register singleton services
            services.TryAddSingleton<ILogger>((s) => LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Trace);
            }).CreateLogger(AppDomain.CurrentDomain.FriendlyName));

            var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

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
            services.TryAddSingleton<IToastMessageService, ToastMessageService>();
            services.TryAddSingleton<IFileService<FileResult>, FileService>();

            services.TryAddSingleton<IAuthenticationService, TAuthenticationService>();
            services.TryAddSingleton<ICommonServices, TCommonServices>();

            services.RegisterAssemblies();

            action.Invoke(services, context.Configuration);

            services.BuildServiceProviderWithLocator(true);
        });

        return wpfAppBuilder;
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="services"></param>
    private static void RegisterAssemblies(this IServiceCollection services)
    {
        var viewTypes = ReflectionExtensions.GetViewTypes();
        var windowTypes = ReflectionExtensions.GetWindowTypes();
        var viewModelTypes = ReflectionExtensions.GetViewModelTypes();

        services.RegisterViewModels(viewModelTypes);
        services.RegisterViews(viewTypes);
        services.RegisterWindows(windowTypes);
    }
}
