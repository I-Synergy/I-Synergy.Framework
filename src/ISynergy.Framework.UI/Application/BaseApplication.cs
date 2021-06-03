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
using ISynergy.Framework.UI.Properties;
using System.Resources;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using System.Globalization;
using Windows.ApplicationModel.Background;
using System.Text.RegularExpressions;
using ISynergy.Framework.Mvvm.Extensions;
using Microsoft.Extensions.Configuration;

#if (NETFX_CORE || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;
using LaunchActivatedEventArgs = Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;
#elif (NET5_0 && WINDOWS)
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
        protected readonly IServiceProvider _serviceProvider;

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

            LanguageService = new LanguageService();

            ConfigureServices(_services);
            
            _serviceProvider = _services.BuildServiceProvider();

            ServiceLocator.SetLocatorProvider(_serviceProvider);

            Logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(BaseApplication));
            Logger.LogInformation("Starting application");

            Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            SetContext();

#if NETFX_CORE|| (NET5_0 && WINDOWS)
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

            ThemeSelector.SetThemeColor(_serviceProvider.GetRequiredService<ISettingsService>().Color.ToEnum(Mvvm.Enumerations.ThemeColors.Default));

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
#endif

#if HAS_UNO || NETFX_CORE
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

#if NETFX_CORE || (NET5_0 && WINDOWS)
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
#if NETFX_CORE || (NET5_0 && WINDOWS)
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

            Application.Current.Resources["SystemAccentColor"] = ThemeSelector.AccentColor;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

#if (NETFX_CORE || HAS_UNO)
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
#elif (NET5_0 && WINDOWS)
                if (args.UWPLaunchActivatedEventArgs.PreviousExecutionState == ApplicationExecutionState.Terminated)
#endif
                {
                    //TODO: Load state from previously suspended application
                }

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

                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

#if (NETFX_CORE || HAS_UNO)
            if (!args.PrelaunchActivated)
#elif (NET5_0 && WINDOWS)
            if (!args.UWPLaunchActivatedEventArgs.PrelaunchActivated)
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
                Window.Current.Activate();
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
            var rootFrame = Window.Current.Content as Frame;

            if (GetDescendantFromName(Window.Current.Content, "ContentRootFrame") is Frame _frame)
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
#if NETFX_CORE || (NET5_0 && WINDOWS)
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
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            services.AddSingleton((s) => LanguageService);
            services.AddSingleton((s) => _navigationService);

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<IBusyService, BusyService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();
            services.AddSingleton<IConverterService, ConverterService>();
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
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        public virtual void SetContext()
        {
            Context = _serviceProvider.GetRequiredService<IContext>();
            Context.ViewModels = ViewModelTypes;

#if NETFX_CORE || (NET5_0 && WINDOWS) || __WASM__
            //Only in Windows i can set the culture.
            var culture = CultureInfo.CurrentCulture;

                culture.NumberFormat.CurrencySymbol = $"{Context.CurrencySymbol} ";
                culture.NumberFormat.CurrencyNegativePattern = 1;

                Context.NumberFormat = culture.NumberFormat;
#endif

            var localizationFunctions = _serviceProvider.GetRequiredService<LocalizationFunctions>();
            localizationFunctions.SetLocalizationLanguage(_serviceProvider.GetRequiredService<ISettingsService>().Culture);
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public abstract void HandleException(Exception ex, string message);
    }
}
