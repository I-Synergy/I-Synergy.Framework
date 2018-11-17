using Flurl.Http;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ISynergy.Enumerations;
using ISynergy.Events;
using ISynergy.Locators;
using ISynergy.Providers;
using ISynergy.Services;
using ISynergy.ViewModels.Base;
using ISynergy.Views;
using ISynergy.Views.Library;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.System.Profile;
using Windows.UI;
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
        public IContext Context { get; private set; }

        private ILogger logger = null;

        public ILogger Logger
        {
            get
            {
                if (logger is null)
                {
                    logger = SimpleIoc.Default.GetInstance<ILogger>();
                }

                return logger;
            }
        }

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

            Initialize();
            LaunchApplication();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;

                // The received URI is eventArgs.Uri.AbsoluteUri
                if (eventArgs.Uri.AbsoluteUri.Contains("test"))
                {
                    Initialize(SoftwareEnvironments.Test);
                }
                if (eventArgs.Uri.AbsoluteUri.Contains("local"))
                {
                    Initialize(SoftwareEnvironments.Local);
                }
                else
                {
                    Initialize();
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

        protected virtual void Initialize(SoftwareEnvironments environment = SoftwareEnvironments.Production)
        {
            SimpleIoc.Default.Register<ILogger>(() => new LoggerFactory().CreateLogger<BaseApplication>());
            SimpleIoc.Default.Register<IFlurlClient>(() => new FlurlClient());
            SimpleIoc.Default.Register<IBusyService, BusyService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ITelemetryService, TelemetryService>();
            SimpleIoc.Default.Register<IAuthenticationService, AuthenticationService>();
            SimpleIoc.Default.Register<IAuthenticationProvider, AuthenticationProvider>();
            SimpleIoc.Default.Register<IUIVisualizerService, UIVisualizerService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IInfoService, InfoService>();
            SimpleIoc.Default.Register<IConverterService, ConverterService>();
            SimpleIoc.Default.Register<IUpdateService, UpdateService>();

            SimpleIoc.Default.Register<INoteWindow>(() => SimpleIoc.Default.GetInstance<NoteWindow>());
            SimpleIoc.Default.Register<IMapsWindow>(() => SimpleIoc.Default.GetInstance<MapsWindow>());

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(1024, 768);

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

        protected void RegisterAssemblies(List<Assembly> assemblies)
        {
            assemblies.Add(Assembly.Load("I-Synergy.Framework.Core"));
            assemblies.Add(Assembly.Load("I-Synergy.Framework.Windows"));

            List<Type> viewTypes = new List<Type>();
            List<Type> windowTypes = new List<Type>();
            List<Type> viewmodelTypes = new List<Type>();

            foreach (Assembly assembly in assemblies)
            {
                viewmodelTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IViewModel), false) != null &&
                        q.Name.EndsWith(Constants.ViewModel) &&
                        q.Name != Constants.ViewModel &&
                        !q.IsAbstract &&
                        !q.IsInterface)
                    .ToList());

                viewTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IView), false) != null && (
                        q.Name.EndsWith(Constants.View) ||
                        q.Name.EndsWith(Constants.Page)) &&
                        q.Name != Constants.View &&
                        q.Name != Constants.Page &&
                        !q.IsAbstract &&
                        !q.IsInterface)
                    .ToList());

                windowTypes.AddRange(assembly.GetTypes()
                    .Where(q =>
                        q.GetInterface(nameof(IWindow), false) != null &&
                        q.Name.EndsWith(Constants.Window) &&
                        q.Name != Constants.Window &&
                        !q.IsAbstract &&
                        !q.IsInterface)
                    .ToList());
            }

            foreach (var viewmodel in viewmodelTypes.Distinct())
            {
                RegisterType(viewmodel);
            }

            foreach (Type view in viewTypes.Distinct())
            {
                RegisterType(view);

                Type viewmodel = viewmodelTypes.Where(q => q.Name == view.Name.ReplaceLastOf(Constants.View, Constants.ViewModel)).FirstOrDefault();

                if (viewmodel != null)
                {
                    SimpleIoc.Default.GetInstance<INavigationService>().Configure(viewmodel.FullName, view);
                }
            }

            foreach (var window in windowTypes.Distinct())
            {
                RegisterType(window);
            }

            SetContext();
        }

        private void RegisterType(Type implementationType)
        {
            // Get the Register<T1>() method
            MethodInfo methodInfo =
                SimpleIoc.Default.GetType().GetMethods()
                         .Where(m => m.Name == nameof(SimpleIoc.Default.Register))
                         .Where(m => m.IsGenericMethod)
                         .Where(m => m.GetGenericArguments().Length == 1)
                         .Where(m => m.GetParameters().Length == 0)
                         .Single();

            // Create a version of the method that takes your types
            methodInfo = methodInfo.MakeGenericMethod(implementationType);

            // Invoke the method on the default container (no parameters)
            methodInfo.Invoke(SimpleIoc.Default, null);
        }

        private void RegisterType(Type interfaceType, Type implementationType)
        {
            // Get the Register<T1,T2>() method
            MethodInfo methodInfo =
                SimpleIoc.Default.GetType().GetMethods()
                         .Where(m => m.Name == nameof(SimpleIoc.Default.Register))
                         .Where(m => m.IsGenericMethod)
                         .Where(m => m.GetGenericArguments().Length == 2)
                         .Where(m => m.GetParameters().Length == 0)
                         .Single();

            // Create a version of the method that takes your types
            methodInfo = methodInfo.MakeGenericMethod(interfaceType, implementationType);

            // Invoke the method on the default container (no parameters)
            methodInfo.Invoke(SimpleIoc.Default, null);
        }

        public virtual void SetContext()
        {
            Context = SimpleIoc.Default.GetInstance<IContext>();

            try
            {
                Logger.LogInformation("Update settings");
                SimpleIoc.Default.GetInstance<IBaseSettingsService>().CheckForUpgrade();
            }
            catch (Exception ex)
            {
                // if update is not available, application should still continue.
                Logger.LogError(ex.Message, ex);
            }

            string culture = SimpleIoc.Default.GetInstance<IBaseSettingsService>().Application_Culture;

            if (culture is null) culture = "en";

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            Context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            ((TelemetryClient)SimpleIoc.Default.GetInstance<ITelemetryService>().Client).InstrumentationKey = SimpleIoc.Default.GetInstance<IBaseSettingsService>().ApplicationInsights_InstrumentationKey;
            ((TelemetryClient)SimpleIoc.Default.GetInstance<ITelemetryService>().Client).Context.User.UserAgent = SimpleIoc.Default.GetInstance<IInfoService>().ProductName;
            ((TelemetryClient)SimpleIoc.Default.GetInstance<ITelemetryService>().Client).Context.Session.Id = Guid.NewGuid().ToString();
            ((TelemetryClient)SimpleIoc.Default.GetInstance<ITelemetryService>().Client).Context.Component.Version = SimpleIoc.Default.GetInstance<IInfoService>().ProductVersion.ToString();
            ((TelemetryClient)SimpleIoc.Default.GetInstance<ITelemetryService>().Client).Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            CustomXamlResourceLoader.Current = new CustomResourceLoader(SimpleIoc.Default.GetInstance<ILanguageService>());
        }

        public virtual async Task HandleException(Exception ex)
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();

            if (!(connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess))
            {
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowInformationAsync(
                    SimpleIoc.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT_INTERNET"));
            }
            else
            {
                if (ex is NotImplementedException)
                {
                    await SimpleIoc.Default.GetInstance<IDialogService>().ShowInformationAsync(
                        SimpleIoc.Default.GetInstance<ILanguageService>().GetString("EX_FUTURE_MODULE"));
                }
                else if (ex is UnauthorizedAccessException)
                {
                    await SimpleIoc.Default.GetInstance<IDialogService>().ShowErrorAsync(ex.Message);
                }
                else if (ex is IOException)
                {
                    if (ex.Message.Contains("The process cannot access the file") && ex.Message.Contains("because it is being used by another process"))
                    {
                        await SimpleIoc.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            SimpleIoc.Default.GetInstance<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await SimpleIoc.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            SimpleIoc.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                    }
                }
                else if (ex is ArgumentException)
                {
                    await SimpleIoc.Default.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            SimpleIoc.Default.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            ((ArgumentException)ex).ParamName)
                        );
                }
                else
                {
                    await SimpleIoc.Default.GetInstance<IDialogService>().ShowErrorAsync(
                        SimpleIoc.Default.GetInstance<ILanguageService>().GetString("EX_DEFAULT"));
                }
            }

            try
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
                SimpleIoc.Default.GetInstance<ITelemetryService>().Flush();
            }
            catch { }
            finally
            {
                Messenger.Default.Send(new ExceptionHandledMessage(this));
            }
        }
    }
}
