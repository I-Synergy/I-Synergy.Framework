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
        protected readonly IExceptionHandlerService _exceptionHandlerService;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected readonly ILogger _logger;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected readonly IContext _context;

        protected readonly IThemeService _themeService;
        protected readonly IAuthenticationService _authenticationService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IBaseApplicationSettingsService _applicationSettingsService;

        private Task Initialize { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseApplication()
            : base()
        {
            _logger = ServiceLocator.Default.GetInstance<ILogger>();
            _logger.LogInformation("Starting application");

            // Pass a timeout to limit the execution time.
            // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            DispatcherUnhandledException += BaseApplication_DispatcherUnhandledException;

            _context = ServiceLocator.Default.GetInstance<IContext>();
            _authenticationService = ServiceLocator.Default.GetInstance<IAuthenticationService>();
            _themeService = ServiceLocator.Default.GetInstance<IThemeService>();
            _exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

            _applicationSettingsService = ServiceLocator.Default.GetInstance<IBaseApplicationSettingsService>();
            _applicationSettingsService.LoadSettings();

            _localizationService = ServiceLocator.Default.GetInstance<ILocalizationService>();

            if (_applicationSettingsService.Settings is not null)
                _localizationService.SetLocalizationLanguage(_applicationSettingsService.Settings.Culture);

            _logger.LogInformation("Starting initialization of application");

            InitializeApplication();

            _logger.LogInformation("Finishing initialization of application");
        }

        protected virtual async void BaseApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_exceptionHandlerService is not null)
                await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            else
                _logger.LogCritical(e.Exception, e.Exception.Message);

            e.Handled = true;
        }

        protected virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception.Message);
        }

        protected virtual async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (_exceptionHandlerService is not null)
                await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            else
                _logger.LogCritical(e.Exception, e.Exception.Message);

            e.SetObserved();
        }

        protected virtual async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                if (_exceptionHandlerService is not null)
                    await _exceptionHandlerService.HandleExceptionAsync(exception);
                else
                    _logger.LogCritical(exception, exception.Message);
        }

        public void InitializeApplication() => Initialize = InitializeApplicationAsync();

        public virtual Task InitializeApplicationAsync()
        {
            var culture = CultureInfo.CurrentCulture;
            var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
            numberFormat.CurrencySymbol = $"{_context.CurrencySymbol} ";
            numberFormat.CurrencyNegativePattern = 1;
            _context.NumberFormat = numberFormat;

            return Task.CompletedTask;
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
                rootFrame.Content = shellView;
            }

            _logger.LogInformation("Loading theme");
            _themeService.SetStyle();

            MainWindow.Title = context.Title ?? string.Empty;
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
    }
}
