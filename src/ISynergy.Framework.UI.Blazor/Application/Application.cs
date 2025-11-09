using ISynergy.Framework.Core.Abstractions.Events;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Messages;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI;

public abstract class Application : ComponentBase
{
    protected readonly IExceptionHandlerService _exceptionHandlerService;
    protected readonly ICommonServices _commonServices;
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;
    protected readonly ISettingsService _settingsService;
    protected readonly IApplicationLifecycleService _lifecycleService;
    protected readonly ILogger<Application> _logger;
    protected readonly IMessengerService _messengerService;

    protected readonly ApplicationFeatures? _features;

    private int _lastErrorMessage = 0;
    private bool _isDisposing = false;

    private readonly Dictionary<string, Exception> _processedExceptions = new Dictionary<string, Exception>();

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application(
        ICommonServices commonServices,
        ISettingsService settingsService,
        ILogger<Application> logger)
        : base()
    {
        _commonServices = commonServices;
        _settingsService = settingsService;
        _logger = logger;

        try
        {
            _logger.LogTrace("Setting up services and global exception handler.");

            // Use injected commonServices instead of ServiceLocator
            // Get remaining services from ServiceLocator as they're not available via constructor injection
            _dialogService = ServiceLocator.Default.GetRequiredService<IDialogService>();
            _navigationService = ServiceLocator.Default.GetRequiredService<INavigationService>();
            _exceptionHandlerService = ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>();
            _lifecycleService = _commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>();
            _messengerService = ServiceLocator.Default.GetRequiredService<IMessengerService>();

            _features = ServiceLocator.Default.GetRequiredService<IOptions<ApplicationFeatures>>().Value;

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Pass a timeout to limit the execution time.
            // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

            _commonServices.BusyService.StartBusy();

            if (_settingsService.LocalSettings is not null)
                _settingsService.LocalSettings.Language.SetLocalizationLanguage();

            _lifecycleService.ApplicationLoaded += OnApplicationLoaded;

            _messengerService.Register<ShowInformationMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowInformationAsync(m.Content.Message, m.Content.Title);
            });

            _messengerService.Register<ShowWarningMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title);
            });

            _messengerService.Register<ShowErrorMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title);
            });

            // Initialize environment variables from configuration and command-line parameters
            InitializeEnvironmentVariables();
        }
        catch (Exception ex)
        {
            // Last resort exception handling if logger isn't available
            Debug.WriteLine($"Fatal error in Application constructor: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Called when the application is fully loaded (UI ready + initialization complete).
    /// Override or subscribe to this event to handle post-load operations.
    /// </summary>
    protected virtual void OnApplicationLoaded(object? sender, EventArgs e)
    {
        _logger?.LogTrace("Application lifecycle event: ApplicationLoaded raised");
    }

    /// <summary>
    /// Initializes environment variables from appsettings.json with command-line parameter override.
    /// </summary>
    protected virtual void InitializeEnvironmentVariables()
    {
        try
        {
            _logger?.LogTrace("Initializing environment variables");

            var configuration = ServiceLocator.Default.GetRequiredService<IConfiguration>();

            var environmentFromConfig = configuration.GetValue<string>(nameof(Environment));

            if (!string.IsNullOrEmpty(environmentFromConfig))
            {
                var normalizedEnvironment = NormalizeEnvironmentValue(environmentFromConfig);
                Environment.SetEnvironmentVariable(nameof(Environment), normalizedEnvironment);
                _logger?.LogTrace("Environment variable set from configuration: {Environment}", normalizedEnvironment);
            }

            // Handle command-line parameters which override configuration
            HandleCommandArguments();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during environment variable initialization");
        }
    }

    /// <summary>
    /// Normalizes environment value to proper casing (e.g., "development" -> "Development").
    /// </summary>
    private string NormalizeEnvironmentValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
    }

    /// <summary>
    /// Handles environment and country parameters from command line arguments.
    /// Command-line parameters override configuration values.
    /// </summary>
    protected virtual void HandleCommandArguments()
    {
        var commandLineArgs = Environment.GetCommandLineArgs();

        foreach (var arg in commandLineArgs.EnsureNotNull())
        {
            if (string.IsNullOrEmpty(arg))
                continue;

            // Look for query string format arguments (e.g., "?environment=development&country=nl")
            if (arg.Contains("?"))
            {
                var environmentValue = arg.ExtractQueryParameter(nameof(Environment));

                if (!string.IsNullOrEmpty(environmentValue))
                {
                    var normalizedEnvironment = NormalizeEnvironmentValue(environmentValue);
                    Environment.SetEnvironmentVariable(nameof(Environment), normalizedEnvironment);
                    _logger?.LogTrace("Environment variable overridden from command-line: {Environment}", normalizedEnvironment);
                }
            }
            else
            {
                _logger?.LogTrace("Command line argument: {Argument}", arg);
            }
        }
    }

    /// <summary>
    /// Handles the first chance exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
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
                _exceptionHandlerService.HandleException(e.Exception);
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
    protected virtual void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
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
                _exceptionHandlerService.HandleException(e.Exception);
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
    protected virtual void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (_isDisposing)
            return;

        if (e.ExceptionObject is Exception exception)
        {
            if (_exceptionHandlerService is not null)
            {
                try
                {
                    _exceptionHandlerService.HandleException(exception);
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
                _messengerService.Unregister<ShowInformationMessage>(this);
                _messengerService.Unregister<ShowWarningMessage>(this);
                _messengerService.Unregister<ShowErrorMessage>(this);

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
