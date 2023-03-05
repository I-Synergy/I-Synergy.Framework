using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Exceptions;
using ISynergy.Framework.UI.Views;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;

namespace ISynergy.Framework.UI
{
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
            _logger = ServiceLocator.Default.GetInstance<ILogger>() ?? LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Trace);
            }).CreateLogger(AppDomain.CurrentDomain.FriendlyName);

            _logger.LogInformation("Starting application");

            // Pass a timeout to limit the execution time.
            // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            MauiExceptions.UnhandledException += CurrentDomain_UnhandledException;
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

            _logger.LogInformation("Starting initialization of application");

            _logger.LogInformation("Loading theme");
            _themeService.SetStyle();

            _logger.LogInformation("Setting up main page.");

            MainPage = new LoadingView();

            InitializeApplication();

            _logger.LogInformation("Finishing initialization of application");
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

        /// <summary>
        /// Initialize the application.
        /// </summary>
        /// <example>
        /// <code>
        ///     await base.InitializeApplicationAsync();
        ///     // wait 5 seconds before showing the main window...
        ///     await Task.Delay(5000);
        ///     await ServiceLocator.Default.GetInstance{INavigationService}().ReplaceMainWindowAsync{IShellView}();
        /// </code>
        /// </example>
        /// <returns></returns>
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
        public virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>();
    }
}
