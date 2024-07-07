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

public static class WindowsAppSdkHostBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and adds loggingBuilder.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="loggingBuilder"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureLogging<TApplication>(this IHostBuilder appBuilder, Action<ILoggingBuilder, IConfiguration> loggingBuilder)
        where TApplication : Microsoft.UI.Xaml.Application, new()
    {
        appBuilder.ConfigureLogging((context, logger) =>
        {
#if DEBUG
            logger.AddDebug();
#endif
            logger.SetMinimumLevel(LogLevel.Trace);

            loggingBuilder.Invoke(logger, context.Configuration);
        });

        return appBuilder;
    }

    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TExceptionHandler"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="appBuilder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureServices<TApplication, TContext, TExceptionHandler, TResource>(this IHostBuilder appBuilder, Action<IServiceCollection> action)
        where TApplication : Microsoft.UI.Xaml.Application, new()
        where TContext : class, IContext
        where TExceptionHandler : class, IExceptionHandlerService
        where TResource : class
    {
        appBuilder.ConfigureServices((context, services) =>
        {
            services.AddLogging();
            services.AddOptions();

            //// Register singleton windowsAppBuilder
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

            services.RegisterAssemblies();

            action.Invoke(services);

            ServiceLocator.SetLocatorProvider(services.BuildServiceProvider());
        });

        return appBuilder;
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
