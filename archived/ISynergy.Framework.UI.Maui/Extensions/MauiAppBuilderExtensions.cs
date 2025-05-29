using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using FileResult = ISynergy.Framework.Mvvm.Models.FileResult;

#if WINDOWS
using ISynergy.Framework.UI.Abstractions.Services;
#endif

namespace ISynergy.Framework.UI.Extensions;

public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TAuthenticationService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="appBuilder"></param>
    /// <param name="infoService"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureServices<TContext, TCommonServices, TAuthenticationService, TSettingsService, TResource>(
        this MauiAppBuilder appBuilder,
        IInfoService infoService,
        Action<MauiAppBuilder> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TSettingsService : class, ISettingsService
        where TAuthenticationService : class, IAuthenticationService
        where TResource : class
    {
        appBuilder.Services.AddOptions();

        appBuilder.Services.Configure<ApplicationFeatures>(appBuilder.Configuration.GetSection(nameof(ApplicationFeatures)).BindWithReload);
        appBuilder.Services.Configure<ApplicationOptions>(appBuilder.Configuration.GetSection(nameof(ApplicationOptions)).BindWithReload);

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
        languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));
        languageService.AddResourceManager(typeof(TResource));

        appBuilder.Services.TryAddSingleton<IInfoService>(s => infoService);
        appBuilder.Services.TryAddSingleton<ILanguageService>(s => languageService);
        appBuilder.Services.TryAddSingleton<IMessageService>(s => MessageService.Default);

        appBuilder.Services.TryAddScoped<TContext>();
        appBuilder.Services.TryAddScoped<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddScoped<ISettingsService, TSettingsService>();
        appBuilder.Services.TryAddScoped<IAuthenticationProvider, AuthenticationProvider>();
        appBuilder.Services.TryAddScoped<ICredentialLockerService, CredentialLockerService>();

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
        appBuilder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<IDialogService, DialogService>();
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();
        appBuilder.Services.TryAddSingleton<IAuthenticationService, TAuthenticationService>();
        appBuilder.Services.TryAddSingleton<ICommonServices, TCommonServices>();

#if WINDOWS
        appBuilder.Services.TryAddSingleton<IUpdateService, UpdateService>();
#endif

        appBuilder.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(appBuilder);

        appBuilder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("segoeui.ttf", "SegoeUI");
                fonts.AddFont("segoesb.ttf", "SegoeSemiBold");
                fonts.AddFont("segoeuib.ttf", "SegoeUIBold");
                fonts.AddFont("segoeuil.ttf", "SegoeUILight");
                fonts.AddFont("segoeuisl.ttf", "SegoeUISemiLight");
                fonts.AddFont("segoeuisb.ttf", "SegoeUISemiBold");
                fonts.AddFont("segoemdl2.ttf", "SegoeMdl2");
                fonts.AddFont("opensans-medium.ttf", "OpenSansMedium");
                fonts.AddFont("opensans-regular.ttf", "OpenSansRegular");
                fonts.AddFont("opensans-semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("opendyslexic3-bold.ttf", "OpenDyslexic3-Bold");
                fonts.AddFont("opendyslexic3-regular.ttf", "OpenDyslexic3-Regular");
            });

        return appBuilder;
    }

    /// <summary>
    /// Registers the assemblies.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter">The assembly filter.</param>
    private static void RegisterAssemblies(this MauiAppBuilder appBuilder, Assembly assembly, Func<AssemblyName, bool> assemblyFilter)
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

        appBuilder.Services.RegisterAssemblies(assemblies);
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

        services.RegisterViewModelRoutes(viewModelTypes, viewTypes);
    }

    private static void RegisterViewModelRoutes(this IServiceCollection services, IEnumerable<Type> viewModelTypes, IEnumerable<Type> viewTypes)
    {
        foreach (var view in viewTypes.Distinct().EnsureNotNull())
        {
            if (viewModelTypes.FirstOrDefault(q => q.Name.Equals(view.GetRelatedViewModel())) is { } viewmodel)
                services.RegisterViewModelRoute(viewmodel, view);
        }
    }

    private static void RegisterViewModelRoute(this IServiceCollection services, Type viewmodel, Type view)
    {
        var abstraction = Array.Find(viewmodel
            .GetInterfaces(), q =>
                q.GetInterfaces().Contains(typeof(IViewModel))
                && !q.Name.StartsWith(nameof(IViewModel)));

        services.RegisterRoute(viewmodel, abstraction, view);
    }

    private static void RegisterRoute(this IServiceCollection services, Type type, Type abstraction, Type view)
    {
        if (abstraction is not null)
            Routing.RegisterRoute(abstraction.Name, view);

        Routing.RegisterRoute(type.Name, view);
    }

    /// <summary>
    /// Registers the action in the service collection with the page resolver
    /// </summary>
    /// <param name="services"></param>
    public static void AddPageResolver(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, ResolverService>());
    }
}
