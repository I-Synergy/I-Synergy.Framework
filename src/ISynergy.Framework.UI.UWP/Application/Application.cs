using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#pragma warning disable IDE0130, S1200

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class Application : Windows.UI.Xaml.Application, IDisposable
{
    protected readonly ICommonServices? _commonServices;
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;
    protected readonly ISettingsService? _settingsService;
    protected readonly IExceptionHandlerService? _exceptionHandlerService;
    protected readonly ILogger<Application>? _logger;

    protected IThemeService _themeSelector;

    protected readonly ApplicationFeatures? _features;
    protected readonly SplashScreenOptions _splashScreenOptions;

    protected Windows.UI.Xaml.Window? _mainWindow;

    private int _lastErrorMessage = 0;
    private bool _isDisposing = false;
    private readonly Dictionary<string, Exception> _processedExceptions = new Dictionary<string, Exception>();

    /// <summary>
    /// Gets the current main window from the running application instance
    /// </summary>
    public static Windows.UI.Xaml.Window? MainWindow
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
    protected override void OnLaunched(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
    {
        _mainWindow = Windows.UI.Xaml.Window.Current;

        var rootFrame = MainWindow.Content as Frame;

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (rootFrame is null)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();
            rootFrame.NavigationFailed += OnNavigationFailed;

            rootFrame.Navigated += OnNavigated;

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Register a handler for BackRequested events and set the
            // visibility of the Back button
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                rootFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

            // Add custom resourcedictionaries from code.
            var dictionary = Application.Current.Resources?.MergedDictionaries;

            if (dictionary is not null)
            {
                foreach (var item in GetAdditionalResourceDictionaries())
                {
                    if (!dictionary.Any(t => t.Source == item.Source))
                        Application.Current.Resources.MergedDictionaries.Add(item);
                }
            }

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        if (!args.PrelaunchActivated)
        {
            if (rootFrame.Content is null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                var view = ServiceLocator.Default.GetRequiredService<IShellView>();
                rootFrame.Navigate(view.GetType(), args.Arguments);
            }
        }

        _themeSelector = ServiceLocator.Default.GetRequiredService<IThemeService>();

        MainWindow.Activate();
    }

    /// <summary>
    /// Handles the <see cref="E:Navigated" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
    private static void OnNavigated(object sender, NavigationEventArgs e)
    {

        // Each time a navigation event occurs, update the Back button's visibility
        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            ((Frame)sender).CanGoBack ?
            AppViewBackButtonVisibility.Visible :
            AppViewBackButtonVisibility.Collapsed;
    }

    /// <summary>
    /// Handles the <see cref="E:BackRequested" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="BackRequestedEventArgs" /> instance containing the event data.</param>
    private static void OnBackRequested(object sender, BackRequestedEventArgs e)
    {
        var rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

        if (Windows.UI.Xaml.Window.Current.Content.FindDescendantByName("ContentRootFrame") is Frame _frame)
        {
            rootFrame = _frame;
        }

        if (rootFrame.CanGoBack)
        {
            e.Handled = true;
            rootFrame.GoBack();
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

    protected virtual async Task HandleLaunchArgumentsAsync(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
    {
        try
        {
            _logger?.LogTrace("Handling launch event arguments");

            if (args is not null)
            {
                switch (args.Kind)
                {
                    case Windows.ApplicationModel.Activation.ActivationKind.Launch:
                        await HandleLaunchActivationAsync(args.Arguments);
                        break;
                    case Windows.ApplicationModel.Activation.ActivationKind.Protocol:
                        await HandleProtocolActivationAsync(args.Arguments);
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

