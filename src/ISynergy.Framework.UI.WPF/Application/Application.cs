using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Messages;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
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

    protected readonly ApplicationFeatures? _features;

    protected bool _isShuttingDown = false;

    public event EventHandler<ReturnEventArgs<bool>>? ApplicationInitialized;
    public event EventHandler<ReturnEventArgs<bool>>? ApplicationLoaded;

    public virtual void RaiseApplicationInitialized() => ApplicationInitialized?.Invoke(this, new ReturnEventArgs<bool>(true));
    public virtual void RaiseApplicationLoaded() => ApplicationLoaded?.Invoke(this, new ReturnEventArgs<bool>(true));

    private Task? Initialize { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application()
        : base()
    {
        var host = CreateHostBuilder()
           .Build()
           .SetLocatorProvider();

        _logger = host.Services.GetRequiredService<ILogger<Application>>();

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
        DispatcherUnhandledException += BaseApplication_DispatcherUnhandledException;

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


    protected abstract void OnApplicationLoaded(object? sender, ReturnEventArgs<bool> e);

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
        // Ignore task cancellation exceptions during shutdown
        if (e.Exception is OperationCanceledException || e.Exception is TaskCanceledException)
        {
            // Check if we're in the process of shutting down
            if (_isShuttingDown)
                return; // Ignore the exception
        }
        else
        {
            Debug.WriteLine(e.Exception.ToMessage(Environment.StackTrace));
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
    /// Initializes the application asynchronously.
    /// </summary>
    private void InitializeApplication() => Initialize = InitializeApplicationAsync();

    protected override void OnExit(ExitEventArgs e)
    {
        _isShuttingDown = true;
        base.OnExit(e);
    }

    /// <summary>
    /// LoadAssembly the application.
    /// </summary>
    /// <example>
    /// <code>
    ///     await base.InitializeApplicationAsync();
    ///     // wait 5 seconds before showing the main window...
    ///     await Task.Delay(5000);
    ///     await _commonServices.ScopedContextService.GetService{INavigationService}().ReplaceMainWindowAsync{IShellView}();
    /// </code>
    /// </example>
    /// <returns></returns>
    public abstract Task InitializeApplicationAsync();

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
            if (Current.Resources?.MergedDictionaries is not null)
            {
                foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
                {
                    if (!Current.Resources.MergedDictionaries.Contains(item))
                        Current.Resources.MergedDictionaries.Add(item);
                }
            }

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        rootFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;

        MainWindow.Show();
        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    /// <exception cref="Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
    private void OnNavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e) =>
        throw new Exception($"Failed to load {e.Uri}: {e.Exception}");

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
            MessengerService.Default.Unregister<ShowInformationMessage>(this);
            MessengerService.Default.Unregister<ShowWarningMessage>(this);
            MessengerService.Default.Unregister<ShowErrorMessage>(this);

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            DispatcherUnhandledException -= BaseApplication_DispatcherUnhandledException;

            ApplicationLoaded -= OnApplicationLoaded;
        }

        // free native resources if there are any.
    }
    #endregion
}
