using System;
using System.Collections.Generic;
using System.Globalization;
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
using ISynergy.Framework.Mvvm.Messaging;
using ISynergy.Framework.UI.Abstractions.Providers;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Functions;
using ISynergy.Framework.UI.Providers;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Microsoft.Extensions.DependencyInjection;
using ISynergy.Framework.UI.Properties;
using System.Resources;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.Mvvm;

#if NETFX_CORE
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;
#endif

using Window = ISynergy.Framework.UI.Controls.Window;

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
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IContext Context { get; private set; }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; private set; }
        /// <summary>
        /// Gets the theme selector.
        /// </summary>
        /// <value>The theme selector.</value>
        public IThemeSelectorService ThemeSelector { get; private set; }

        /// <summary>
        /// Gets the language service.
        /// </summary>
        /// <value>The language service.</value>
        public ILanguageService LanguageService { get; private set; }

        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// The services
        /// </summary>
        private readonly IServiceCollection _services;
        /// <summary>
        /// The navigation service
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApplication" /> class.
        /// </summary>
        protected BaseApplication()
        {
            _services = new ServiceCollection();
            _navigationService = new NavigationService();

            LanguageService = new LanguageService(new ResourceManager(typeof(Resources)));

            ConfigureServices(_services);
            
            _serviceProvider = _services.BuildServiceProvider();

            ServiceLocator.SetLocatorProvider(_serviceProvider);

            Logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(BaseApplication));
            Logger.LogInformation("Starting application");

            Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            SetContext();

#if NETFX_CORE
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
#endif

            ThemeSelector = _serviceProvider.GetRequiredService<IThemeSelectorService>();
            ThemeSelector.Initialize();

            if (ThemeSelector.Theme is ElementTheme.Dark)
            {
                RequestedTheme = ApplicationTheme.Dark;
            }
            else if (ThemeSelector.Theme is ElementTheme.Light)
            {
                RequestedTheme = ApplicationTheme.Light;
            }

#if NETFX_CORE
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    break;
                case "Windows.IoT":
                    break;
                case "Windows.Xbox":
                    RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
                    var result = ApplicationViewScaling.TrySetDisableLayoutScaling(true);
                    break;
            }

            RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            Suspending += OnSuspending;
#endif

            //Gets or sets a value that indicates whether to engage the text performance visualization feature of Microsoft Visual Studio when the app runs.
            //this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;

            //Gets or sets a value that indicates whether to display frame-rate and per-frame CPU usage info. These display as an overlay of counters in the window chrome while the app runs.
            //this.EnableFrameRateCounter = true;
        }

        

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs" /> instance containing the event data.</param>
        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            await HandleException(e.Exception, e.Exception.Message);
        }

#if NETFX_CORE
        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private async void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await HandleException(e.Exception, $"{e.Exception.Message}{Environment.NewLine}{e.Message}");
            e.Handled = true;
        }
#else
        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private async void Current_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var ex = e.Exception;
            await HandleException(ex, $"{ex.Message}{Environment.NewLine}{e.Message}");
        }
#endif

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs" /> instance containing the event data.</param>
        private async void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                await HandleException(exception, exception.Message);
            }
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if NETFX_CORE
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Desktop":
                    break;
                case "Windows.IoT":
                    break;
                case "Windows.Xbox":
                    ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                    break;
            }
#endif

            ThemeSelector.SetThemeColor(_serviceProvider.GetRequiredService<IApplicationSettingsService>().Color.ToEnum(ThemeColors.Default));


            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Windows.UI.Xaml.Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;

                // Place the frame in the current Window
                Windows.UI.Xaml.Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    try
                    {
                        var view = _serviceProvider.GetRequiredService<IShellView>();
                        rootFrame.Navigate(view.GetType(), e.Arguments);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    
                }

                // Ensure the current window is active
                Windows.UI.Xaml.Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

        /// <summary>
        /// Handles the <see cref="E:Navigated" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        private static void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Handles the <see cref="E:BackRequested" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="BackRequestedEventArgs" /> instance containing the event data.</param>
        private static void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

            if (GetDescendantFromName(Windows.UI.Xaml.Window.Current.Content, "ContentRootFrame") is Frame _frame)
            {
                rootFrame = _frame;
            }

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        /// <summary>
        /// Gets the name of the descendant from.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns>FrameworkElement.</returns>
        public static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
                return null;

            for (var i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name)
                        return frameworkElement;

                    frameworkElement = GetDescendantFromName(frameworkElement, name);

                    if (frameworkElement != null)
                        return frameworkElement;
                }
            }

            return null;
        }

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
#if NETFX_CORE
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                task.Value?.Unregister(true);
            }
