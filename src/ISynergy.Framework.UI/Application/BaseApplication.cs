using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Windows.ApplicationModel.Activation;
using Application = Microsoft.UI.Xaml.Application;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

#if WINDOWS10_0_18362_0_OR_GREATER
using Microsoft.Windows.AppLifecycle;
#endif


namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class BaseApplication.
    /// </summary>
    public abstract class BaseApplication : Application
    {
        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        private readonly IThemeService _themeService;

        /// <summary>
        /// The settings service
        /// </summary>
        private readonly IBaseApplicationSettingsService _settingsService;

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
        /// The exception handler service
        /// </summary>
        protected readonly IExceptionHandlerService _exceptionHandlerService;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected IContext _context;

        /// <summary>
        /// Main Application Window.
        /// </summary>
        /// <value>The main window.</value>
        public Window MainWindow { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplication" /> class.
        /// </summary>
        protected BaseApplication()
        {
            _logger = ConfigureLogger().CreateLogger(AppDomain.CurrentDomain.FriendlyName);

            Application.Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            _services = new ServiceCollection();

            _logger.LogDebug("Registering services for dependency injection.");
            
            ConfigureServices(_services);

            _serviceProvider = _services.BuildServiceProvider();

            _logger.LogDebug("Attach IServiceProvider to ServiceLocator.");
            
            ServiceLocator.SetLocatorProvider(_serviceProvider);

            _languageService = new LanguageService();

            _logger.LogDebug("Add language resource dictionaries.");
            
            AddResourceManager(_languageService);

            _navigationService = new NavigationService();

            _logger.LogDebug("Configure NavigationService view-viewmodel mapping.");
            
            ConfigureNavigationService(_navigationService);

            _settingsService = _serviceProvider.GetRequiredService<IBaseApplicationSettingsService>();

            _logger.LogInformation("Loading application settings...");
            
            _settingsService.LoadSettingsAsync();

            var localizationFunctions = _serviceProvider.GetRequiredService<ILocalizationService>();
            localizationFunctions.SetLocalizationLanguage(_settingsService.Settings.Culture);

            _themeService = _serviceProvider.GetRequiredService<IThemeService>();
            _exceptionHandlerService = _serviceProvider.GetRequiredService<IExceptionHandlerService>();
            _logger.LogInformation("Starting application...");

            _context = _serviceProvider.GetRequiredService<IContext>();
            _context.ViewModels = ViewModelTypes;

            // Bootstrap all registered modules.
            foreach (var bootstrapper in BootstrapperTypes.Distinct())
                if (_serviceProvider.GetService(bootstrapper) is IBootstrap instance)
                    instance.Bootstrap();
        }

        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        protected virtual void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;

                if (_exceptionHandlerService is not null)
                    _exceptionHandlerService.HandleExceptionAsync(e.Exception).Await();
                else
                    _logger.LogCritical(e.Exception, e.Message);
            }
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs" /> instance containing the event data.</param>
        protected virtual void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (_exceptionHandlerService is not null)
                _exceptionHandlerService.HandleExceptionAsync(e.Exception).Await();
            else
                _logger.LogCritical(e.Exception, e.Exception.Message);

            e.SetObserved();
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        protected virtual void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                if (_exceptionHandlerService is not null)
                    _exceptionHandlerService.HandleExceptionAsync(exception).Await();
                else
                    _logger.LogCritical(exception, exception.Message);
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            MainWindow = new Window();
            //MainWindow.Activated += OnActivated;

            _themeService.InitializeMainWindow(MainWindow);
            _themeService.SetStyle(_settingsService.Settings.Color, _settingsService.Settings.Theme);
            _themeService.SetTitlebar();

            var rootFrame = MainWindow.Content as Frame;

            if (rootFrame is null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Add custom resourcedictionaries from code.
                var dictionary = Application.Current.Resources?.MergedDictionaries;

                if (dictionary is not null)
                {
                    foreach (var item in GetAdditionalResourceDictionaries())
                    {
                        if (!dictionary.Any(t => t.Source == item.Source))
                            Application.Current.Resources.MergedDictionaries.Add(item);
                    }
                }

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;
            }

            var shellView = _serviceProvider.GetRequiredService<IShellView>();
            var shellViewModel = _serviceProvider.GetRequiredService<IShellViewModel>();
            var context = _serviceProvider.GetRequiredService<IContext>();

            Argument.IsNotNull(context);
            Argument.IsNotNull(shellView);

#if WINDOWS10_0_18362_0_OR_GREATER
            var args = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (args.Kind == ExtendedActivationKind.Protocol &&
                args.Data is IProtocolActivatedEventArgs protocolActivatedEventArgs &&
                protocolActivatedEventArgs.Uri != null &&
                HttpUtility.ParseQueryString(protocolActivatedEventArgs.Uri.Query) is NameValueCollection query &&
                query.HasKeys() &&
                !string.IsNullOrEmpty(query["environment"]))
            {
                if (Enum.TryParse<SoftwareEnvironments>(query["environment"].ToCapitalized(), out SoftwareEnvironments environment))
                    context.Environment = environment;
            }
#endif
            
            if (rootFrame.Content is null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(shellView.GetType());
            }

            MainWindow.Title = context.Title;
            //MainWindow.Content = (UIElement)shellView;
            MainWindow.Activate();
        }

        /// <summary>
        /// Get a new list of additional resource dictionaries which can be merged.
        /// </summary>
        /// <returns>IList&lt;ResourceDictionary&gt;.</returns>
        protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>();

        protected virtual void OnActivated(object sender, WindowActivatedEventArgs args)
        {
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        /// <exception cref="ISynergy.Framework.UI.Common.Result.Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="loglevel">The loglevel.</param>
        /// <returns>ILoggerFactory.</returns>
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
            services.AddSingleton<ILogger>((s) => _logger);
            services.AddSingleton<IExceptionHandlerService, BaseExceptionHandlerService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
            services.AddSingleton<IConverterService, ConverterService>();
            services.AddSingleton<IBusyService, BusyService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IDispatcherService, DispatcherService>();
            services.AddSingleton<IClipboardService, ClipboardService>();

            services.AddSingleton<IFileService>(provider => 
                new FileService(
                    MainWindow, 
                    provider.GetRequiredService<IDialogService>(), 
                    provider.GetRequiredService<ILanguageService>()));
        }

        /// <summary>
        /// Gets the shellView model types.
        /// </summary>
        /// <value>The shellView model types.</value>
        public List<Type> ViewModelTypes { get; private set; }

        /// <summary>
        /// Gets the shellView types.
        /// </summary>
        /// <value>The shellView types.</value>
        public List<Type> ViewTypes { get; private set; }

        /// <summary>
        /// Gets the window types.
        /// </summary>
        /// <value>The window types.</value>
        public List<Type> WindowTypes { get; private set; }

        /// <summary>
        /// Bootstrapper types
        /// </summary>
        /// <value>The bootstrapper types.</value>
        public List<Type> BootstrapperTypes { get; private set; }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="mainAssembly">The main assembly.</param>
        protected void RegisterAssemblies(Assembly mainAssembly) => RegisterAssemblies(mainAssembly, null);

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="mainAssembly">The main assembly.</param>
        /// <param name="assemblyFilter">The assembly filter.</param>
        protected void RegisterAssemblies(Assembly mainAssembly, Func<AssemblyName, bool> assemblyFilter)
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
                        q.GetInterface(nameof(IView), false) is not null
                        && !q.Name.Equals(GenericConstants.ShellView)
                        && (q.Name.EndsWith(GenericConstants.View) || q.Name.EndsWith(GenericConstants.Page))
                        && q.Name != GenericConstants.View
                        && q.Name != GenericConstants.Page
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                WindowTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IWindow), false) is not null
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
                    _services.AddSingleton(abstraction, viewmodel);
                }
                else
                {
                    _services.AddSingleton(viewmodel);
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
                    _services.AddSingleton(abstraction, view);
                }
                else
                {
                    _services.AddSingleton(view);
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
                    _services.AddSingleton(abstraction, window);
                }
                else
                {
                    _services.AddSingleton(window);
                }
            }

            foreach (var bootstrapper in BootstrapperTypes.Distinct())
            {
                _services.AddSingleton(bootstrapper);
            }
        }

        /// <summary>
        /// Add resource managers to languageservice.
        /// </summary>
        /// <param name="languageService"></param>
        /// <example>_languageService.AddResourceManager(resourceManager)</example>
        public abstract void AddResourceManager(ILanguageService languageService);

        private void ConfigureNavigationService(INavigationService navigationService)
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
