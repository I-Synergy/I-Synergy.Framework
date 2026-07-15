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
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Globalization.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "View, ViewModel and Window types are rooted by the application's Razor compilation and direct references; RegisterAssemblies only rediscovers types the application already preserves. typeof(TResource) is statically known, so its ResourceManager is preserved at compile time.")]
    public static IServiceCollection ConfigureServices<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TContext,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCommonServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TExceptionHandlerService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSettingsService,
        TResource>( // NOSONAR
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

        // Manual, trim-safe binding via the IConfigurationSection string indexer (case-insensitive,
        // same as ConfigurationBinder.Bind). NOTE: any property added to these option types later
        // must be bound explicitly here.
        var clientSection = configuration.GetSection(nameof(ClientApplicationOptions));
        services.Configure<ClientApplicationOptions>(options =>
        {
            options.ClientId = clientSection[nameof(ClientApplicationOptions.ClientId)];
            options.ClientSecret = clientSection[nameof(ClientApplicationOptions.ClientSecret)];
            options.Endpoint = clientSection[nameof(ClientApplicationOptions.Endpoint)];
        });

        var analyticSection = configuration.GetSection(nameof(AnalyticOptions));
        services.Configure<AnalyticOptions>(options =>
        {
            options.GoogleAnalyticsMeasurementId = analyticSection[nameof(AnalyticOptions.GoogleAnalyticsMeasurementId)];
            options.MicrosoftClarityProjectId = analyticSection[nameof(AnalyticOptions.MicrosoftClarityProjectId)];
        });

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(TResource));

        services.TryAddSingleton<IInfoService>(s => infoService);
        services.TryAddSingleton<ILanguageService>(s => languageService);

        // Fallback: English-only if ConfigureSupportedCultures was not called
        services.TryAddSingleton<ISupportedCulturesService>(s =>
            new SupportedCulturesService([CultureInfo.GetCultureInfo(GlobalizationConstants.DefaultCultureInfoName)]));

        services.TryAddSingleton<IMessengerService, MessengerService>();
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
    [RequiresUnreferencedCode("Discovers and loads referenced assemblies at runtime and scans them for IView/IWindow/IViewModel implementors; not trim-compatible. Types are only found if the application still references them.")]
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
    [RequiresUnreferencedCode("Scans the supplied assemblies for IView/IWindow/IViewModel implementors; not trim-compatible.")]
    public static void RegisterAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var viewTypes = assemblies.ToViewTypes();
        var windowTypes = assemblies.ToWindowTypes();
        var viewModelTypes = assemblies.ToViewModelTypes();

        services.RegisterViewModels(viewModelTypes);
        services.RegisterViews(viewTypes);
        services.RegisterWindows(windowTypes);
    }

    /// <summary>
    /// Configures the list of cultures the application supports.
    /// English ("en") is always included automatically and serves as the default.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">A delegate that adds cultures using the builder.</param>
    /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection ConfigureSupportedCultures(
        this IServiceCollection services,
        Action<ISupportedCulturesBuilder> configure)
    {
        var builder = new SupportedCulturesBuilder();
        configure(builder);
        services.Replace(ServiceDescriptor.Singleton<ISupportedCulturesService>(
            s => new SupportedCulturesService(builder.Build())));
        return services;
    }
}
