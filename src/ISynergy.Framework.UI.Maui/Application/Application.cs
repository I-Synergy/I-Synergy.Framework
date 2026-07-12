using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Globalization.Extensions;
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
using System.Net.Sockets;

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

    protected readonly string _language;
    protected readonly ApplicationFeatures? _features;
    protected readonly SplashScreenOptions _splashScreenOptions;
    protected LoadingView? _loadingView;
    protected Microsoft.Maui.Controls.Window? _mainWindow;

    /// <summary>
    /// Gets the icon to display in the title bar. Override to provide a custom icon.
    /// Return null or empty to omit the icon.
    /// </summary>
    protected virtual ImageSource? Icon => null;

    /// <summary>
    /// Creates the title bar for the main window. Override to provide a custom title bar.
    /// </summary>
    protected virtual TitleBar SetTitleBar()
    {
        var titleBar = new TitleBar();

        if (Icon != null)
            titleBar.Icon = Icon;

        if (!string.IsNullOrEmpty(InfoService.Default.ProductName))
            titleBar.Title = InfoService.Default.ProductName;

        var isRightToLeft = CultureInfo.GetCultureInfo(_language).TextInfo.IsRightToLeft;

        if (isRightToLeft)
            titleBar.FlowDirection = FlowDirection.RightToLeft;
        else
            titleBar.FlowDirection = FlowDirection.LeftToRight;

        return titleBar;
    }

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

            // Get logger first for proper error tracking
            try
            {
                _logger = IPlatformApplication.Current!.Services.GetRequiredService<ILogger<Application>>();
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

                _commonServices = IPlatformApplication.Current!.Services.GetRequiredService<ICommonServices>();
                _dialogService = IPlatformApplication.Current!.Services.GetRequiredService<IDialogService>();
                _navigationService = IPlatformApplication.Current!.Services.GetRequiredService<INavigationService>();
                _exceptionHandlerService = IPlatformApplication.Current!.Services.GetRequiredService<IExceptionHandlerService>();
                _settingsService = IPlatformApplication.Current!.Services.GetRequiredService<ISettingsService>();
                _themeService = IPlatformApplication.Current!.Services.GetRequiredService<IThemeService>();
                _lifecycleService = IPlatformApplication.Current!.Services.GetRequiredService<IApplicationLifecycleService>();
                _messengerService = IPlatformApplication.Current!.Services.GetRequiredService<IMessengerService>();

                _features = IPlatformApplication.Current!.Services.GetRequiredService<IOptions<ApplicationFeatures>>().Value;

                // Pass a timeout to limit the execution time.
                // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
                AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

                _commonServices.BusyService.StartBusy();

                // Always initialize the culture so DefaultThreadCurrentCulture is never null.
                // Falls back to the OS culture when no language is persisted yet.
                _language = _settingsService.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;

                var initialCulture = !string.IsNullOrEmpty(_language)
                    ? CultureInfo.GetCultureInfo(_language)
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
        // Always use LoadingView — it provides the default splash.svg background
        // when no custom image or video is configured
        var mainPage = CreateLoadingView();
        mainPage.ApplyFlowDirection(_language);

        _mainWindow = new Microsoft.Maui.Controls.Window(mainPage);
        _mainWindow.ApplyFlowDirection(_language);
        _mainWindow.TitleBar = SetTitleBar();

        _themeService.ApplyTheme();

        return _mainWindow;
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

            var configuration = IPlatformApplication.Current!.Services.GetRequiredService<IConfiguration>();

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
        // MAUI probes for missing image density variants and missing local settings files at
        // startup; these exceptions are caught internally and are not fatal. Skip them here so
        // they are not forwarded to Sentry as errors.
        // Also skip IOException wrapping SocketException with OperationAborted/ConnectionAborted,
        // which occurs when the OS suspends in-flight socket reads while the app is backgrounded.
        // Skip TaskCanceledException and OperationCanceledException; task/operation cancellation is routine, not an error.
        if (e.Exception is FileNotFoundException { Message: null or "" }
            || e.Exception is FileNotFoundException fnfEx
                && (fnfEx.FileName?.EndsWith(".resources", StringComparison.OrdinalIgnoreCase) == true
                    || fnfEx.FileName?.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase) == true)
            || e.Exception is InvalidOperationException { Message: "Unable to load image file." }
            || (e.Exception is IOException ioEx && ioEx.InnerException is SocketException socketEx
                && (socketEx.SocketErrorCode == SocketError.OperationAborted || socketEx.SocketErrorCode == SocketError.ConnectionAborted))
            || e.Exception is OperationCanceledException)
            return;

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
