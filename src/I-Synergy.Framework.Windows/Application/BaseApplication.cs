using CommonServiceLocator;
using DryIoc;
using Flurl.Http;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ISynergy.Events;
using ISynergy.Extensions;
using ISynergy.Providers;
using ISynergy.Services;
using ISynergy.ViewModels;
using ISynergy.ViewModels.Base;
using ISynergy.Views;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.Connectivity;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;

namespace ISynergy
{
    public abstract class BaseApplication : Application
    {
        private IContext _context { get; set; }
        private bool _preview { get; set; }

        /// <summary>
        /// Gets the <see cref="ILogger"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILogger"/> instance.</value>
        protected readonly ILogger _logger = new LoggerFactory().CreateLogger<BaseApplication>();

        /// <summary>
        /// Gets the default DryIoc <see cref="IContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IContainer"/> instance.</value>
        protected IContainer _container { get; set; }

        /// <summary>
        /// Creates the DryIoc <see cref="IContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IContainer"/>.</returns>
        private IContainer CreateContainer() => new Container(Rules.Default.WithConcreteTypeDynamicRegistrations());

        public BaseApplication()
            : base()
        {
            ThemeSelectorService.Initialize();

            if (ThemeSelectorService.Theme == ElementTheme.Dark)
            {
                this.RequestedTheme = ApplicationTheme.Dark;
            }
            else if (ThemeSelectorService.Theme == ElementTheme.Light)
            {
                this.RequestedTheme = ApplicationTheme.Light;
            }

            this.Suspending += OnSuspending;
            this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            //Gets or sets a value that indicates whether to engage the text performance visualization feature of Microsoft Visual Studio when the app runs.
            //this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;

            //Gets or sets a value that indicates whether to display frame-rate and per-frame CPU usage info. These display as an overlay of counters in the window chrome while the app runs.
            //this.EnableFrameRateCounter = true;

            Application.Current.UnhandledException += Current_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            await HandleException(e.Exception);
        }

        private async void Current_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await HandleException(e.Exception);
        }

