using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Assembly;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class BaseApplication.
    /// </summary>
    public abstract partial class BaseApplication
    {
        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        private IThemeService _themeSelector;
        
        /// <summary>
        /// The services
        /// </summary>
        private readonly IServiceCollection _services;

        /// <summary>
        /// The navigation service
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// The service provider
        /// </summary>
        protected readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected readonly ILogger _logger;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected IContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplication" /> class.
        /// </summary>
        protected BaseApplication()
        {
            _services = new ServiceCollection();
            _navigationService = new NavigationService();
            _languageService = new LanguageService();

            ConfigureServices(_services);

            _serviceProvider = _services.BuildServiceProvider();

            ServiceLocator.SetLocatorProvider(_serviceProvider);

            _logger = _serviceProvider.GetRequiredService<ILogger>();
            _logger.LogInformation("Starting application");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Initialize();
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs" /> instance containing the event data.</param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception, e.Exception.Message);
            e.SetObserved();
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception, exception.Message);
            }
        }

        /// <summary>
        /// Configures the logger.
        /// </summary>
        protected virtual ILoggerFactory ConfigureLogger(LogLevel loglevel = LogLevel.Information)
        {
            var factory =  LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif WINDOWS_UWP || WINDOWS
                builder.AddDebug();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(loglevel);

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace);

                // Layouter specific messages
                builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug);

                builder.AddFilter("Windows.Storage", LogLevel.Debug);

                // Binding related messages
                builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);

                // Binder memory references tracking
                builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug);

                // RemoteControl and HotReload related
                builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug);
            });

#if HAS_UNO
            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
#endif
            return factory;
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            // Register singleton services
            services.AddSingleton((s) => _languageService);
            services.AddSingleton((s) => _navigationService);
            services.AddSingleton<ILogger>((s) => ConfigureLogger().CreateLogger(AppDomain.CurrentDomain.FriendlyName));
            services.AddSingleton<IMessageService, MessageService>();
            
            // Register scoped services
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();
            services.AddScoped<IConverterService, ConverterService>();
            services.AddScoped<IBusyService, BusyService>();
            services.AddScoped<IDialogService, DialogService>();
            services.AddScoped<IDispatcherService, DispatcherService>();
            services.AddScoped<IFileService, FileService>();

            //Register functions
            services.AddScoped<LocalizationFunctions>();
        }

        /// <summary>
        /// Gets the view model types.
        /// </summary>
        /// <value>The view model types.</value>
        public List<Type> ViewModelTypes { get; private set; }
        
        /// <summary>
        /// Gets the view types.
        /// </summary>
        /// <value>The view types.</value>
        public List<Type> ViewTypes { get; private set; }

        /// <summary>
        /// Gets the window types.
        /// </summary>
        /// <value>The window types.</value>
        public List<Type> WindowTypes { get; private set; }

        /// <summary>
        /// Bootstrapper types
        /// </summary>
        public List<Type> BootstrapperTypes { get; private set; }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="mainAssembly"></param>
        /// <param name="assemblyFilter"></param>
        protected void RegisterAssemblies(Assembly mainAssembly, Func<AssemblyName, bool> assemblyFilter)
        {
            var assemblies = new List<Assembly>();
            assemblies.Add(mainAssembly);

            foreach (var item in mainAssembly.GetReferencedAssemblies().Where(assemblyFilter))
                assemblies.Add(Assembly.Load(item));

            foreach (var item in mainAssembly.GetReferencedAssemblies().Where(x => 
                x.Name.StartsWith("ISynergy.Framework.UI") ||
                x.Name.StartsWith("ISynergy.Framework.Mvvm")))
                assemblies.Add(Assembly.Load(item));

            RegisterAssemblies(assemblies);
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        protected void RegisterAssemblies(List<Assembly> assemblies)
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
                        q.GetInterface(nameof(IView), false) is not null && (
                        q.Name.EndsWith(GenericConstants.View)
                        || q.Name.EndsWith(GenericConstants.Page))
                        && q.Name != GenericConstants.View
                        && q.Name != nameof(ISynergy.Framework.UI.Controls.View)
                        && q.Name != GenericConstants.Page
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                WindowTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IWindow), false) is not null
                        && q.Name.EndsWith(GenericConstants.Window)
                        && q.Name != GenericConstants.Window
                        && q.Name != nameof(ISynergy.Framework.UI.Controls.Window)
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
                    _services.AddScoped(abstraction, viewmodel);
                }
                else
                {
                    _services.AddScoped(viewmodel);
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
                    _services.AddScoped(abstraction, view);
                }
                else
                {
                    _services.AddScoped(view);
                }

                var viewmodel = ViewModelTypes.Find(q =>
                {
                    var name = view.Name.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel);
                    return q.GetViewModelName().Equals(name) || q.Name.Equals(name);
                });

                if (viewmodel is not null)
                {
                    _navigationService.Configure(viewmodel.GetViewModelFullName(), view);
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
                    _services.AddScoped(abstraction, window);
                }
                else
                {
                    _services.AddScoped(window);
                }
            }

            foreach (var bootstrapper in BootstrapperTypes.Distinct())
            {
                _services.AddScoped(bootstrapper);
            }
        }

        /// <summary>
        /// Add resource managers to languageservice.
        /// </summary>
        public virtual void AddResourceManager(ResourceManager resourceManager) =>
            _languageService.AddResourceManager(resourceManager);

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public abstract void HandleException(Exception exception, string message);
    }
}