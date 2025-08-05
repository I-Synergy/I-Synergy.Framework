using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Security;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Security;
using ISynergy.Framework.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Reflection;
using DialogService = ISynergy.Framework.UI.Services.DialogService;
using FileResult = ISynergy.Framework.Mvvm.Models.FileResult;
using IDialogService = ISynergy.Framework.Mvvm.Abstractions.Services.IDialogService;
using IMessageService = ISynergy.Framework.Core.Abstractions.Services.IMessageService;
using MessageService = ISynergy.Framework.Core.Services.MessageService;

namespace ISynergy.Framework.UI.Extensions;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all windowsAppBuilder.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="builder"></param>
    /// <param name="infoService"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    public static MauiAppBuilder ConfigureServices<TContext, TCommonServices, TSettingsService, TResource>(
        this MauiAppBuilder builder,
        IInfoService infoService,
        Action<MauiAppBuilder> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TSettingsService : class, ISettingsService
        where TResource : class
    {
        builder.Services.AddOptions();

        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)).BindWithReload);
        builder.Services.Configure<AnalyticOptions>(builder.Configuration.GetSection(nameof(AnalyticOptions)).BindWithReload);

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

        builder.Services.AddFluentUIComponents();

        builder.Services.TryAddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
        builder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        builder.Services.TryAddSingleton<INavigationService, NavigationService>();
        builder.Services.TryAddSingleton<IBusyService, BusyService>();
        builder.Services.TryAddSingleton<IDialogService, DialogService>();

        builder.Services.TryAddSingleton<TCommonServices>();
        builder.Services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

        builder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        builder.Services.AddAuthorizationCore();

        builder.Services.TryAddTransient<IAntiforgeryHttpClientFactory, AntiforgeryHttpClientFactory>();

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.TryAddScoped<MauiAuthenticationStateProvider>();
        builder.Services.TryAddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<MauiAuthenticationStateProvider>());

        builder.Services.AddSingleton<IFormFactorService, FormFactorService>();

        builder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(builder);

        return builder;
    }
}
