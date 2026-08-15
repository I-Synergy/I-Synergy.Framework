using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.Messages;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using Windows.UI;

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
    protected readonly IThemeService _themeService;
    protected readonly ILogger<Application> _logger;
    protected readonly IMessengerService _messengerService;

    protected readonly ApplicationFeatures? _features;
    protected readonly SplashScreenOptions _splashScreenOptions;
    protected LoadingView? _loadingView;

    protected Color? _accentColor;
    protected IServiceProvider? _services;
    private static IServiceProvider? _staticServices;
    protected Microsoft.UI.Xaml.Window? _mainWindow;

    private Task? Initialize { get; set; }
    private int lastErrorMessage = 0;

    /// <summary>
    /// Gets the current main window from the running application instance
    /// </summary>
    public static Microsoft.UI.Xaml.Window MainWindow
    {
        get
        {
            if (Application.Current is Application app)
                return app._mainWindow!;

            throw new InvalidOperationException("Main window not available.");
        }
    }

    public static IServiceProvider Services
    {
        get
        {
            // Prefer the instance field when Application.Current is available and has a value
            if (Application.Current is Application app && app._services is not null)
                return app._services;

            // Fall back to the static field, set before the Application is constructed
            if (_staticServices is not null)
                return _staticServices;

            throw new InvalidOperationException("Services not available.");
        }
        internal set
        {
            _staticServices = value;
            if (Application.Current is Application app)
                app._services = value;
        }
    }

    public static CancellationToken CancellationToken
    {
        get; internal set;
    }

    public static Color? AccentColor
    {
        get
        {
            if (Application.Current is Application app)
                return app._accentColor;

            return null;
        }
        internal set
        {
            if (Application.Current is Application app && value is Color color)
            {
                app._accentColor = color;
                app.SetApplicationColor(color);
            }
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

            // Get logger first for proper error tracking
            try
            {
                _logger = Services.GetRequiredService<ILogger<Application>>();
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

                AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                CommandExceptionDispatcher.UnhandledCommandException += CommandExceptionDispatcher_UnhandledCommandException;

                _commonServices = Services.GetRequiredService<ICommonServices>();
                _dialogService = Services.GetRequiredService<IDialogService>();
                _navigationService = Services.GetRequiredService<INavigationService>();
                _exceptionHandlerService = Services.GetRequiredService<IExceptionHandlerService>();
                _settingsService = Services.GetRequiredService<ISettingsService>();
                _themeService = Services.GetRequiredService<IThemeService>();
                _lifecycleService = Services.GetRequiredService<IApplicationLifecycleService>();
                _messengerService = Services.GetRequiredService<IMessengerService>();

                _features = Services.GetRequiredService<IOptions<ApplicationFeatures>>().Value;

                // Pass a timeout to limit the execution time.
                // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
                AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

                _commonServices.BusyService.StartBusy();

                // Always initialize the culture so DefaultThreadCurrentCulture is never null.
                // Falls back to the OS culture when no language is persisted yet.
                var initialCulture = !string.IsNullOrEmpty(_settingsService.LocalSettings?.Language)
                    ? CultureInfo.GetCultureInfo(_settingsService.LocalSettings.Language)
                    : CultureInfo.CurrentCulture;
                initialCulture.ApplyCulture(ConfigureNumberFormat);

                _lifecycleService.ApplicationLoaded += OnApplicationLoaded;

                _messengerService.Register<ShowInformationMessage>(this, async m =>
                {
                    await _dialogService.ShowInformationAsync(m.Content.Message, m.Content.Title);
                });

                _messengerService.Register<ShowWarningMessage>(this, async m =>
                {
                    await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title);
                });

                _messengerService.Register<ShowErrorMessage>(this, async m =>
                {
                    await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title);
                });

                // Initialize environment variables from configuration and command-line parameters
                InitializeEnvironmentVariables(); // NOSONAR
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
    /// Called when the application is fully loaded (UI ready + initialization complete).
    /// Override or subscribe to this event to handle post-load operations.
    /// </summary>
    protected virtual void OnApplicationLoaded(object? sender, EventArgs e)
    {
        _logger?.LogTrace("Application lifecycle event: ApplicationLoaded raised");
    }

    /// <summary>
    /// Creates a loading view page and stores a reference for completion tracking.
    /// </summary>
    private LoadingView CreateLoadingView()
    {
        _loadingView = new LoadingView(_commonServices, _splashScreenOptions);
        return _loadingView;
    }

    /// <summary>
    /// Waits for the loading view to complete if it's a video splash screen.
    /// For Image and None types, returns immediately.
    /// </summary>
    protected async Task WaitForLoadingViewAsync()
    {
        // Only wait for LoadingView if splash screen type is Video
        if (_splashScreenOptions.SplashScreenType == SplashScreenTypes.Video && _loadingView is LoadingView loadingView)
        {
            _logger?.LogInformation("Waiting for video loading view to complete...");
            await loadingView.WaitForLoadingCompleteAsync();
            _logger?.LogInformation("Video loading view completed");
        }
        // For Image and None types, this returns immediately
    }

    /// <summary>
    /// Initializes the application asynchronously.
    /// </summary>
    private void InitializeApplication() => Initialize = InitializeApplicationAsync();

    /// <summary>
    /// Initializing the application.
    /// </summary>
    /// <returns></returns>
    protected abstract Task InitializeApplicationAsync();

    /// <summary>
    /// Get a new list of additional resource dictionaries which can be merged.
    /// </summary>
    /// <returns>IList&lt;ResourceDictionary&gt;.</returns>
    protected virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
        new List<ResourceDictionary>();

    /// <summary>
    /// Initializes environment variables from appsettings.json with command-line parameter override.
    /// </summary>
    protected virtual void InitializeEnvironmentVariables()
    {
        try
        {
            _logger?.LogTrace("Initializing environment variables");

            var configuration = Services.GetRequiredService<IConfiguration>();

            var environmentFromConfig = configuration[nameof(Environment)];

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
    /// Override to customize the <see cref="NumberFormatInfo"/> of the application
    /// culture (currency symbol, decimal places, negative patterns, etc.).
    /// </summary>
    protected virtual void ConfigureNumberFormat(CultureInfo culture)
    {
    }

    /// <summary>
    /// Normalizes environment value to proper casing (e.g., "development" -> "Development").
    /// </summary>
    private static string NormalizeEnvironmentValue(string value)
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
        // Skip IOException wrapping SocketException with OperationAborted/ConnectionAborted,
        // which occurs when the OS suspends in-flight socket reads while the app is backgrounded.
        // Skip TaskCanceledException and OperationCanceledException; task/operation cancellation is routine, not an error.
        if (e.Exception is IOException ioEx && ioEx.InnerException is SocketException socketEx
                && (socketEx.SocketErrorCode == SocketError.OperationAborted || socketEx.SocketErrorCode == SocketError.ConnectionAborted)
            || e.Exception is OperationCanceledException)
            return;

        if (e.Exception.HResult != lastErrorMessage)
        {
            lastErrorMessage = e.Exception.HResult;
            _logger.LogError(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
        }
    }

    /// <summary>
    /// Handles the unobserved task exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleException(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void CurrentDomain_UnhandledException(object? sender, System.UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                _exceptionHandlerService.HandleException(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Handles an exception thrown by a synchronous relay command's execute delegate
    /// (e.g. a plain <c>RelayCommand</c> bound to a UI control). Routing it here instead of
    /// letting it propagate synchronously avoids an unhandled exception on the UI thread.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void CommandExceptionDispatcher_UnhandledCommandException(object? sender, CommandExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleException(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Invoked when the application is launched. Override this method to perform application initialization and to display initial content in the associated Window.
    /// </summary>
    /// <param name="args">Event data for the event.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) // NOSONAR
    {
        try
        {
            var theme = _settingsService?.LocalSettings?.Theme ?? ISynergy.Framework.Core.Enumerations.Themes.Default;
            var language = _settingsService?.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;

            try
            {
                _mainWindow = WindowHelper.CreateWindow(theme, language);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error creating main window; falling back to default theme");
                _mainWindow = WindowHelper.CreateWindow(ISynergy.Framework.Core.Enumerations.Themes.Default, language);
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
            _themeService.ApplyTheme();

            // LoadingView shows the spinner for all splash screen types, including None.
            // Its Loaded event fires SignalApplicationUIReady(); InitializeApplicationAsync
            // fires SignalApplicationInitialized(). Both signals are required before
            // ApplicationLifecycleService raises ApplicationLoaded and navigation begins.
            _mainWindow.Content = CreateLoadingView();
            _mainWindow.Activate();

            InitializeApplication();
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
                _exceptionHandlerService.HandleException(exception);
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

    public bool _isClosing = false;

    protected void ExitSuccess()
    {
        if (_isClosing)
        {
            return;
        }

        Exiting?.Invoke();
        _isClosing = true;
    }


    public event Action? Exiting;

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
        if (disposing)
        {
            _messengerService.Unregister<ShowInformationMessage>(this);
            _messengerService.Unregister<ShowWarningMessage>(this);
            _messengerService.Unregister<ShowErrorMessage>(this);

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            CommandExceptionDispatcher.UnhandledCommandException -= CommandExceptionDispatcher_UnhandledCommandException;

            // Only unregister event handlers — the DI container owns the singleton's lifetime
            if (_lifecycleService is not null)
                _lifecycleService.ApplicationLoaded -= OnApplicationLoaded;

            Exiting = null;
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

