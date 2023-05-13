using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.Mvvm.Models;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.UI.Extensions
{
    public static class WindowsAppBuilderExtensions
    {
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
        /// <typeparam name="TExceptionHandler"></typeparam>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assemblyFilter"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureServices<TApplication, TContext, TExceptionHandler, TResource>(this IServiceCollection services, IConfiguration configuration, Func<AssemblyName, bool> assemblyFilter)
            where TApplication : Microsoft.UI.Xaml.Application
            where TContext : class, IContext
            where TExceptionHandler : class, IExceptionHandlerService
            where TResource : class
        {
            services.AddLogging();
            services.AddOptions();

            var mainAssembly = Assembly.GetAssembly(typeof(TApplication));

            services.Configure<ConfigurationOptions>(configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            var navigationService = new NavigationService();


            var infoService = InfoService.Default;
            infoService.LoadAssembly(mainAssembly);

            var languageService = LanguageService.Default;
            languageService.AddResourceManager(typeof(ISynergy.Framework.Mvvm.Properties.Resources));
            languageService.AddResourceManager(typeof(ISynergy.Framework.UI.Properties.Resources));

            // Register singleton services
            services.AddSingleton<ILogger>((s) => LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Trace);
            }).CreateLogger(AppDomain.CurrentDomain.FriendlyName));

            services.AddSingleton<IInfoService>(s => InfoService.Default);
            services.AddSingleton<ILanguageService>(s => LanguageService.Default);
            services.AddSingleton<IMessageService>(s => MessageService.Default);

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseNavigationService, NavigationService>((s) => navigationService));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<INavigationService, NavigationService>((s) => navigationService));
            
            services.AddScoped<IContext, TContext>();
            
            services.AddSingleton<IExceptionHandlerService, TExceptionHandler>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
            services.AddSingleton<IConverterService, ConverterService>();
            services.AddSingleton<IBusyService, BusyService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDispatcherService, DispatcherService>();
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddTransient<IThemeService, ThemeService>();
            services.AddTransient<IFileService<FileResult>, FileService>();

            languageService.AddResourceManager(typeof(TResource));

            services.RegisterAssemblies(mainAssembly, navigationService, assemblyFilter);

            return services;
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="mainAssembly">The main assembly.</param>
        /// <param name="navigationService"></param>
        /// <param name="assemblyFilter">The assembly filter.</param>
        private static void RegisterAssemblies(this IServiceCollection services, Assembly mainAssembly, INavigationService navigationService, Func<AssemblyName, bool> assemblyFilter)
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

            services.RegisterAssemblies(assemblies);

            ConfigureNavigationService(navigationService);
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies">The assemblies.</param>
        private static void RegisterAssemblies(this IServiceCollection services, List<Assembly> assemblies)
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
                        && (q.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(q.Name, GenericConstants.ViewModelTRegex, RegexOptions.None, TimeSpan.FromMilliseconds(100)))
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

                if (abstraction is not null && !viewmodel.IsGenericType)
                {
                    services.AddScoped(abstraction, viewmodel);
                }
                else
                {
                    services.AddScoped(viewmodel);
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
                    services.AddTransient(abstraction, view);
                }
                else
                {
                    services.AddTransient(view);
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
                    services.AddTransient(abstraction, window);
                }
                else
                {
                    services.AddTransient(window);
                }
            }

            foreach (var bootstrapper in BootstrapperTypes.Distinct())
            {
                services.AddSingleton(bootstrapper);
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
                    navigationService.Configure(viewmodel.GetViewModelFullName(), view);
            }
        }
    }
}
