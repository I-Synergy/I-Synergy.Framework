using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flurl.Http;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Constants;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Messaging;
using ISynergy.Framework.Windows.Abstractions.Providers;
using ISynergy.Framework.Windows.Abstractions.Services;
using ISynergy.Framework.Windows.Abstractions.Views;
using ISynergy.Framework.Windows.Controls;
using ISynergy.Framework.Windows.Functions;
using ISynergy.Framework.Windows.Providers;
using ISynergy.Framework.Windows.Services;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Networking.Connectivity;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;
using Window = ISynergy.Framework.Windows.Controls.Window;

namespace ISynergy.Framework.Windows
{
    /// <summary>
    /// Class BaseApplication.
    /// Implements the <see cref="Windows.UI.Xaml.Application" />
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Application" />
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
        /// Initializes a new instance of the <see cref="BaseApplication"/> class.
        /// </summary>
        protected BaseApplication()
        {
            ConfigureServices();

            ThemeSelector = ServiceLocator.Default.GetInstance<IThemeSelectorService>();
            ThemeSelector.Initialize();

            if (ThemeSelector.Theme is ElementTheme.Dark)
            {
                RequestedTheme = ApplicationTheme.Dark;
            }
            else if (ThemeSelector.Theme is ElementTheme.Light)
            {
                RequestedTheme = ApplicationTheme.Light;
            }

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

            Suspending += OnSuspending;
            RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            //Gets or sets a value that indicates whether to engage the text performance visualization feature of Microsoft Visual Studio when the app runs.
            //this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;

            //Gets or sets a value that indicates whether to display frame-rate and per-frame CPU usage info. These display as an overlay of counters in the window chrome while the app runs.
            //this.EnableFrameRateCounter = true;

            Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// Handles the UnobservedTaskException event of the TaskScheduler control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnobservedTaskExceptionEventArgs"/> instance containing the event data.</param>
        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            await HandleException(e.Exception, e.Exception.Message);
        }

