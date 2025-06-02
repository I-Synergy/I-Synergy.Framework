using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ISynergy.Framework.UI;
public abstract class Application : ComponentBase
{
    protected readonly ICommonServices? _commonServices;
    protected readonly ISettingsService? _settingsService;
    protected readonly IExceptionHandlerService? _exceptionHandlerService;
    protected readonly ILogger<Application>? _logger;

    protected readonly ApplicationFeatures? _features;

    private int _lastErrorMessage = 0;
    private bool _isInitialized = false;
    private bool _isDisposing = false;

    private readonly Dictionary<string, Exception> _processedExceptions = new Dictionary<string, Exception>();

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application(
        ICommonServices commonServices,
        ISettingsService settingsService,
        IExceptionHandlerService exceptionHandlerService,
        IOptions<ApplicationFeatures> features,
        ILogger<Application> logger)
        : base()
    {
        _commonServices = commonServices;
        _settingsService = settingsService;
        _exceptionHandlerService = exceptionHandlerService;
        _features = features.Value;
        _logger = logger;

        try
        {
            _logger.LogTrace("Setting up global exception handler.");

            // Set up exception handling
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            _logger.LogTrace("Starting application");

            // Pass a timeout to limit the execution time.
            // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

            _logger.LogTrace("Starting initialization of application");

            _logger.LogTrace("Setting up main page.");
            _commonServices.BusyService.StartBusy();

            _logger.LogTrace("Setting up authentication service.");
            _commonServices.AuthenticationService.AuthenticationChanged += OnAuthenticationChanged;
            _commonServices.AuthenticationService.SoftwareEnvironmentChanged += OnSoftwareEnvironmentChanged;

            _logger.LogTrace("Setting up localization service.");
            if (_settingsService.LocalSettings is not null)
                _settingsService.LocalSettings.Language.SetLocalizationLanguage();

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            // Last resort exception handling if logger isn't available
            Debug.WriteLine($"Fatal error in Application constructor: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <returns></returns>
    protected abstract IHostApplicationBuilder CreateHostBuilder();

    protected abstract void OnAuthenticationChanged(object? sender, ReturnEventArgs<bool> e);

    protected virtual void OnSoftwareEnvironmentChanged(object? sender, ReturnEventArgs<SoftwareEnvironments> e)
    {
        try
        {
            _commonServices?.InfoService?.SetTitle(e.Value);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnSoftwareEnvironmentChanged");
        }
    }

    /// <summary>
    /// Handles the first chance exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual async void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        if (_isDisposing)
            return;

        // Create a unique key for this exception to prevent duplicates
        string exceptionKey = $"{e.Exception.GetType().FullName}:{e.Exception.Message}:{e.Exception.StackTrace?.GetHashCode()}";

        // Check if we've already processed this exact exception
        if (_processedExceptions.ContainsKey(exceptionKey))
            return;

        // Add to processed exceptions with a limit
        if (_processedExceptions.Count > 100)
            _processedExceptions.Clear();

        _processedExceptions[exceptionKey] = e.Exception;

        if (_exceptionHandlerService is not null)
        {
            try
            {
                await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            }
            catch (Exception handlerEx)
            {
                // If exception handler fails, log directly
                _logger?.LogError(handlerEx, "Error in exception handler");
            }
        }
        else
        {
            if (e.Exception.HResult != _lastErrorMessage)
            {
                _lastErrorMessage = e.Exception.HResult;
                _logger?.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
            }
        }
    }

    /// <summary>
    /// Handles the unobserved task exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual async void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_isDisposing)
        {
            e.SetObserved();
            return;
        }

        if (_exceptionHandlerService is not null)
        {
            try
            {
                await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
            }
            catch (Exception handlerEx)
            {
                // If exception handler fails, log directly
                _logger?.LogError(handlerEx, "Error in exception handler");
            }
        }
        else
            _logger?.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual async void CurrentDomain_UnhandledException(object? sender, System.UnhandledExceptionEventArgs e)
    {
        if (_isDisposing)
            return;

        if (e.ExceptionObject is Exception exception)
        {
            if (_exceptionHandlerService is not null)
            {
                try
                {
                    await _exceptionHandlerService.HandleExceptionAsync(exception);
                }
                catch (Exception handlerEx)
                {
                    // If exception handler fails, log directly
                    _logger?.LogError(handlerEx, "Error in exception handler");
                }
            }
            else
                _logger?.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
        }
    }

    /// <summary>
    /// Initializing the application.
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
    protected abstract Task InitializeApplicationAsync();

    protected virtual Task HandleProtocolActivationAsync(string e) =>
        Task.CompletedTask;

    protected virtual Task HandleLaunchActivationAsync(string e) =>
        Task.CompletedTask;

    protected virtual Task HandleCommandLineArgumentsAsync(string[] e) =>
        Task.CompletedTask;

    protected virtual Task HandleApplicationInitializedAsync() =>
        Task.CompletedTask;


    #region IDisposable
    // Dispose() calls Dispose(true)

#if IOS || MACCATALYST
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
#else
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
#endif

    // The bulk of the clean-up code is implemented in Dispose(bool)
#if IOS || MACCATALYST
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected new virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // free managed resources
            MessageService.Default.Unregister<StyleChangedMessage>(this);
        }

        // free native resources if there are any.
        base.Dispose(disposing);
    }
#else
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        _isDisposing = true;

        if (disposing)
        {
            try
            {
                // free managed resources
                if (_commonServices?.AuthenticationService is not null)
                {
                    _commonServices.AuthenticationService.AuthenticationChanged -= OnAuthenticationChanged;
                    _commonServices.AuthenticationService.SoftwareEnvironmentChanged -= OnSoftwareEnvironmentChanged;
                }

                AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during disposal");
            }
        }

        // free native resources if there are any.
    }
#endif

    ~Application()
    {
        Dispose(false);
    }
    #endregion
}
