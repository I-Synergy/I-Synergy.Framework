using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Providers;
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Security;
using ISynergy.Framework.AspNetCore.Blazor.Abstractions.Services;
using ISynergy.Framework.AspNetCore.Blazor.Options;
using ISynergy.Framework.AspNetCore.Blazor.Providers;
using ISynergy.Framework.AspNetCore.Blazor.Security;
using ISynergy.Framework.AspNetCore.Blazor.Services;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ISynergy.Framework.AspNetCore.Blazor.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> for Blazor application setup.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    /// <typeparam name="TCommonServices">The common services type.</typeparam>
    /// <typeparam name="TExceptionHandlerService">The exception handler service type.</typeparam>
    /// <typeparam name="TSettingsService">The settings service type.</typeparam>
    /// <typeparam name="TResource">The resource type for localization.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="infoService">The info service instance.</param>
    /// <param name="action">An optional action to further configure services.</param>
    /// <param name="assembly">The entry assembly used to resolve referenced assemblies.</param>
    /// <param name="assemblyFilter">An optional filter applied to referenced assembly names.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    /// <remarks>
    /// This overload calls the reflection-based <c>RegisterAssemblies</c> method, which is not
    /// AOT-compatible. Replace the <c>RegisterAssemblies</c> call with <c>AddBlazorRegistrations()</c>
    /// (emitted by the bundled source generator) for NativeAOT and trimmed publish scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("RegisterAssemblies uses runtime assembly scanning. Use AddBlazorRegistrations() for AOT scenarios.")]
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
        languageService.AddResourceManager(typeof(Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(Framework.AspNetCore.Blazor.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));
#pragma warning restore IL2026

        services.TryAddSingleton<IInfoService>(s => infoService);
        services.TryAddSingleton<ILanguageService>(s => languageService);
        services.TryAddSingleton<IMessengerService, MessengerService>();
        services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        services.TryAddSingleton<IBusyService, BusyService>();
        services.AddSingleton<IExceptionHandlerService, TExceptionHandlerService>();

        services.TryAddScoped<TContext>();
        services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());

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

    /// <summary>
    /// Registers the assemblies by scanning them at runtime for <c>IView</c>, <c>IWindow</c>,
    /// and <c>IViewModel</c> implementors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The entry assembly used to resolve referenced assemblies.</param>
    /// <param name="assemblyFilter">An optional filter applied to referenced assembly names.</param>
    /// <remarks>
    /// This overload uses runtime assembly scanning and is not AOT-compatible.
    /// Replace with the source-generator-emitted <c>AddBlazorRegistrations()</c> extension method
    /// for NativeAOT and trimmed publish scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddBlazorRegistrations() instead.")]
    [RequiresDynamicCode("Assembly scanning is not AOT-compatible. Use AddBlazorRegistrations() instead.")]
    public static void RegisterAssemblies(this IServiceCollection services, Assembly assembly, Func<AssemblyName, bool> assemblyFilter)
    {
        var referencedAssemblies = assembly.GetAllReferencedAssemblyNames();
        var assemblies = new List<Assembly>();

        if (assemblyFilter is not null)
            foreach (var item in referencedAssemblies.Where(assemblyFilter).EnsureNotNull())
                assemblies.Add(Assembly.Load(item));

        foreach (var item in referencedAssemblies.Where(x =>
            x.Name!.StartsWith("ISynergy.Framework.AspNetCore.Blazor") ||
            x.Name!.StartsWith("ISynergy.Framework.Mvvm")).EnsureNotNull())
            assemblies.Add(Assembly.Load(item));

        services.RegisterAssemblies(assemblies);
    }

    /// <summary>
    /// Registers the assemblies by scanning a set of pre-loaded assemblies for
    /// <c>IView</c>, <c>IWindow</c>, and <c>IViewModel</c> implementors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <remarks>
    /// This overload uses runtime assembly scanning and is not AOT-compatible.
    /// Replace with the source-generator-emitted <c>AddBlazorRegistrations()</c> extension method
    /// for NativeAOT and trimmed publish scenarios.
    /// </remarks>
    [RequiresUnreferencedCode("Assembly scanning is not AOT-compatible. Use AddBlazorRegistrations() instead.")]
    [RequiresDynamicCode("Assembly scanning is not AOT-compatible. Use AddBlazorRegistrations() instead.")]
    public static void RegisterAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var viewTypes = assemblies.ToViewTypes();
        var windowTypes = assemblies.ToWindowTypes();
        var viewModelTypes = assemblies.ToViewModelTypes();

        services.RegisterViewModels(viewModelTypes);
        services.RegisterViews(viewTypes);
        services.RegisterWindows(windowTypes);
    }
}
