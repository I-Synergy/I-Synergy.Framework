using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Exceptions;
using ISynergy.Framework.UI.Views;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;

[assembly: ExportFont("ISynergy.ttf", Alias = "ISynergy")]
[assembly: ExportFont("OpenDyslexic3-Bold.ttf", Alias = "OpenDyslexic3-Bold")]
[assembly: ExportFont("OpenDyslexic3-Regular.ttf", Alias = "OpenDyslexic3-Regular")]
[assembly: ExportFont("SegMDL2.ttf", Alias = "SegoeMdl2")]
[assembly: ExportFont("segoeui.ttf", Alias = "SegoeUI")]

namespace ISynergy.Framework.UI;

public abstract class BaseApplication : Application, IBaseApplication, IDisposable
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

    public AppTheme Theme { get; set; } = AppTheme.Dark;

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

        _logger.LogInformation("Setting up main page.");
        MainPage = new LoadingView();

        _logger.LogInformation("Setting up global exception handler.");
        SetGlobalExceptionHandler();

        _logger.LogInformation("Setting up context.");
        _context = ServiceLocator.Default.GetInstance<IContext>();

        _logger.LogInformation("Setting up authentication service.");
        _authenticationService = ServiceLocator.Default.GetInstance<IAuthenticationService>();
        _authenticationService.AuthenticationChanged += AuthenticationChanged;

        _logger.LogInformation("Setting up theming service.");
        _themeService = ServiceLocator.Default.GetInstance<IThemeService>();

        _logger.LogInformation("Setting up exception handler service.");
        _exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

        _logger.LogInformation("Setting up application settings service.");
        _applicationSettingsService = ServiceLocator.Default.GetInstance<IBaseApplicationSettingsService>();
        _applicationSettingsService.LoadSettings();

        _logger.LogInformation("Setting up localization service.");
        _localizationService = ServiceLocator.Default.GetInstance<ILocalizationService>();

        if (_applicationSettingsService.Settings is not null)
            _localizationService.SetLocalizationLanguage(_applicationSettingsService.Settings.Language);

        _logger.LogInformation("Setting style.");
        _themeService.SetStyle();

        _logger.LogInformation("Starting initialization of application");
        InitializeApplication();

        _logger.LogInformation("Finishing initialization of application");
    }

    public abstract void AuthenticationChanged(object sender, ReturnEventArgs<bool> e);

    protected virtual void SetGlobalExceptionHandler()
    {
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        MauiExceptions.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    protected virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        Debug.WriteLine(e.Exception.Message);
    }

    protected virtual void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.Message);

        e.SetObserved();
    }

    protected virtual void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                _exceptionHandlerService.HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.Message);
    }

    public void InitializeApplication() => Initialize = InitializeApplicationAsync();

    /// <summary>
    /// LoadAssembly the application.
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

    #region IDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // NOTE: Leave out the finalizer altogether if this class doesn't
    // own unmanaged resources, but leave the other methods
    // exactly as they are.
    //~ObservableClass()
    //{
    //    // Finalizer calls Dispose(false)
    //    Dispose(false);
    //}

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            _authenticationService.AuthenticationChanged -= AuthenticationChanged;
            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            MauiExceptions.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }

        // free native resources if there are any.
    }
    #endregion
}
