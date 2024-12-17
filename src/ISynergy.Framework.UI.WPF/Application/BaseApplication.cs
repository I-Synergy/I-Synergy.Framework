using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class BaseApplication : Application, IBaseApplication, IDisposable
{
    protected readonly IExceptionHandlerService _exceptionHandlerService;
    protected readonly IScopedContextService _scopedContextService;
    protected readonly ILogger _logger;
    protected readonly IThemeService _themeService;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ISettingsService _settingsService;
    protected readonly IBaseCommonServices _commonServices;

    protected IContext _context;

    private Task Initialize { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected BaseApplication(IScopedContextService scopedContextService)
        : base()
    {
        _scopedContextService = scopedContextService;

        _logger = _scopedContextService.GetService<ILogger>();
        _logger.LogInformation("Starting application");

        // Pass a timeout to limit the execution time.
        // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

        _logger.LogInformation("Setting up global exception handler.");
        SetGlobalExceptionHandler();

        _logger.LogTrace("Starting initialization of application");

        _logger.LogTrace("Setting up main page.");

        _logger.LogTrace("Getting common services.");
        _commonServices = _scopedContextService.GetService<IBaseCommonServices>();
        _commonServices.BusyService.StartBusy();

        _logger.LogInformation("Setting up context.");
        _context = _scopedContextService.GetService<IContext>();

        _logger.LogInformation("Setting up authentication service.");
        _authenticationService = _scopedContextService.GetService<IAuthenticationService>();
        _authenticationService.AuthenticationChanged += AuthenticationChanged;

        _logger.LogInformation("Setting up theming service.");
        _themeService = _scopedContextService.GetService<IThemeService>();

        _logger.LogInformation("Setting up exception handler service.");
        _exceptionHandlerService = _scopedContextService.GetService<IExceptionHandlerService>();

        _logger.LogInformation("Setting up application settings service.");
        _settingsService = _scopedContextService.GetService<ISettingsService>();

        _logger.LogInformation("Setting up localization service.");

        if (_settingsService.LocalSettings is not null)
            _settingsService.LocalSettings.Language.SetLocalizationLanguage(_context);

        _logger.LogInformation("Setting style.");
        MessageService.Default.Register<StyleChangedMessage>(this, m => StyleChanged(m));
        _themeService.SetStyle();

        _logger.LogInformation("Starting initialization of application");

        InitializeApplication();

        _logger.LogInformation("Finishing initialization of application");
    }

    /// <summary>
    /// Handles the style changed event.
    /// </summary>
    /// <param name="m"></param>
    public virtual void StyleChanged(StyleChangedMessage m) { }

    /// <summary>
    /// Handles the authentication changed event.   
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public abstract void AuthenticationChanged(object sender, Core.Events.ReturnEventArgs<bool> e);

    /// <summary>
    /// Sets the global exception handler.
    /// </summary>
    protected virtual void SetGlobalExceptionHandler()
    {
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        DispatcherUnhandledException += BaseApplication_DispatcherUnhandledException;
    }

    /// <summary>
    /// Handles the dispatcher unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void BaseApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.Handled = true;
    }

    /// <summary>
    /// Handles the first chance exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        Debug.WriteLine(e.Exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Handles the unobserved task exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                await _exceptionHandlerService.HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Initializes the application asynchronously.
    /// </summary>
    private void InitializeApplication() => Initialize = InitializeApplicationAsync();

    /// <summary>
    /// LoadAssembly the application.
    /// </summary>
    /// <example>
    /// <code>
    ///     await base.InitializeApplicationAsync();
    ///     // wait 5 seconds before showing the main window...
    ///     await Task.Delay(5000);
    ///     await _scopedContextService.GetService{INavigationService}().ReplaceMainWindowAsync{IShellView}();
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
            if (Application.Current.Resources?.MergedDictionaries is not null)
            {
                foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
                {
                    if (!Application.Current.Resources.MergedDictionaries.Contains(item))
                        Application.Current.Resources.MergedDictionaries.Add(item);
                }
            }

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        rootFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

        MessageService.Default.Register<EnvironmentChangedMessage>(this, m =>
        {
            _commonServices.InfoService?.SetTitle(m.Content);
        });

        MainWindow.Show();
        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    /// <exception cref="Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
        throw new Exception($"Failed to load {e.Uri}: {e.Exception}");

    public virtual Task HandleCommandLineArgumentsAsync(string[] e) =>
        Task.CompletedTask;

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
            MessageService.Default.Unregister<StyleChangedMessage>(this);
            MessageService.Default.Unregister<EnvironmentChangedMessage>(this);

            if (_authenticationService is not null)
                _authenticationService.AuthenticationChanged -= AuthenticationChanged;

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            DispatcherUnhandledException -= BaseApplication_DispatcherUnhandledException;
        }

        // free native resources if there are any.
    }
    #endregion
}
