using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace ISynergy.Framework.UI.Blazor.Extensions;

public static class BlazorAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TAuthenticationService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="builder"></param>
    /// <param name="infoService"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder ConfigureServices<TContext, TCommonServices, TAuthenticationService, TSettingsService, TResource>(
        this IHostApplicationBuilder builder,
        IInfoService infoService,
        Action<IHostApplicationBuilder> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TSettingsService : class, ISettingsService
        where TAuthenticationService : class, IAuthenticationService
        where TResource : class
    {
        builder.Services.AddOptions();

        builder.Services.Configure<ApplicationFeatures>(builder.Configuration.GetSection(nameof(ApplicationFeatures)).BindWithReload);
        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)).BindWithReload);

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        builder.Services.TryAddSingleton<IInfoService>(s => infoService);
        builder.Services.TryAddSingleton<ILanguageService>(s => languageService);
        builder.Services.TryAddSingleton<IMessageService>(s => MessageService.Default);

        builder.Services.TryAddScoped<TContext>();
        builder.Services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());

        builder.Services.TryAddScoped<ISettingsService, TSettingsService>();
        builder.Services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();

        builder.Services.TryAddSingleton<Microsoft.FluentUI.AspNetCore.Components.DialogService>();
        builder.Services.TryAddSingleton<Microsoft.FluentUI.AspNetCore.Components.IDialogService>(s => s.GetRequiredService<Microsoft.FluentUI.AspNetCore.Components.DialogService>());

        builder.Services.TryAddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
        builder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        builder.Services.TryAddSingleton<INavigationService, NavigationService>();
        builder.Services.TryAddSingleton<IBusyService, BusyService>();
        builder.Services.TryAddSingleton<IDialogService, DialogService>();
        builder.Services.TryAddSingleton<IAuthenticationService, TAuthenticationService>();
        builder.Services.TryAddSingleton<ICommonServices, TCommonServices>();

        builder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        builder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(builder);

        return builder;
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter">The assembly filter.</param>
    private static void RegisterAssemblies(this IServiceCollection services, Assembly assembly, Func<AssemblyName, bool> assemblyFilter)
    {
        var referencedAssemblies = assembly.GetAllReferencedAssemblyNames();
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
}
