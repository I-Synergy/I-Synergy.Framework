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
using Microsoft.Extensions.DependencyInjection;
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
        public IServiceProvider ServiceProvider { get; internal set; }
        public IServiceCollection ServiceCollection { get; internal set; }
        public IContext Context { get; internal set; }
        public ILogger Logger { get; internal set; }

        public bool _preview { get; internal set; }

        ///// <summary>
        ///// Gets the default DryIoc <see cref="IContainer"/> for the application.
        ///// </summary>
        ///// <value>The default <see cref="IContainer"/> instance.</value>
        //protected IContainer _container { get; set; }

        ///// <summary>
        ///// Creates the DryIoc <see cref="IContainer"/> that will be used as the default container.
        ///// </summary>
        ///// <returns>A new instance of <see cref="IContainer"/>.</returns>
        //private IContainer CreateContainer() => new Container(Rules.Default.WithConcreteTypeDynamicRegistrations());

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
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddLogging();

            ServiceCollection.AddSingleton<ISynergyService, SynergyService>();
            ServiceCollection.AddSingleton<IBusyService, BusyService>();
            ServiceCollection.AddSingleton<ITelemetryService, TelemetryService>();
            ServiceCollection.AddSingleton<IUIVisualizerService, UIVisualizerService>();
            ServiceCollection.AddSingleton<INavigationService, NavigationService>();
            ServiceCollection.AddSingleton<IDialogService, DialogService>();
            ServiceCollection.AddSingleton<IWindowService, WindowService>();
            ServiceCollection.AddSingleton<IInfoService, InfoService>();

            ServiceCollection.AddScoped<IUpdateService, UpdateService>();
            ServiceCollection.AddScoped<IAuthenticationProvider, AuthenticationProvider>();
            ServiceCollection.AddScoped<IConverterService, ConverterService>();
            

            ServiceProvider = ServiceCollection.BuildServiceProvider();

            Logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger<BaseApplication>();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(1024, 768);

            Logger.LogInformation(Gratification.Gratify());
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

            foreach (var item in viewmodelTypes.Distinct())
            {
                ServiceCollection.AddSingleton(item);
            }

            foreach (var item in viewTypes.Distinct())
            {
                ServiceCollection.AddSingleton(item);
            }

            foreach (var item in windowTypes.Distinct())
            {
                ServiceCollection.AddSingleton(item);
            }

            ServiceProvider = ServiceCollection.BuildServiceProvider();

            foreach (Type view in viewTypes)
            {
                Type viewmodel = viewmodelTypes.Where(q => q.Name == view.Name.ReplaceLastOf(Constants.View, Constants.ViewModel)).FirstOrDefault();

                if (viewmodel != null)
                {
                    ServiceProvider.GetService<INavigationService>().Configure(viewmodel.FullName, view);
                }
            }

            SetContext();
        }

        public virtual void SetContext(
            string previewApiUrl = @"https://app-test.i-synergy.nl/api",
            string previewAccountUrl = @"https://app-test.i-synergy.nl/account",
            string previewTokenUrl = @"https://app-test.i-synergy.nl/oauth/token",
            string previewWebUrl = @"http://test.i-synergy.nl/")
        {
            Context = ServiceProvider.GetService<IContext>();

            if (_preview)
            {
                Context.Environment = ".preview";
                Context.ApiUrl = previewApiUrl;
                Context.AccountUrl = previewAccountUrl;
                Context.TokenUrl = previewTokenUrl;
                Context.WebUrl = previewWebUrl;
            }

            try
            {
                Logger.LogInformation("Update settings");
                ServiceProvider.GetService<ISettingsServiceBase>().CheckForUpgrade();
            }
            catch (Exception ex)
            {
                // if update is not available, application should still continue.
                Logger.LogError(ex.Message, ex);
            }

            string culture = ServiceProvider.GetService<ISettingsServiceBase>().Application_Culture;

            if (culture is null) culture = "en";

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            Context.CurrencySymbol = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

            ((TelemetryClient)ServiceProvider.GetService<ITelemetryService>().Client).InstrumentationKey = ServiceProvider.GetService<ISettingsServiceBase>().ApplicationInsights_InstrumentationKey;
            ((TelemetryClient)ServiceProvider.GetService<ITelemetryService>().Client).Context.User.UserAgent = ServiceProvider.GetService<IInfoService>().ProductName;
            ((TelemetryClient)ServiceProvider.GetService<ITelemetryService>().Client).Context.Session.Id = Guid.NewGuid().ToString();
            ((TelemetryClient)ServiceProvider.GetService<ITelemetryService>().Client).Context.Component.Version = ServiceProvider.GetService<IInfoService>().ProductVersion.ToString();
            ((TelemetryClient)ServiceProvider.GetService<ITelemetryService>().Client).Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            CustomXamlResourceLoader.Current = new CustomResourceLoader(ServiceProvider.GetService<ILanguageService>());
        }

        public virtual async Task HandleException(Exception ex)
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();

            if (!(connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess))
            {
                await ServiceProvider.GetService<IDialogService>().ShowInformationAsync(
                    ServiceProvider.GetService<ILanguageService>().GetString("EX_DEFAULT_INTERNET"));
            }
            else
            {
                if (ex is NotImplementedException)
                {
                    await ServiceProvider.GetService<IDialogService>().ShowInformationAsync(
                        ServiceProvider.GetService<ILanguageService>().GetString("EX_FUTURE_MODULE"));
                }
                else if (ex is UnauthorizedAccessException)
                {
                    await ServiceProvider.GetService<IDialogService>().ShowErrorAsync(ex.Message);
                }
                else if (ex is IOException)
                {
                    if (ex.Message.Contains("The process cannot access the file") && ex.Message.Contains("because it is being used by another process"))
                    {
                        await ServiceProvider.GetService<IDialogService>().ShowErrorAsync(
                            ServiceProvider.GetService<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await ServiceProvider.GetService<IDialogService>().ShowErrorAsync(
                            ServiceProvider.GetService<ILanguageService>().GetString("EX_DEFAULT"));
                    }
                }
                else if (ex is ArgumentException)
                {
                    await ServiceProvider.GetService<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceProvider.GetService<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            ((ArgumentException)ex).ParamName)
                        );
                }
                else
                {
                    await ServiceProvider.GetService<IDialogService>().ShowErrorAsync(
                        ServiceProvider.GetService<ILanguageService>().GetString("EX_DEFAULT"));
                }
            }

            try
            {
                ServiceProvider.GetService<ITelemetryService>().TrackException(ex);
                ServiceProvider.GetService<ITelemetryService>().Flush();
            }
            catch { }
            finally
            {
                Messenger.Default.Send(new ExceptionHandledMessage());
            }
        }
    }
}
