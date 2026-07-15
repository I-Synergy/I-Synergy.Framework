using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Options;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Globalization.Abstractions.Services;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Globalization.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using Windows.ApplicationModel.Activation;

namespace ISynergy.Framework.UI.Extensions;

public static class WindowsAppBuilderExtensions
{
    /// <summary>
    /// Returns an instance of the <see cref="IServiceCollection"/> and configures all appBuilder.
    /// </summary>
    /// <typeparam name="TApplication"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TCommonServices"></typeparam>
    /// <typeparam name="TExceptionHandlerService"></typeparam>
    /// <typeparam name="TSettingsService"></typeparam>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="appBuilder"></param>
    /// <param name="action"></param>
    /// <param name="assembly"></param>
    /// <param name="assemblyFilter"></param>
    /// <returns></returns>
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "View, ViewModel and Window types are rooted by the application XAML compilation (XamlTypeInfo) and direct references; RegisterAssemblies only rediscovers types the application already preserves. typeof(TResource) is statically known, so its ResourceManager is preserved at compile time.")]
    public static IHostApplicationBuilder ConfigureServices<
        TApplication,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TContext,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TCommonServices,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TExceptionHandlerService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSettingsService,
        TResource>(
        this IHostApplicationBuilder appBuilder,
        Action<IHostApplicationBuilder> action,
        Assembly assembly,
        Func<AssemblyName, bool> assemblyFilter)
        where TApplication : Application
        where TContext : class, IContext
        where TCommonServices : class, ICommonServices
        where TExceptionHandlerService : class, IExceptionHandlerService
        where TSettingsService : class, ISettingsService
        where TResource : class
    {
        appBuilder.Services.AddOptions();

        // Manual, trim-safe binding via the IConfigurationSection string indexer (case-insensitive,
        // same as ConfigurationBinder.Bind). NOTE: any property added to these option types later
        // must be bound explicitly here.
        var featuresSection = appBuilder.Configuration.GetSection(nameof(ApplicationFeatures));
        appBuilder.Services.Configure<ApplicationFeatures>(options =>
            options.CheckForUpdatesInMicrosoftStore =
                bool.TryParse(featuresSection[nameof(ApplicationFeatures.CheckForUpdatesInMicrosoftStore)], out var check) && check);

        var clientSection = appBuilder.Configuration.GetSection(nameof(ClientApplicationOptions));
        appBuilder.Services.Configure<ClientApplicationOptions>(options =>
        {
            options.ClientId = clientSection[nameof(ClientApplicationOptions.ClientId)];
            options.ClientSecret = clientSection[nameof(ClientApplicationOptions.ClientSecret)];
            options.Endpoint = clientSection[nameof(ClientApplicationOptions.Endpoint)];
        });

        var mainAssembly = Assembly.GetAssembly(typeof(TApplication));
        if (mainAssembly is not null)
        {
            var infoService = InfoService.Default;
            infoService.LoadAssembly(mainAssembly);
        }

        var languageService = LanguageService.Default;
        languageService.AddResourceManager(typeof(TResource));

        appBuilder.Services.TryAddSingleton<IInfoService>(s => InfoService.Default);
        appBuilder.Services.TryAddSingleton<ILanguageService>(s => languageService);

        // Fallback: English-only if ConfigureSupportedCultures was not called
        appBuilder.Services.TryAddSingleton<ISupportedCulturesService>(s =>
            new SupportedCulturesService([CultureInfo.GetCultureInfo(GlobalizationConstants.DefaultCultureInfoName)]));

        appBuilder.Services.TryAddSingleton<IMessengerService, MessengerService>();

        appBuilder.Services.TryAddSingleton<TContext>();
        appBuilder.Services.TryAddSingleton<IContext>(s => s.GetRequiredService<TContext>());

        appBuilder.Services.TryAddSingleton<TCommonServices>();
        appBuilder.Services.TryAddSingleton<ICommonServices>(s => s.GetRequiredService<TCommonServices>());

        appBuilder.Services.TryAddSingleton<IExceptionHandlerService, TExceptionHandlerService>();

        appBuilder.Services.TryAddSingleton<ISettingsService, TSettingsService>();
        appBuilder.Services.TryAddSingleton<ILocalSettings>((s) => s.GetRequiredService<ISettingsService>().LocalSettings);
        appBuilder.Services.TryAddSingleton<IRoamingSettings>((s) => s.GetRequiredService<ISettingsService>().RoamingSettings);
        appBuilder.Services.TryAddSingleton<IGlobalSettings>((s) => s.GetRequiredService<ISettingsService>().GlobalSettings);

        appBuilder.Services.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();
        appBuilder.Services.TryAddSingleton<IMigrationService, MigrationService>();
        appBuilder.Services.TryAddSingleton<IThemeService, ThemeService>();

        appBuilder.Services.TryAddSingleton<ITokenStorageService, TokenStorageService>();
        appBuilder.Services.TryAddSingleton<IApplicationLifecycleService, ApplicationLifecycleService>();
        appBuilder.Services.TryAddSingleton<IDispatcherService, DispatcherService>();
        appBuilder.Services.TryAddSingleton<INavigationService, NavigationService>();
        appBuilder.Services.TryAddSingleton<IBladeService, BladeService>();
        appBuilder.Services.TryAddSingleton<IBusyService, BusyService>();
        appBuilder.Services.TryAddSingleton<DialogService>();
        appBuilder.Services.TryAddSingleton<IDialogService>(sp => sp.GetRequiredService<DialogService>());
        appBuilder.Services.TryAddSingleton<IClipboardService, ClipboardService>();
        appBuilder.Services.TryAddSingleton<IFileService<FileResult>, FileService>();

        appBuilder.Services.RegisterAssemblies(assembly, assemblyFilter);

        action.Invoke(appBuilder);

        return appBuilder;
    }

    /// <summary>
    /// Registers callbacks that are invoked when a second instance redirects its
    /// activation (protocol URI, command-line args, etc.) to this instance.
    /// <para>
    /// Separate from single-instance enforcement (handled by WindowsAppSdkHost).
    /// This method only handles WHAT to do with the activation arguments.
    /// </para>
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="defaultAction">Called with command-line arguments (fallback).</param>
    /// <param name="protocolAction">Called for protocol URI activations (myapp://...).</param>
    /// <param name="launchAction">Called for standard launch activations.</param>
    public static IHostApplicationBuilder ConfigureActivationHandlers(
        this IHostApplicationBuilder builder,
        Action<string[]> defaultAction,
        Action<ProtocolActivatedEventArgs> protocolAction,
        Action<LaunchActivatedEventArgs> launchAction)
    {
        AppActivationExtensions.SetCallbacks(defaultAction, protocolAction, launchAction);
        return builder;
    }

    /// <summary>
    /// Enables single-instance behavior. When a second instance is launched, its
    /// activation is redirected to the first instance and the second process exits.
    /// <para>
    /// Without this call, multiple instances can run simultaneously.
    /// </para>
    /// </summary>
    public static IHostApplicationBuilder ConfigureSingleInstance(this IHostApplicationBuilder builder)
    {
        AppActivationExtensions.IsSingleInstanceEnabled = true;
        return builder;
    }

    /// <summary>
    /// Configures the list of cultures the application supports.
    /// English ("en") is always included automatically and serves as the default.
    /// </summary>
    /// <param name="appBuilder">The host application builder.</param>
    /// <param name="configure">A delegate that adds cultures using the builder.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/>.</returns>
    public static IHostApplicationBuilder ConfigureSupportedCultures(
        this IHostApplicationBuilder appBuilder,
        Action<ISupportedCulturesBuilder> configure)
    {
        var builder = new SupportedCulturesBuilder();
        configure(builder);
        appBuilder.Services.Replace(ServiceDescriptor.Singleton<ISupportedCulturesService>(
            s => new SupportedCulturesService(builder.Build())));
        return appBuilder;
    }

    /// <summary>
    /// Registers the available theme colors that users can select in the application.
    /// </summary>
    /// <param name="appBuilder">The host application builder.</param>
    /// <param name="configure">A delegate to populate the list of available color hex strings.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/>.</returns>
    public static IHostApplicationBuilder UseThemeColors(
        this IHostApplicationBuilder appBuilder,
        Action<IList<string>>? configure = null)
    {
        appBuilder.Services.Configure<ThemeColorOptions>(options =>
            configure?.Invoke(options.AvailableColors));
        return appBuilder;
    }
}
