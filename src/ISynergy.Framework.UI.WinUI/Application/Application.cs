using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Enumerations;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Options;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class Application : Microsoft.UI.Xaml.Application, IDisposable
{
    protected readonly ICommonServices? _commonServices;
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;
    protected readonly ISettingsService? _settingsService;
    protected readonly IExceptionHandlerService? _exceptionHandlerService;
    protected readonly ILogger<Application>? _logger;

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
            _splashScreenOptions = splashScreenOptions ?? new SplashScreenOptions() { SplashScreenType = SplashScreenTypes.None };

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
                _logger.LogTrace("Setting up global exception handler.");

                // Get required services
                _commonServices = host.Services.GetRequiredService<ICommonServices>();
                _dialogService = host.Services.GetRequiredService<IDialogService>();
                _navigationService = host.Services.GetRequiredService<INavigationService>();

                _features = host.Services.GetRequiredService<IOptions<ApplicationFeatures>>().Value;
                _exceptionHandlerService = host.Services.GetRequiredService<IExceptionHandlerService>();

                _logger.LogTrace("Retrieving scoped SettingsService");
                _settingsService = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>();

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

                _logger.LogTrace("Setting up localization service.");

                if (_settingsService.LocalSettings is not null)
                    _settingsService.LocalSettings.Language.SetLocalizationLanguage();
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

    protected abstract void OnAuthenticationChanged(object? sender, ReturnEventArgs<bool> e);

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

            _logger?.LogTrace("Settings initial view");

            var splashScreen = new SplashScreen();
            var viewModel = _commonServices?.ScopedContextService.GetService<SplashScreenViewModel>();

            if (_mainWindow is not null && viewModel is not null)
            {
                viewModel.Initialize(
                    dispatcherQueue: _mainWindow.DispatcherQueue,
                    onLoadingComplete: async () =>
                    {
                        try
                        {
                            await HandleApplicationInitializedAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error in HandleApplicationInitializedAsync");
                        }
                    },
                    splashScreenOptions: _splashScreenOptions,
                    async (dispatcher) =>
                    {
                        try
                        {
                            await HandleLaunchArgumentsAsync(args);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error handling launch arguments");
                        }
                    },
                    async (dispatcher) =>
                    {
                        if (_commonServices is not null && _features?.CheckForUpdatesInMicrosoftStore == true)
                        {
                            await dispatcher.EnqueueAsync(async () =>
                            {
                                try
                                {
                                    _commonServices.BusyService.UpdateMessage(LanguageService.Default.GetString("UpdateCheckForUpdates"));

                                    var updateService = _commonServices.ScopedContextService.GetService<IUpdateService>();

                                    if (updateService != null &&
                                        await updateService.CheckForUpdateAsync() &&
                                        _dialogService != null &&
                                        await _dialogService.ShowMessageAsync(
                                            LanguageService.Default.GetString("UpdateFoundNewUpdate") + Environment.NewLine + LanguageService.Default.GetString("UpdateExecuteNow"),
                                            "Update",
                                            MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                                    {
                                        _commonServices.BusyService.UpdateMessage(LanguageService.Default.GetString("UpdateDownloadAndInstall"));
                                        await updateService.DownloadAndInstallUpdateAsync();
                                        Environment.Exit(Environment.ExitCode);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger?.LogError(ex, "Failed to check for updates");
                                    if (_dialogService != null)
                                    {
                                        try
                                        {
                                            await _dialogService.ShowErrorAsync("Failed to check for updates");
                                        }
                                        catch (Exception dialogEx)
                                        {
                                            _logger?.LogError(dialogEx, "Error showing dialog");
                                        }
                                    }
                                }
                            });
                        }
                    },
                    async (dispatcher) =>
                    {
                        try
                        {
                            _logger?.LogTrace("Starting initialization of application");
                            await InitializeApplicationAsync();
                            _logger?.LogTrace("Finishing initialization of application");
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error in InitializeApplicationAsync");
                        }
                    });

                splashScreen.ViewModel = viewModel;

                splashScreen.Loaded += SplashScreen_Loaded;
                splashScreen.Unloaded += SplashScreen_Unloaded;

                _mainWindow.Content = splashScreen;

                _logger?.LogTrace("Activate main window");
                _mainWindow.Activate();
            }
            else
            {
                _logger?.LogWarning("Failed to create a main window");
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

    protected virtual async void SplashScreen_Loaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is SplashScreen splashScreen && splashScreen.ViewModel is SplashScreenViewModel viewModel)
                await viewModel.StartInitializationAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in SplashScreen_Loaded");
        }
    }

    protected virtual void SplashScreen_Unloaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is SplashScreen splashScreen)
            {
                splashScreen.Loaded -= SplashScreen_Loaded;
                splashScreen.Unloaded -= SplashScreen_Unloaded;
            }

            if (_exceptionHandlerService is not null)
                _exceptionHandlerService.SetApplicationInitialized();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in SplashScreen_Unloaded");
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

