using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Messages;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Globalization;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class Application : Microsoft.UI.Xaml.Application, IDisposable
{
    protected readonly IExceptionHandlerService _exceptionHandlerService;
    protected readonly ICommonServices _commonServices;
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;
    protected readonly ISettingsService _settingsService;
    protected readonly IApplicationLifecycleService _lifecycleService;
    protected readonly ILogger<Application> _logger;

    protected readonly ApplicationFeatures? _features;
    protected readonly SplashScreenOptions _splashScreenOptions;

    protected Microsoft.UI.Xaml.Window? _mainWindow;

    private int _lastErrorMessage = 0;
    private bool _isDisposing = false;
    private readonly Dictionary<string, Exception> _processedExceptions = new Dictionary<string, Exception>();

    /// <summary>
    /// Gets the current main window from the running application instance
    /// </summary>
    public static Microsoft.UI.Xaml.Window? MainWindow
    {
        get
        {
            if (Application.Current is Application app)
                return app._mainWindow;

            return null;
        }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application(SplashScreenOptions? splashScreenOptions = null)
        : base()
    {
        try
        {
            _splashScreenOptions = splashScreenOptions ?? new SplashScreenOptions(SplashScreenTypes.None);

            // Create host and set up dependency injection
            IHost? host = null;

            try
            {
                host = CreateHostBuilder()
                    .Build()
                    .SetLocatorProvider();
            }
            catch (Exception ex)
            {
                // Log startup error and rethrow to prevent partial initialization
                Debug.WriteLine($"Critical error during host creation: {ex}");
                throw;
            }

            // Get logger first for proper error tracking
            try
            {
                _logger = host.Services.GetRequiredService<ILogger<Application>>();
                _logger.LogTrace("Application constructor started");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get logger: {ex}");
                throw;
            }

            try
            {
                _logger.LogTrace("Setting up services and global exception handler.");

                _commonServices = ServiceLocator.Default.GetRequiredService<ICommonServices>();
                _dialogService = ServiceLocator.Default.GetRequiredService<IDialogService>();
                _navigationService = ServiceLocator.Default.GetRequiredService<INavigationService>();
                _exceptionHandlerService = ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>();
                _settingsService = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>();
                _lifecycleService = _commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>();

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

                MessengerService.Default.Register<ShowInformationMessage>(this, async m =>
                {
                    var dialogResult = await _dialogService.ShowInformationAsync(m.Content.Message, m.Content.Title);
                });

                MessengerService.Default.Register<ShowWarningMessage>(this, async m =>
                {
                    var dialogResult = await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title);
                });

                MessengerService.Default.Register<ShowErrorMessage>(this, async m =>
                {
                    var dialogResult = await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title);
                });

                // Initialize environment variables from configuration and command-line parameters
                InitializeEnvironmentVariables();
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Critical error during application initialization");
                throw;
            }
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
    protected abstract IHostBuilder CreateHostBuilder();

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

    /// <summary>
    /// Get a new list of additional resource dictionaries which can be merged.
    /// </summary>
    /// <returns>IList&lt;ResourceDictionary&gt;.</returns>
    protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
        new List<ResourceDictionary>();

    /// <summary>
    /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
    /// </summary>
    /// <param name="args">Event data for the event.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        try
        {
            if (_settingsService?.LocalSettings is not null)
            {
                try
                {
                    _mainWindow = WindowHelper.CreateWindow(_settingsService.LocalSettings.Theme);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Error setting application theme");
                }
            }

            _logger?.LogTrace("Loading custom resource dictionaries");

            if (Application.Current.Resources?.MergedDictionaries is not null)
            {
                var additionalDictionaries = GetAdditionalResourceDictionaries().EnsureNotNull();

                foreach (var item in additionalDictionaries)
                {
                    try
                    {
                        if (!Application.Current.Resources.MergedDictionaries.Contains(item))
                            Application.Current.Resources.MergedDictionaries.Add(item);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Error adding resource dictionary");
                    }
                }
            }

            _logger?.LogTrace("Setting up theming.");

            if (_settingsService?.LocalSettings is not null)
            {
                try
                {
                    this.SetApplicationColor(_settingsService.LocalSettings.Color);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Error setting application color");
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Critical error in OnLaunched");

            // Create a basic error window as a last resort
            try
            {
                var errorWindow = new Microsoft.UI.Xaml.Window();
                var textBlock = new Microsoft.UI.Xaml.Controls.TextBlock
                {
                    Text = $"Critical application error: {ex.Message}",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap
                };

                var grid = new Microsoft.UI.Xaml.Controls.Grid();
                grid.Children.Add(textBlock);

                errorWindow.Content = grid;
                errorWindow.Activate();
            }
            catch
            {
                // Last resort - can't even show an error window
                Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    /// <exception cref="Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
    protected virtual void OnNavigationFailed(object? sender, NavigationFailedEventArgs e)
    {
        var exception = new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        _logger?.LogError(exception, "Navigation failed");

        if (_exceptionHandlerService != null)
        {
            try
            {
                _exceptionHandlerService.HandleExceptionAsync(exception).ConfigureAwait(false);
            }
            catch
            {
                // If exception handler fails, throw the original exception
                throw exception;
            }
        }
        else
        {
            throw exception;
        }
    }

    protected virtual async Task HandleLaunchArgumentsAsync(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        try
        {
            _logger?.LogTrace("Handling launch event arguments");

            if (args.UWPLaunchActivatedEventArgs is not null)
            {
                switch (args.UWPLaunchActivatedEventArgs.Kind)
                {
                    case Windows.ApplicationModel.Activation.ActivationKind.Launch:
                        await HandleLaunchActivationAsync(args.UWPLaunchActivatedEventArgs.Arguments);
                        break;
                    case Windows.ApplicationModel.Activation.ActivationKind.Protocol:
                        await HandleProtocolActivationAsync(args.UWPLaunchActivatedEventArgs.Arguments);
                        break;
                }
            }

            _logger?.LogTrace("Handle command line arguments");

            if (Environment.GetCommandLineArgs().Length > 1)
                await HandleCommandLineArgumentsAsync(Environment.GetCommandLineArgs());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling launch arguments");
        }
    }

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
                MessengerService.Default.Unregister<ShowInformationMessage>(this);
                MessengerService.Default.Unregister<ShowWarningMessage>(this);
                MessengerService.Default.Unregister<ShowErrorMessage>(this);

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

