using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Security;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Security;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ISynergy.Framework.UI.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TExceptionHandlerService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="infoService"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureServices<TContext, TCommonServices, TExceptionHandlerService, TSettingsService, TResource>(
        this IServiceCollection services,
        IConfiguration configuration,
        IInfoService infoService,
        Action<IServiceCollection> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TExceptionHandlerService : class, IExceptionHandlerService
        where TSettingsService : class, ISettingsService
        where TResource : class
    {
        services.AddOptions();

        services.Configure<ClientApplicationOptions>(configuration.GetSection(nameof(ClientApplicationOptions)).BindWithReload);
        services.Configure<AnalyticOptions>(configuration.GetSection(nameof(AnalyticOptions)).BindWithReload);

        var languageService = new LanguageService();
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        services.TryAddSingleton<IInfoService>(s => infoService);
        services.TryAddSingleton<ILanguageService>(s => languageService);
        services.TryAddSingleton<IMessengerService, MessengerService>();
        services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        services.TryAddSingleton<IBusyService, BusyService>();

        services.TryAddScoped<TContext>();
        services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());

        services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();

        services.TryAddScoped<ISettingsService, TSettingsService>();
        services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();

        services.TryAddSingleton<TCommonServices>();
        services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddHttpClient<IStaticAssetService, StaticAssetService>();

        services.TryAddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();

        services.TryAddSingleton<IFormFactorService, FormFactorService>();
        services.TryAddSingleton<RequestCancellationService>();

        services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(services);

        return services;
    }
}
