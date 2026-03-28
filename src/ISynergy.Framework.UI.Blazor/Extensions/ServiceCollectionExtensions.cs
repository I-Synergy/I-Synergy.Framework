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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.UI.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all services.
    /// </summary>
    /// <typeparam name="TContext">The application context type.</typeparam>
    /// <typeparam name="TCommonServices">The common services type.</typeparam>
    /// <typeparam name="TExceptionHandlerService">The exception handler service type.</typeparam>
    /// <typeparam name="TSettingsService">The settings service type.</typeparam>
    /// <typeparam name="TResource">The resource type for localization.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="infoService">The application info service.</param>
    /// <param name="action">Additional service configuration action.</param>
    /// <param name="assembly">The assembly to scan for ViewModels and Views.</param>
    /// <param name="assemblyFilter">A filter function for assembly names.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    /// <remarks>
    /// This overload calls <c>RegisterAssemblies</c> which performs runtime assembly scanning.
    /// This is not compatible with Blazor WASM AOT or IL trimming.
    /// Use <c>services.AddUITypes()</c> from <c>ISynergy.Framework.UI.SourceGenerator</c> for AOT-compatible registration.
    /// </remarks>
    [RequiresUnreferencedCode(
        "RegisterAssemblies performs runtime assembly scanning which is not trim-safe. " +
        "Use AddUITypes() from ISynergy.Framework.UI.SourceGenerator for AOT-compatible registration.")]
    [RequiresDynamicCode(
        "BindWithReload uses dynamic code generation which is not AOT-compatible. " +
        "Use AddUITypes() from ISynergy.Framework.UI.SourceGenerator for AOT-compatible registration.")]
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

#pragma warning disable IL2026 // typeof() arguments are statically known; resource types are preserved at compile time
        var languageService = new LanguageService();
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));
#pragma warning restore IL2026

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
