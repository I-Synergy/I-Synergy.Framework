using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
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
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Extensions;
using System.Globalization;
using ISynergy.Framework.Core.Validation;
using System.IO;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Options;
using Windows.ApplicationModel.Core;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;
using Windows.UI.Core;

#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
using WinRT.Interop;
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

        protected readonly ITelemetryService _telemetryService;

        protected readonly IExceptionHandlerService _exceptionHandlerService;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected IContext _context;

        /// <summary>
        /// Main Application Window.
        /// </summary>
        public Window MainWindow { get; private set; }

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

            _telemetryService = _serviceProvider.GetRequiredService<ITelemetryService>();
            _exceptionHandlerService = _serviceProvider.GetRequiredService<IExceptionHandlerService>();
            _logger = _serviceProvider.GetRequiredService<ILogger>();
            _logger.LogInformation("Starting application");

            Application.Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Initialize();
        }

        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private async void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            }
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e) =>
            OnLaunchApplication(e);

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs" /> instance containing the event data.</param>
        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            e.SetObserved();
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private async void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                await _exceptionHandlerService.HandleExceptionAsync(exception);
        }

        /// <summary>
        /// On launch of application.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnLaunchApplication(LaunchActivatedEventArgs e)
        {
            MainWindow = new Window();

#if WINDOWS10_0_18362_0_OR_GREATER
            var appWindow = GetAppWindowForCurrentWindow(MainWindow);

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            }
#endif
            var rootFrame = MainWindow.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame is null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

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

            if (e.UWPLaunchActivatedEventArgs.Kind == ActivationKind.Launch)
            {
                if (rootFrame.Content is null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    var view = _serviceProvider.GetRequiredService<IShellView>();
                    rootFrame.Navigate(view.GetType(), e.Arguments);
                }
            }

            _themeSelector = _serviceProvider.GetRequiredService<IThemeService>();
            _themeSelector.InitializeMainWindow(MainWindow);

            MainWindow.Activate();
        }

#if WINDOWS10_0_18362_0_OR_GREATER && !HAS_UNO
        protected virtual AppWindow GetAppWindowForCurrentWindow(Window window)
        {
            var hWnd = WindowNative.GetWindowHandle(window);
            var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
#endif

        /// <summary>
        /// Sets the context.
        /// </summary>
        public virtual void Initialize()
        {
            Current.UnhandledException += Current_UnhandledException;

            _context = _serviceProvider.GetRequiredService<IContext>();
            _context.ViewModels = ViewModelTypes;

            var localizationFunctions = _serviceProvider.GetRequiredService<LocalizationFunctions>();
            var settingsService = _serviceProvider.GetRequiredService<IBaseApplicationSettingsService>();
            localizationFunctions.SetLocalizationLanguage(settingsService.Settings.Culture);

            // Bootstrap all registered modules.
            foreach (var bootstrapper in BootstrapperTypes.Distinct())
                if (_serviceProvider.GetService(bootstrapper) is IBootstrap instance)
                    instance.Bootstrap();
        }


        /// <summary>
        /// Get a new list of additional resource dictionaries which can be merged.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>();

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

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
            services.AddSingleton<ILogger>((s) => ConfigureLogger().CreateLogger(AppDomain.CurrentDomain.FriendlyName));
            services.AddSingleton<IMessageService, MessageService>();
            
            // Register scoped services
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();
            services.AddScoped<IConverterService, ConverterService>();
            services.AddScoped<IBusyService, BusyService>();
            services.AddScoped<IDialogService, DialogService>();
            services.AddScoped<IDispatcherService, DispatcherService>();

            services.AddScoped<IFileService>(provider => 
                new FileService(
                    MainWindow, 
                    provider.GetRequiredService<IDialogService>(), 
                    provider.GetRequiredService<ILanguageService>()));

            //Register functions
            services.AddScoped<LocalizationFunctions>();

            services.AddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
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
    }
}
