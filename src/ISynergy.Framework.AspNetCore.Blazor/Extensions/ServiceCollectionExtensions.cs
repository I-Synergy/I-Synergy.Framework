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
using Sample.Services;
using System.Reflection;

namespace ISynergy.Framework.AspNetCore.Blazor.Extensions;

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
        languageService.AddResourceManager(typeof(Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(Framework.AspNetCore.Blazor.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        services.TryAddSingleton<IInfoService>(s => infoService);
        services.TryAddSingleton<ILanguageService>(s => languageService);
        services.TryAddSingleton<IMessengerService, MessengerService>();
        services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        services.TryAddSingleton<IBusyService, BusyService>();
        services.AddSingleton<IExceptionHandlerService, ExceptionHandlerService>();

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
    /// Registers the assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter">The assembly filter.</param>
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
    /// Registers the assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
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
