using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ISynergy.Framework.UI
{
    /// <summary>
    /// Class BaseApplication.
    /// </summary>
    public abstract class BaseApplication : Application, IBaseApplication
    {
        /// <summary>
        /// Gets the ExceptionHandler service.
        /// </summary>
        private readonly IExceptionHandlerService _exceptionHandlerService;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private readonly ILogger _logger;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        private readonly IContext _context;

        private readonly IThemeService _themeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IBaseApplicationSettingsService _applicationSettingsService;

        private Task Initialize { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseApplication()
            : base()
        {
            _logger = ServiceLocator.Default.GetInstance<ILogger>();
            _logger.LogInformation("Starting application");

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            _context = ServiceLocator.Default.GetInstance<IContext>();
            _authenticationService = ServiceLocator.Default.GetInstance<IAuthenticationService>();
            _themeService = ServiceLocator.Default.GetInstance<IThemeService>();
            _exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

            _applicationSettingsService = ServiceLocator.Default.GetInstance<IBaseApplicationSettingsService>();
            _applicationSettingsService.LoadSettings();

            _localizationService = ServiceLocator.Default.GetInstance<ILocalizationService>();

            if (_applicationSettingsService.Settings is not null)
                _localizationService.SetLocalizationLanguage(_applicationSettingsService.Settings.Culture);
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {

            Debug.WriteLine(e.Exception.Message);
        }

        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (_exceptionHandlerService is not null)
                await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            else
                _logger.LogCritical(e.Exception, e.Exception.Message);

            e.SetObserved();
        }

        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                if (_exceptionHandlerService is not null)
                    await _exceptionHandlerService.HandleExceptionAsync(exception);
                else
                    _logger.LogCritical(exception, exception.Message);
        }

        public void InitializeApplication() => Initialize = InitializeApplicationAsync();

        public virtual async Task InitializeApplicationAsync()
        {
            _logger.LogInformation("Starting initialization of application");

            var culture = CultureInfo.CurrentCulture;
            var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
            numberFormat.CurrencySymbol = $"{_context.CurrencySymbol} ";
            numberFormat.CurrencyNegativePattern = 1;
            _context.NumberFormat = numberFormat;

            _logger.LogInformation("Loading theme");
            _themeService.SetStyle();

            _logger.LogInformation("Setting up main page.");

            //MainPage = new NavigationPage(ServiceLocator.Default.GetInstance<LoginView>());

            //var refreshToken = string.Empty;

            //if (Preferences.ContainsKey(nameof(Token.RefreshToken)))
            //    refreshToken = Preferences.Get(nameof(Token.RefreshToken), string.Empty);

            //if (!_context.IsAuthenticated && !string.IsNullOrEmpty(refreshToken))
            //    await _authenticationService.AuthenticateWithRefreshTokenAsync(refreshToken);

            _logger.LogInformation("Finishing initialization of application");
        }

        /// <summary>
        /// Get a new list of additional resource dictionaries which can be merged.
        /// </summary>
        /// <returns>IList&lt;ResourceDictionary&gt;.</returns>
        protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>();

        /// <summary>
        /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new Window();
            MainWindow.Activate();

            //MainWindow.Activated += OnActivated;

            _themeService.InitializeMainWindow(MainWindow);
            _themeService.SetTitlebar();

            InitializeApplication();

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

            var shellView = ServiceLocator.Default.GetInstance<IShellView>();
            var shellViewModel = ServiceLocator.Default.GetInstance<IShellViewModel>();
            var context = ServiceLocator.Default.GetInstance<IContext>();

            Argument.IsNotNull(context);
            Argument.IsNotNull(shellView);

            if (rootFrame.Content is null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(shellView, e.Args);
            }

            //MainWindow.Title = context.Title;
            //MainWindow.Content = (UIElement)shellView;
            
            MainWindow.Show();
            MainWindow.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        /// <exception cref="Result.Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
            throw new Exception($"Failed to load {e.Uri}: {e.Exception}");

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="loglevel">The loglevel.</param>
        /// <returns>ILoggerFactory.</returns>
        protected virtual ILoggerFactory ConfigureLogger(LogLevel loglevel = LogLevel.Information)
        {
            var factory = LoggerFactory.Create(builder =>
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
    }
}
