using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Events;
using ISynergy.Framework.Mvvm.Messages;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class Application : System.Windows.Application, IDisposable
{
    protected readonly IExceptionHandlerService _exceptionHandlerService;
    protected readonly ICommonServices _commonServices;
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;
    protected readonly ISettingsService _settingsService;
    protected readonly IApplicationLifecycleService _lifecycleService;
    protected readonly ILogger<Application> _logger;
    protected readonly IThemeService? _themeService;

    protected readonly ApplicationFeatures? _features;
    protected readonly SplashScreenOptions _splashScreenOptions;

    protected IServiceProvider? _services;
    protected LoadingView? _loadingView;
    protected bool _isShuttingDown = false;

    private Task? Initialize { get; set; }
    private int lastErrorMessage = 0;

    public static IServiceProvider Services
    {
        get
        {
            if (Application.Current is Application app)
                return app._services!;

            throw new InvalidOperationException("Services not available.");
        }
        internal set
        {
            if (Application.Current is Application app)
                app._services = value;
        }
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application(SplashScreenOptions? splashScreenOptions = null)
        : base()
    {
        _splashScreenOptions = splashScreenOptions ?? new SplashScreenOptions(SplashScreenTypes.None);
        var host = CreateHostBuilder() // NOSONAR
           .Build();

        Services = host.Services;
        _logger = Services.GetRequiredService<ILogger<Application>>();

        _logger.LogTrace("Setting up services and global exception handler.");

        _commonServices = Services.GetRequiredService<ICommonServices>();
        _dialogService = Services.GetRequiredService<IDialogService>();
        _navigationService = Services.GetRequiredService<INavigationService>();
        _exceptionHandlerService = Services.GetRequiredService<IExceptionHandlerService>();
        _settingsService = Services.GetRequiredService<ISettingsService>();
        _lifecycleService = Services.GetRequiredService<IApplicationLifecycleService>();
        _themeService = Services.GetService<IThemeService>();
        var messengerService = Services.GetRequiredService<IMessengerService>();

        _features = Services.GetRequiredService<IOptions<ApplicationFeatures>>().Value;

        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        DispatcherUnhandledException += BaseApplication_DispatcherUnhandledException;
        CommandExceptionDispatcher.UnhandledCommandException += CommandExceptionDispatcher_UnhandledCommandException;

        // Pass a timeout to limit the execution time.
        // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

        _commonServices.BusyService.StartBusy();

        if (_settingsService.LocalSettings is not null && !string.IsNullOrEmpty(_settingsService.LocalSettings.Language))
            _settingsService.LocalSettings.Language.SetLocalizationLanguage();

        _lifecycleService.ApplicationLoaded += OnApplicationLoaded;

        messengerService.Register<ShowInformationMessage>(this, async m =>
        {
            await _dialogService.ShowInformationAsync(m.Content.Message, m.Content.Title);
        });

        messengerService.Register<ShowWarningMessage>(this, async m =>
        {
            await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title);
        });

        messengerService.Register<ShowErrorMessage>(this, async m =>
        {
            await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title);
        });

        // Initialize environment variables from configuration and command-line parameters
        InitializeEnvironmentVariables(); // NOSONAR
    }

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

        foreach (var arg in commandLineArgs.EnsureNotNull()) // NOSONAR
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
    /// Handles the dispatcher unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void BaseApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleException(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.Handled = true;
    }

    /// <summary>
    /// Handles the first chance exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
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

    /// <summary>
    /// Handles an exception thrown by a synchronous relay command's execute delegate
    /// (e.g. a plain <c>RelayCommand</c> bound to a UI control). Routing it here instead of
    /// letting it propagate synchronously avoids an unhandled exception on the UI thread.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CommandExceptionDispatcher_UnhandledCommandException(object? sender, CommandExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleException(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _isShuttingDown = true;
        base.OnExit(e);
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
    public virtual IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
        new List<ResourceDictionary>();

    /// <summary>
    /// Creates a loading view page and stores a reference for completion tracking.
    /// </summary>
    private LoadingView CreateLoadingView()
    {
        _loadingView = new LoadingView(_commonServices, _splashScreenOptions);
        return _loadingView;
    }

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
            if (Current.Resources?.MergedDictionaries is not null)
            {
                foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
                {
                    if (!Current.Resources.MergedDictionaries.Contains(item))
                        Current.Resources.MergedDictionaries.Add(item);
                }
            }

            // Apply theme colors to merged resource dictionaries
            _themeService?.ApplyTheme();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        rootFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;

        MainWindow.Title = _commonServices.InfoService.ProductName;

        // Navigate to loading view — its Loaded event fires SignalApplicationUIReady()
        rootFrame.Navigate(CreateLoadingView());

        MainWindow.Show();
        MainWindow.Activate();

        // Apply RTL flow direction from saved settings so every WPF app
        // gets correct layout from the first frame.
        var language = _settingsService?.LocalSettings?.Language ?? GlobalizationConstants.DefaultCultureInfoName;
        if (MainWindow is FrameworkElement wpfRoot)
        {
            wpfRoot.ApplyFlowDirection(language);
        }

        InitializeApplication();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    /// <exception cref="Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
    private static void OnNavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e) =>
        throw new InvalidOperationException($"Failed to load {e.Uri}: {e.Exception}");

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
            var messengerService = Services.GetRequiredService<IMessengerService>();
            messengerService.Unregister<ShowInformationMessage>(this);
            messengerService.Unregister<ShowWarningMessage>(this);
            messengerService.Unregister<ShowErrorMessage>(this);

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            DispatcherUnhandledException -= BaseApplication_DispatcherUnhandledException;
            CommandExceptionDispatcher.UnhandledCommandException -= CommandExceptionDispatcher_UnhandledCommandException;

            _lifecycleService.ApplicationLoaded -= OnApplicationLoaded;
        }

        // free native resources if there are any.
    }
    #endregion
}
