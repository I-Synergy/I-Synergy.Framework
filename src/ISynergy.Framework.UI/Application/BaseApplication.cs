using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Core.Abstractions.Services;
using Windows.UI.ViewManagement;
using System.Globalization;
using Windows.ApplicationModel.Background;
using System.Text.RegularExpressions;
using ISynergy.Framework.Mvvm.Extensions;
using ISynergy.Framework.Core.Services;
using System.Resources;
using ISynergy.Framework.UI.Extensions;

#if (WINDOWS_UWP)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;
using LaunchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;
#endif

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class BaseApplication.
    /// Implements the <see cref="Application" />
    /// </summary>
    /// <seealso cref="Application" />
    public abstract class BaseApplication : Application
    {
        /// <summary>
        /// Main Application Window.
        /// </summary>
        public Window MainWindow { get; private set; }
        
        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        protected readonly IThemeSelectorService _themeSelector;
        
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

            Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Initialize();

#if WINDOWS_UWP || WINDOWS
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
#endif

            _themeSelector = _serviceProvider.GetRequiredService<IThemeSelectorService>();
            _themeSelector.Initialize();

            if (_themeSelector.Theme is ElementTheme.Dark)
            {
                RequestedTheme = ApplicationTheme.Dark;
            }
            else if (_themeSelector.Theme is ElementTheme.Light)
            {
                RequestedTheme = ApplicationTheme.Light;
            }

            _themeSelector.SetThemeColor(_serviceProvider.GetRequiredService<IBaseSettingsService>().Color.ToEnum(Mvvm.Enumerations.ThemeColors.Default));

#if HAS_UNO || WINDOWS_UWP
            Suspending += OnSuspending;
#endif
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

#if WINDOWS_UWP || WINDOWS
        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(e.Exception, $"{e.Exception.Message}{Environment.NewLine}{e.Message}");
        }
#else
        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var ex = e.Exception;
            HandleException(ex, $"{ex.Message}{Environment.NewLine}{e.Message}");
        }
#endif

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
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
#if WinUI
            MainWindow = new Window();
            MainWindow.Activate();
#else
            MainWindow = Window.Current;
#endif

            var rootFrame = MainWindow.Content as Frame;

            Application.Current.Resources["SystemAccentColor"] = _themeSelector.AccentColor;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame is null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                // Add custom resourcedictionaries from code.
                var dictionary = Application.Current.Resources?.MergedDictionaries;
                
                if(dictionary is not null)
                {
                    foreach (var item in GetAdditionalResourceDictionaries())
                    {
                        if(!dictionary.Any(t => t.Source == item.Source))
                            Application.Current.Resources.MergedDictionaries.Add(item);
                    }
                }

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;
            }

#if WINDOWS_UWP
            if (!args.PrelaunchActivated)
#elif WinUI
            if (args.UWPLaunchActivatedEventArgs.Kind == ActivationKind.Launch)
#endif
            {
                if (rootFrame.Content is null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    var view = _serviceProvider.GetRequiredService<IShellView>();
                    rootFrame.Navigate(view.GetType(), args.Arguments);
                }

                // Ensure the current window is active
                MainWindow.Activate();
            }
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
        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

#if HAS_UNO || WINDOWS_UWP
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private static void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            //TODO: Save application state and stop any background activity
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                task.Value?.Unregister(true);
            }

            deferral.Complete();
        }
#endif


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
#elif WINDOWS_UWP
                builder.AddDebug();
#else
                builder.AddConsole();
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

            services.AddSingleton((s) => _languageService);
            services.AddSingleton((s) => _navigationService);
            
            services.AddSingleton<ILogger>((s) => ConfigureLogger().CreateLogger(AppDomain.CurrentDomain.FriendlyName));
            
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IBusyService, BusyService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
            services.AddSingleton<IConverterService, ConverterService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IDispatcherService, DispatcherService>();

            //Register functions
            services.AddSingleton<LocalizationFunctions>();
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

            RegisterAssemblies(assemblies);
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        protected void RegisterAssemblies(List<Assembly> assemblies)
        {
            assemblies.Add(Assembly.GetAssembly(typeof(MvvmAssemblyIdentifier)));
            assemblies.Add(Assembly.GetAssembly(typeof(UIAssemblyIdentifier)));
            assemblies.Add(Assembly.GetAssembly(typeof(CommonAssemblyIdentifier)));

#if WinUI || WINDOWS_UWP
            assemblies.Add(Assembly.GetAssembly(typeof(WinUIAssemblyIdentifier)));
#endif

            ViewTypes = new List<Type>();
            WindowTypes = new List<Type>();
            ViewModelTypes = new List<Type>();
            BootstrapperTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                ViewModelTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IViewModel), false) != null
                        && !q.Name.Equals(GenericConstants.ShellViewModel)
                        && (q.Name.EndsWith(GenericConstants.ViewModel) || Regex.IsMatch(q.Name, GenericConstants.ViewModelTRegex))
                        && q.Name != GenericConstants.ViewModel
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                ViewTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IView), false) != null && (
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
                        q.GetInterface(nameof(IWindow), false) != null
                        && q.Name.EndsWith(GenericConstants.Window)
                        && q.Name != GenericConstants.Window
                        && q.Name != nameof(ISynergy.Framework.UI.Controls.Window)
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                BootstrapperTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IBootstrap), false) != null
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());
            }

            foreach (var viewmodel in ViewModelTypes.Distinct())
            {
                var abstraction = viewmodel.GetInterfaces(false).FirstOrDefault();

                if (abstraction != null && !viewmodel.IsGenericType)
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

                if (abstraction != null)
                {
                    _services.AddSingleton(abstraction, view);
                }
                else
                {
                    _services.AddSingleton(view);
                }

                var viewmodel = ViewModelTypes.Find(q =>
                {
                    var name = view.Name.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel);
                    return q.GetViewModelName().Equals(name) || q.Name.Equals(name);
                });

                if (viewmodel != null)
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

                if (abstraction != null)
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
        /// Sets the context.
        /// </summary>
        public virtual void Initialize()
        {
            _context = _serviceProvider.GetRequiredService<IContext>();
            _context.ViewModels = ViewModelTypes;

#if WINDOWS_UWP || __WASM__
            //Only in Windows i can set the culture.
            var culture = CultureInfo.CurrentCulture;

                culture.NumberFormat.CurrencySymbol = $"{_context.CurrencySymbol} ";
                culture.NumberFormat.CurrencyNegativePattern = 1;

                _context.NumberFormat = culture.NumberFormat;
#endif

            var localizationFunctions = _serviceProvider.GetRequiredService<LocalizationFunctions>();
            localizationFunctions.SetLocalizationLanguage(_serviceProvider.GetRequiredService<IBaseSettingsService>().Culture);

            // Bootstrap all registered modules.
            foreach (var bootstrapper in BootstrapperTypes.Distinct())
                if (_serviceProvider.GetService(bootstrapper) is IBootstrap instance)
                    instance.Bootstrap();
        }

        /// <summary>
        /// Add resource managers to languageservice.
        /// </summary>
        public virtual void AddResourceManager(ResourceManager resourceManager) =>
            _languageService.AddResourceManager(resourceManager);

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public abstract void HandleException(Exception ex, string message);
    }
}