        /// <summary>
        /// Handles the UnhandledException event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private async void Current_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await HandleException(e.Exception, e.Message);
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private async void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if(e.ExceptionObject is Exception exception)
            {
                await HandleException(exception, exception.Message);
            }
        }

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
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

            LaunchApplication();
        }

        /// <summary>
        /// Launches the application.
        /// </summary>
        private void LaunchApplication()
        {
            ThemeSelector.SetRequestedTheme();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(global::Windows.UI.Xaml.Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                // Place the frame in the current Window
                global::Windows.UI.Xaml.Window.Current.Content = rootFrame;

                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
            }

            if (rootFrame.Content is null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(ServiceLocator.Default.GetInstance<IShellView>().GetType());
            }

            // Ensure the current window is active
            global::Windows.UI.Xaml.Window.Current.Activate();

            //if (!System.Diagnostics.Debugger.IsAttached)
            //    ISynergy.Framework.Windows.Controls.ScreenSaver.InitializeScreensaver(new Uri("ms-appx://I-Synergy.Pos/Assets/SplashScreen.scale-400.png"));
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        /// <exception cref="Exception">Failed to load Page " + e.SourcePageType.FullName</exception>
        private static void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Handles the <see cref="E:Navigated" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
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
        /// <param name="e">The <see cref="BackRequestedEventArgs"/> instance containing the event data.</param>
        private static void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = global::Windows.UI.Xaml.Window.Current.Content as Frame;

            if (GetDescendantFromName(global::Windows.UI.Xaml.Window.Current.Content, "ContentRootFrame") is Frame _frame)
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
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                task.Value?.Unregister(true);
            }

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
                if (_factory == null)
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
        /// Configures the services.
        /// </summary>
        protected virtual void ConfigureServices()
        {
            ServiceLocator.Default.Register<ILoggerFactory>(() => new LoggerFactory());
            ServiceLocator.Default.Register<IFlurlClient>(() => new FlurlClient());
            ServiceLocator.Default.Register<IMessenger>(() => new Messenger());
            ServiceLocator.Default.Register<IBusyService, BusyService>();
            ServiceLocator.Default.Register<IThemeSelectorService, ThemeSelectorService>();
            ServiceLocator.Default.Register<IDialogService, DialogService>();
            ServiceLocator.Default.Register<IInfoService, InfoService>();
            ServiceLocator.Default.Register<IAuthenticationProvider, AuthenticationProvider>();
            ServiceLocator.Default.Register<IUIVisualizerService, UIVisualizerService>();
            ServiceLocator.Default.Register<INavigationService, NavigationService>();
            ServiceLocator.Default.Register<IConverterService, ConverterService>();
            ServiceLocator.Default.Register<IApplicationSettingsService, ApplicationSettingsService>();

            //Register functions
            ServiceLocator.Default.Register<LocalizationFunctions>();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            Logger = ServiceLocator.Default.GetInstance<ILoggerFactory>().CreateLogger(typeof(BaseApplication));
            Logger.LogInformation("Starting application");

            Logger.LogInformation("Allow 404 errors in Flurl");
            FlurlHttp.Configure(c =>
            {
                c.Timeout = TimeSpan.FromSeconds(30);
                //c.AutoDispose = false;

                // Statuses (in addition to 2xx) that won't throw exceptions.
                // Set to "*" to allow everything.
                c.AllowedHttpStatusRange = "404";
            });
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
            assemblies.Add(Assembly.Load("I-Synergy.Framework.Mvvm"));
            assemblies.Add(Assembly.Load("I-Synergy.Framework.Windows"));

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

                if(abstraction != null)
                {
                    RegisterType(abstraction, viewmodel);
                }
                else
                {
                    RegisterType(viewmodel);
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
                    RegisterType(abstraction, view);
                }
                else
                {
                    RegisterType(view);
                }

                var viewmodel = ViewModelTypes.Find(q => q.Name == view.Name.ReplaceLastOf(GenericConstants.View, GenericConstants.ViewModel));

                if (viewmodel != null)
                {
                    ServiceLocator.Default.GetInstance<INavigationService>().Configure(viewmodel.FullName, view);
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
                    RegisterType(abstraction, window);
                }
                else
                {
                    RegisterType(window);
                }
            }

            SetContext();
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        private void RegisterType(Type implementationType)
        {
            // Get the Register<T1>() method
            var methodInfo =
                ServiceLocator.Default.GetType().GetMethods()
                         .Single(m => m.Name == nameof(ServiceLocator.Default.Register)
                            && m.IsGenericMethod
                            && m.GetGenericArguments().Length == 1
                            && m.GetParameters().Length == 0);

            // Create a version of the method that takes your types
            methodInfo = methodInfo.MakeGenericMethod(implementationType);

            // Invoke the method on the default container (no parameters)
            methodInfo.Invoke(ServiceLocator.Default, null);
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        private void RegisterType(Type interfaceType, Type implementationType)
        {
            // Get the Register<T1,T2>() method
            var methodInfo =
                ServiceLocator.Default.GetType().GetMethods()
                         .Single(m => m.Name == nameof(ServiceLocator.Default.Register)
                            && m.IsGenericMethod
                            && m.GetGenericArguments().Length == 2
                            && m.GetParameters().Length == 0);

            // Create a version of the method that takes your types
            methodInfo = methodInfo.MakeGenericMethod(interfaceType, implementationType);

            // Invoke the method on the default container (no parameters)
            methodInfo.Invoke(ServiceLocator.Default, null);
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        public virtual void SetContext()
        {
            Context = ServiceLocator.Default.GetInstance<IContext>();
            Context.ViewModels = ViewModelTypes;

            var culture = CultureInfo.CurrentCulture;
            culture.NumberFormat.CurrencySymbol = $"{Context.CurrencySymbol} ";
            culture.NumberFormat.CurrencyNegativePattern = 1;

            Context.NumberFormat = culture.NumberFormat;

            var localizationFunctions = ServiceLocator.Default.GetInstance<LocalizationFunctions>();
            localizationFunctions.SetLocalizationLanguage(ServiceLocator.Default.GetInstance<IApplicationSettingsService>().Culture);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        public virtual async Task HandleException(Exception ex, string message)
        {
            // Set busyIndicator to false if it's true.
            await ServiceLocator.Default.GetInstance<IBusyService>().EndBusyAsync();
            await ServiceLocator.Default.GetInstance<ITelemetryService>().TrackExceptionAsync(ex, message);

            var connections = NetworkInformation.GetInternetConnectionProfile();

            if (connections?.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.InternetAccess)
            {
                await ServiceLocator.Default.GetInstance<IDialogService>().ShowInformationAsync(
                    ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT_INTERNET"));
            }
            else
            {
                if (ex is NotImplementedException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowInformationAsync(
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FUTURE_MODULE"));
                }
                else if (ex is UnauthorizedAccessException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(ex.Message);
                }
                else if (ex is IOException)
                {
                    if (ex.Message.Contains("The process cannot access the file") && ex.Message.Contains("because it is being used by another process"))
                    {
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                    }
                }
                else if (ex is ArgumentException argumentException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            argumentException.ParamName)
                        );
                }
                else
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                        ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                }
            }
        }
    }
}
