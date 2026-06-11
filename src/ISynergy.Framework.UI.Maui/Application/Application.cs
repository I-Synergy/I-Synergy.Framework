using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Messages;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Exceptions;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Globalization.Extensions;


#if WINDOWS
using Microsoft.UI.Xaml.Media;
using ISynergy.Framework.UI.Helpers;
#endif

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI;

public abstract class Application : Microsoft.Maui.Controls.Application, IDisposable
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
    protected Microsoft.Maui.Controls.Window? _mainWindow;


    private readonly IDispatcherTimer _clockTimer;
    private readonly TitleBar _titleBar = new TitleBar();

    private Task? Initialize { get; set; }
    private int lastErrorMessage = 0;

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application(SplashScreenOptions? splashScreenOptions = null)
        : base()
    {
        try
        {
            _splashScreenOptions = splashScreenOptions ?? new SplashScreenOptions(SplashScreenTypes.None);

            _clockTimer = Dispatcher.CreateTimer();
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Tick += ClockTimerCallback;
            _clockTimer.Start();

            // Get logger first for proper error tracking
            try
            {
                _logger = ServiceLocator.Default.GetRequiredService<ILogger<Application>>();
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
                MauiExceptions.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                _commonServices = ServiceLocator.Default.GetRequiredService<ICommonServices>();
                _dialogService = ServiceLocator.Default.GetRequiredService<IDialogService>();
                _navigationService = ServiceLocator.Default.GetRequiredService<INavigationService>();
                _exceptionHandlerService = ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>();
                _settingsService = ServiceLocator.Default.GetRequiredService<ISettingsService>();
                _themeService = ServiceLocator.Default.GetRequiredService<IThemeService>();
                _lifecycleService = ServiceLocator.Default.GetRequiredService<IApplicationLifecycleService>();
                _messengerService = ServiceLocator.Default.GetRequiredService<IMessengerService>();

                _features = ServiceLocator.Default.GetRequiredService<IOptions<ApplicationFeatures>>().Value;

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

                _lifecycleService.ApplicationLoaded += OnApplicationLoaded; // NOSONAR - virtual member subscription is intentional

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
                InitializeEnvironmentVariables();
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Critical error during application initialization"); // NOSONAR
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

    protected override void OnStart()
    {
        _logger?.LogTrace("Application OnStart called");
        InitializeApplication();
    }

    protected override Microsoft.Maui.Controls.Window CreateWindow(IActivationState? activationState)
    {
        // Apply RTL flow direction from saved settings so every MAUI app
        // gets correct layout from the very first frame — no per-app code needed.
        var language = _settingsService?.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;

        Microsoft.Maui.Controls.Page mainPage;

        // Always use LoadingView — it provides the default splash.svg background
        // when no custom image or video is configured
        mainPage = CreateLoadingView();

        mainPage.ApplyFlowDirection(language);

        _mainWindow = new Microsoft.Maui.Controls.Window(mainPage);
        _mainWindow.ApplyFlowDirection(language);

        var isRightToLeft = CultureInfo.GetCultureInfo(language).TextInfo.IsRightToLeft;

        if (isRightToLeft)
            _titleBar.FlowDirection = FlowDirection.RightToLeft;
        else
            _titleBar.FlowDirection = FlowDirection.LeftToRight;

        _titleBar.Icon = "icon.png";

        SetClock();

        _mainWindow.TitleBar = _titleBar;
        _mainWindow.Title = InfoService.Default.ProductName ?? string.Empty;

#if WINDOWS
        // Defer theme application until the native WinUI window handler is
        // attached, so title bar colors can be set from the very first frame.
        _mainWindow.HandlerChanged += OnMainWindowHandlerChanged;

        void OnMainWindowHandlerChanged(object? s, EventArgs e)
        {
            if (_mainWindow?.Handler?.PlatformView is Microsoft.UI.Xaml.Window win)
            {
                _mainWindow.HandlerChanged -= OnMainWindowHandlerChanged;
                var appWindow = win.GetAppWindow();
                appWindow?.ApplyFlowDirectionToTitleBar(language);
                // Theme was already applied in the constructor for accent/style.
                // Re-apply here to update title bar colors now that the window exists.
                _themeService.ApplyTheme();
            }
        }
#else
        // For non-Windows platforms, apply theme immediately since title bar styling isn't a concern.
        _themeService.ApplyTheme();
#endif

        return _mainWindow;
    }

    private void ClockTimerCallback(object? sender, object e) => SetClock();

    private void SetClock()
    {
        if (ServiceLocator.Default.GetRequiredService<IContext>() is not null)
            _titleBar.Title = $"{_commonServices.InfoService.ProductName} v{_commonServices.InfoService.ProductVersion} ({Environment.GetEnvironmentVariable(nameof(Environment))}) - {DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";
    }

    /// <summary>
    /// Override to customize the <see cref="NumberFormatInfo"/> of the application
    /// culture (currency symbol, decimal places, negative patterns, etc.).
    /// </summary>
    /// <param name="culture">A mutable clone of the culture to customize.</param>
    protected virtual void ConfigureNumberFormat(CultureInfo culture)
    {
        // Default: no customization. Override in derived classes.
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

            var environmentFromConfig = configuration.GetValue<string?>(nameof(Environment), null);

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
    /// Allows to add or update platform specific handlers.
    /// </summary>
    public virtual void UpdateMauiHandlers(Style style)
    {
    }

    /// <summary>
    /// Handles the first chance exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        if (e.Exception.HResult != lastErrorMessage)
        {
            lastErrorMessage = e.Exception.HResult;
            _logger.LogError(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
        }
    }

    /// <summary>
    /// Handles the unobserved task exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleException(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                _exceptionHandlerService.HandleException(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    #region IDisposable
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

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
            MauiExceptions.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;

            if (_lifecycleService is not null)
                _lifecycleService.ApplicationLoaded -= OnApplicationLoaded;
        }
    }
    #endregion
}