        private async void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            await HandleException(e.ExceptionObject as Exception);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            //XBOX support
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                bool result = ApplicationViewScaling.TrySetDisableLayoutScaling(true);
            }

            Initialize(false);
            LaunchApplication();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;

                // The received URI is eventArgs.Uri.AbsoluteUri
                if (eventArgs.Uri.AbsoluteUri.Contains("test") || eventArgs.Uri.AbsoluteUri.Contains("preview"))
                {
                    Initialize(true);
                }
                else
                {
                    Initialize(false);
                }

                LaunchApplication();
            }
        }

        private void LaunchApplication()
        {
            ThemeSelectorService.SetRequestedTheme();

            DispatcherHelper.Initialize();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

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
                rootFrame.Navigate(typeof(ShellView));
            }

            // Ensure the current window is active
            Window.Current.Activate();

            //ThemeSelectorService.SetRequestedTheme();

            //if (!System.Diagnostics.Debugger.IsAttached)
            //    ISynergy.Controls.ScreenSaver.InitializeScreensaver(new Uri("ms-appx://I-Synergy.Pos/Assets/SplashScreen.scale-400.png"));
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected virtual void Initialize(bool preview = false)
        {
            _preview = preview;

#if PREVIEW
            _preview = true;
#endif

            _container = CreateContainer();

            IoCServiceLocator serviceLocator = new IoCServiceLocator(_container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            // register the locator in DryIoc as well
            _container.UseInstance<IServiceLocator>(serviceLocator);
            _container.UseInstance(_logger);

            _container.Register<IBusyService, BusyService>(Reuse.ScopedOrSingleton);
            _container.Register<IDialogService, DialogService>();
            _container.Register<ITelemetryService, TelemetryService>(Reuse.ScopedOrSingleton);
            _container.Register<IAuthenticationProvider, AuthenticationProvider>();
            _container.Register<IDownloadFileService, DownloadFileService>();
            _container.Register<IUIVisualizerService, UIVisualizerService>(Reuse.ScopedOrSingleton);
            _container.Register<INavigationService, NavigationService>(Reuse.ScopedOrSingleton);
            _container.Register<IWindowService, WindowService>();
            _container.Register<IInfoService, InfoService>();
            _container.Register<IConverterService, ConverterService>();
            _container.Register<IUpdateService, UpdateService>(Reuse.ScopedOrSingleton);

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(1024, 768);

            _logger.LogInformation(Gratification.Gratify());
            _logger.LogInformation("Starting application");

            _logger.LogInformation("Allow 404 errors in Flurl");
            FlurlHttp.Configure(c =>
            {
                c.Timeout = TimeSpan.FromSeconds(30);
                //c.AutoDispose = false;

                // Statuses (in addition to 2xx) that won't throw exceptions.
                // Set to "*" to allow everything.
                c.AllowedHttpStatusRange = "404";
            });
        }

        protected void RegisterAssemblies(List<Assembly> assemblies)
        {
            assemblies.Add(Assembly.Load("I-Synergy.Framework.Core"));
            assemblies.Add(Assembly.Load("I-Synergy.Framework.Windows"));

            ////Register viewmodels
            _container.RegisterMany(
                assemblies,
                serviceTypeCondition: t =>
                    t.Name.EndsWith("ViewModel") &&
                    t.GetInterface(nameof(IViewModel), false) != null,
                ifAlreadyRegistered: IfAlreadyRegistered.Keep,
                made: FactoryMethod.ConstructorWithResolvableArguments);

            ////Register views
            _container.RegisterMany(
                assemblies,
                serviceTypeCondition: t =>
                    t.Name.EndsWith("View") &&
                    t.GetInterface(nameof(IView), false) != null);

            ////Register windows
            _container.RegisterMany(
                assemblies,
                serviceTypeCondition: t =>
                    t.Name.EndsWith("Window") &&
                    t.GetInterface(nameof(IWindow), false) != null);

            Register(assemblies.ToArray());
            SetContext(
                _context,
                _logger,
                _preview);
        }

        public virtual void SetContext(
            IContext context,
            ILogger logger,
            bool preview,
            string previewApiUrl = @"https://app-test.i-synergy.nl/api",
            string previewAccountUrl = @"https://app-test.i-synergy.nl/account",
            string previewTokenUrl = @"https://app-test.i-synergy.nl/oauth/token",
            string previewWebUrl = @"http://test.i-synergy.nl/")
        {
            context = ServiceLocator.Current.GetInstance<IContext>();

            if (preview)
            {
                context.Environment = ".preview";
                context.ApiUrl = previewApiUrl;
                context.AccountUrl = previewAccountUrl;
                context.TokenUrl = previewTokenUrl;
                context.WebUrl = previewWebUrl;
            }

            logger.LogInformation("Update settings");
            ServiceLocator.Current.GetInstance<ISettingsServiceBase>().CheckForUpgrade();

            string culture = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().Application_Culture;

            if (culture is null) culture = "en";

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).InstrumentationKey = ServiceLocator.Current.GetInstance<ISettingsServiceBase>().ApplicationInsights_InstrumentationKey;
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.User.UserAgent = ServiceLocator.Current.GetInstance<IInfoService>().ProductName;
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.Session.Id = Guid.NewGuid().ToString();
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.Component.Version = ServiceLocator.Current.GetInstance<IInfoService>().ProductVersion.ToString();
            ((TelemetryClient)ServiceLocator.Current.GetInstance<ITelemetryService>().Client).Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            CustomXamlResourceLoader.Current = new CustomResourceLoader(ServiceLocator.Current.GetInstance<ILanguageService>());
        }

        public void Register(Assembly[] assemblies)
        {
            List<Type> viewTypes = new List<Type>();
            List<Type> viewmodelTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                viewTypes.AddRange(assembly.GetTypes().Where(q => q.Name.EndsWith(Constants.View) && q.GetInterface(nameof(IView), false) != null).ToList());
                viewmodelTypes.AddRange(assembly.GetTypes().Where(q => q.Name.EndsWith(Constants.ViewModel) && q.GetInterface(nameof(IViewModel), false) != null).ToList());
            }

            foreach (Type view in viewTypes)
            {
                Type viewmodel = viewmodelTypes.Where(q => q.Name == view.Name.ReplaceLastOf(Constants.View, Constants.ViewModel)).FirstOrDefault();

                if (viewmodel != null)
                {
                    ServiceLocator.Current.GetInstance<INavigationService>().Configure(viewmodel.FullName, view);
                }
            }
        }

        public virtual async Task HandleException(Exception ex)
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();

            if (!(connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess))
            {
                await ServiceLocator.Current.GetInstance<IDialogService>().ShowInformationAsync(
                    ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_DEFAULT_INTERNET"));
            }
            else
            {
                if (ex is NotImplementedException)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowInformationAsync(
                        ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_FUTURE_MODULE"));
                }
                else if (ex is UnauthorizedAccessException)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(ex.Message);
                }
                else if (ex is IOException)
                {
                    if (ex.Message.Contains("The process cannot access the file") && ex.Message.Contains("because it is being used by another process"))
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                    }
                }
                else if (ex is ArgumentException)
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            ((ArgumentException)ex).ParamName)
                        );
                }
                else
                {
                    await ServiceLocator.Current.GetInstance<IDialogService>().ShowErrorAsync(
                        ServiceLocator.Current.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                }
            }

            try
            {
                ServiceLocator.Current.GetInstance<ITelemetryService>().TrackException(ex);
                ServiceLocator.Current.GetInstance<ITelemetryService>().Flush();
            }
            catch { }
            finally
            {
                Messenger.Default.Send(new ExceptionHandledMessage());
            }
        }
    }
}
