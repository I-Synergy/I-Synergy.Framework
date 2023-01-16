using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.UI.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.Extensions
{
    public static class MauiAppBuilderExtensions
    {
        private const string _appSetting = "appsettings.json";

        /// <summary>
        /// Gets the shellView model types.
        /// </summary>
        /// <value>The shellView model types.</value>
        public static List<Type> ViewModelTypes { get; private set; }

        /// <summary>
        /// Gets the shellView types.
        /// </summary>
        /// <value>The shellView types.</value>
        public static List<Type> ViewTypes { get; private set; }

        /// <summary>
        /// Gets the window types.
        /// </summary>
        /// <value>The window types.</value>
        public static List<Type> WindowTypes { get; private set; }

        /// <summary>
        /// Bootstrapper types
        /// </summary>
        /// <value>The bootstrapper types.</value>
        public static List<Type> BootstrapperTypes { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="appBuilder"></param>
        /// <param name="assemblyFilter"></param>
        /// <returns></returns>
        public static MauiAppBuilder ConfigureServices<TApplication, TContext, TResource>(this MauiAppBuilder appBuilder, Func<AssemblyName, bool> assemblyFilter)
            where TApplication : class, Microsoft.Maui.IApplication
            where TContext : class, IContext
            where TResource : class
        {
            appBuilder.Services.AddLogging();
            appBuilder.Services.AddOptions();
            appBuilder.Services.AddPageResolver();

            var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.{_appSetting}"))
                .Build();

            appBuilder.Services.Configure<ConfigurationOptions>(configurationRoot.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            var navigationService = new NavigationService();
            var languageService = new LanguageService();

            // Register singleton services
            appBuilder.Services.AddSingleton<ILogger>((s) => ConfigureLogger().CreateLogger(AppDomain.CurrentDomain.FriendlyName));
            appBuilder.Services.AddSingleton<IVersionService>((s) => new VersionService(mainAssembly));
            appBuilder.Services.AddSingleton<IInfoService>((s) => new InfoService(mainAssembly));
            appBuilder.Services.AddSingleton<ILanguageService>((s) => languageService);
            appBuilder.Services.AddSingleton<INavigationService>((s) => navigationService);
            appBuilder.Services.AddSingleton<IContext, TContext>();
            appBuilder.Services.AddSingleton<IExceptionHandlerService, BaseExceptionHandlerService>();
            appBuilder.Services.AddTransient<IThemeService, ThemeService>();
            appBuilder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            appBuilder.Services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
            appBuilder.Services.AddSingleton<IConverterService, ConverterService>();
            appBuilder.Services.AddSingleton<IBusyService, BusyService>();
            appBuilder.Services.AddSingleton<IDialogService, DialogService>();
            appBuilder.Services.AddSingleton<IDispatcherService, DispatcherService>();
            appBuilder.Services.AddSingleton<IClipboardService, ClipboardService>();
            appBuilder.Services.AddSingleton<IFileService<FileResult>, FileService>();
            appBuilder.Services.AddSingleton<IPopupNavigation>((s) => MopupService.Instance);

            languageService.AddResourceManager(typeof(TResource));

            appBuilder.RegisterAssemblies(mainAssembly, navigationService);

            return appBuilder
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMarkup()
                .ConfigureMopups();
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="mainAssembly">The main assembly.</param>
        /// <param name="navigationService"></param>
        private static void RegisterAssemblies(this MauiAppBuilder appBuilder, Assembly mainAssembly, INavigationService navigationService) => appBuilder.RegisterAssemblies(mainAssembly, navigationService, null);

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="mainAssembly">The main assembly.</param>
        /// <param name="navigationService"></param>
        /// <param name="assemblyFilter">The assembly filter.</param>
        private static void RegisterAssemblies(this MauiAppBuilder appBuilder, Assembly mainAssembly, INavigationService navigationService, Func<AssemblyName, bool> assemblyFilter)
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(mainAssembly);

            if (assemblyFilter is not null)
                foreach (var item in mainAssembly.GetReferencedAssemblies().Where(assemblyFilter).EnsureNotNull())
                    assemblies.Add(Assembly.Load(item));

            foreach (var item in mainAssembly.GetReferencedAssemblies().Where(x =>
                x.Name.StartsWith("ISynergy.Framework.UI") ||
                x.Name.StartsWith("ISynergy.Framework.Mvvm")))
                assemblies.Add(Assembly.Load(item));

            appBuilder.RegisterAssemblies(assemblies);

            ConfigureNavigationService(navigationService);
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="assemblies">The assemblies.</param>
        private static void RegisterAssemblies(this MauiAppBuilder appBuilder, List<Assembly> assemblies)
        {
            ViewTypes = new List<Type>();
            WindowTypes = new List<Type>();
            ViewModelTypes = new List<Type>();
            BootstrapperTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                ViewModelTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IViewModel), false) is not null
                        && !q.Name.Equals(GenericConstants.ShellViewModel)
                        && (q.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(q.Name, GenericConstants.ViewModelTRegex))
                        && q.Name != GenericConstants.ViewModel
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                ViewTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterfaces().Any(a => a != null && a.FullName != null && a.FullName.Equals(typeof(IView).FullName))
                        && !q.Name.Equals(GenericConstants.ShellView)
                        && (q.Name.EndsWith(GenericConstants.View) || q.Name.EndsWith(GenericConstants.Page))
                        && q.Name != GenericConstants.View
                        && q.Name != GenericConstants.Page
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                WindowTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterfaces().Any(a => a != null && a.FullName != null && a.FullName.Equals(typeof(IWindow).FullName))
                        && q.Name.EndsWith(GenericConstants.Window)
                        && q.Name != GenericConstants.Window
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                BootstrapperTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IBootstrap), false) is not null
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());
            }

            foreach (var viewmodel in ViewModelTypes.Distinct())
            {
                var abstraction = viewmodel.GetInterfaces(false).FirstOrDefault();

                if (abstraction is not null && !viewmodel.IsGenericType && abstraction != typeof(IQueryAttributable))
                {
                    appBuilder.Services.AddSingleton(abstraction, viewmodel);
                }
                else
                {
                    appBuilder.Services.AddSingleton(viewmodel);
                }
            }

            foreach (var view in ViewTypes.Distinct())
            {
                var abstraction = view
                    .GetInterfaces()
                    .FirstOrDefault(q =>
                        q.GetInterfaces().Contains(typeof(IView))
                        && q.Name != nameof(IView));

                if (abstraction is not null)
                {
                    appBuilder.Services.AddSingleton(abstraction, view);
                }
                else
                {
                    appBuilder.Services.AddSingleton(view);
                }
            }

            foreach (var window in WindowTypes.Distinct())
            {
                var abstraction = window
                    .GetInterfaces()
                    .FirstOrDefault(q =>
                        q.GetInterfaces().Contains(typeof(IWindow))
                        && q.Name != nameof(IWindow));

                if (abstraction is not null)
                {
                    appBuilder.Services.AddTransient(abstraction, window);
                }
                else
                {
                    appBuilder.Services.AddTransient(window);
                }
            }

            foreach (var bootstrapper in BootstrapperTypes.Distinct())
            {
                appBuilder.Services.AddSingleton(bootstrapper);
            }
        }

        public static void ConfigureNavigationService(INavigationService navigationService)
        {
            foreach (var view in ViewTypes.Distinct())
            {
                var viewmodel = ViewModelTypes.Find(q =>
                {
                    var name = view.Name.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel);
                    return q.GetViewModelName().Equals(name) || q.Name.Equals(name);
                });

                if (viewmodel is not null)
                    Routing.RegisterRoute(viewmodel.GetViewModelFullName(), view);
            }
        }

        /// <summary>
        /// Configures the logger.
        /// </summary>
        private static ILoggerFactory ConfigureLogger(LogLevel loglevel = LogLevel.Information)
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(loglevel);
            });

            return factory;
        }

        /// <summary>
        /// Registers the services in the service collection with the page resolver
        /// </summary>
        /// <param name="services"></param>
        public static void AddPageResolver(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, ResolverService>());
        }
    }
}