#endif

            deferral.Complete();
        }

        /// <summary>
        /// The factory
        /// </summary>
        private ILoggerFactory _factory = null;

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        private ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory is null)
                {
                    _factory = new LoggerFactory();
                    ConfigureLogger(_factory);
                }

                return _factory;
            }
            set { _factory = value; }
        }

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="factory">The factory.</param>
        protected abstract void ConfigureLogger(ILoggerFactory factory);

        /// <summary>
        /// Gets the entry assembly.
        /// </summary>
        /// <returns>Assembly.</returns>
        protected abstract Assembly GetEntryAssembly();

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<IMessenger, Messenger>();
            services.AddSingleton<IBusyService, BusyService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton((s) => LanguageService);
            services.AddSingleton<IInfoService>((s) => new InfoService(GetEntryAssembly()));
            services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
            services.AddSingleton<IUIVisualizerService, UIVisualizerService>();
            services.AddSingleton((s) => _navigationService);
            services.AddSingleton<IConverterService, ConverterService>();
            services.AddSingleton<IApplicationSettingsService, ApplicationSettingsService>();
            services.AddSingleton<IFileService, FileService>();

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
        /// Registers the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        protected void RegisterAssemblies(List<Assembly> assemblies)
        {
            assemblies.Add(Assembly.GetAssembly(typeof(MvvmAssemblyIdentifier)));
            assemblies.Add(Assembly.GetAssembly(typeof(UIAssemblyIdentifier)));

            ViewTypes = new List<Type>();
            WindowTypes = new List<Type>();
            ViewModelTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                ViewModelTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IViewModel), false) != null
                        && !q.Name.Equals(GenericConstants.ShellViewModel)
                        && q.Name.EndsWith(GenericConstants.ViewModel)
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
                        && q.Name != nameof(View)
                        && q.Name != GenericConstants.Page
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());

                WindowTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IWindow), false) != null
                        && q.Name.EndsWith(GenericConstants.Window)
                        && q.Name != GenericConstants.Window
                        && q.Name != nameof(Window)
                        && !q.IsAbstract
                        && !q.IsInterface)
                    .ToList());
            }

            foreach (var viewmodel in ViewModelTypes.Distinct())
            {
                var abstraction = viewmodel.GetInterfaces(false).FirstOrDefault();

                if (abstraction != null)
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
                    .Where(q =>
                        q.GetInterfaces().Contains(typeof(IView))
                        && q.Name != nameof(IView))
                    .FirstOrDefault();

                if (abstraction != null)
                {
                    _services.AddSingleton(abstraction, view);
                }
                else
                {
                    _services.AddSingleton(view);
                }

                var viewmodel = ViewModelTypes.Find(q => q.Name == view.Name.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel));

                if (viewmodel != null)
                {
                    _navigationService.Configure(viewmodel.FullName, view);
                }
            }

            foreach (var window in WindowTypes.Distinct())
            {
                var abstraction = window
                    .GetInterfaces()
                    .Where(q =>
                        q.GetInterfaces().Contains(typeof(IWindow))
                        && q.Name != nameof(IWindow))
                    .FirstOrDefault();

                if (abstraction != null)
                {
                    _services.AddSingleton(abstraction, window);
                }
                else
                {
                    _services.AddSingleton(window);
                }
            }
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        public virtual void SetContext()
        {
            Context = _serviceProvider.GetRequiredService<IContext>();
            Context.ViewModels = ViewModelTypes;

#if NETFX_CORE || __WASM__
                //Only in Windows i can set the culture.
                var culture = CultureInfo.CurrentCulture;

                culture.NumberFormat.CurrencySymbol = $"{Context.CurrencySymbol} ";
                culture.NumberFormat.CurrencyNegativePattern = 1;

                Context.NumberFormat = culture.NumberFormat;
#endif

            var localizationFunctions = _serviceProvider.GetRequiredService<LocalizationFunctions>();
            localizationFunctions.SetLocalizationLanguage(_serviceProvider.GetRequiredService<IApplicationSettingsService>().Culture);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public abstract Task HandleException(Exception ex, string message);
    }
}
