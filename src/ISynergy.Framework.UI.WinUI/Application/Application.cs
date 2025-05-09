using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
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
using System.Runtime.InteropServices;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class Application : Microsoft.UI.Xaml.Application, IDisposable
{
    protected readonly ICommonServices _commonServices;
    protected readonly ISettingsService _settingsService;
    protected readonly IExceptionHandlerService _exceptionHandlerService;
    protected readonly ILogger<Application> _logger;

    protected readonly ApplicationFeatures _features;
    protected readonly SplashScreenOptions _splashScreenOptions;

    protected Microsoft.UI.Xaml.Window? _mainWindow;

    private int _lastErrorMessage = 0;

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
        var host = CreateHostBuilder()
            .Build()
            .SetLocatorProvider();

        _splashScreenOptions = splashScreenOptions ?? new SplashScreenOptions() { SplashScreenType = SplashScreenTypes.None };

        _logger = host.Services.GetRequiredService<ILogger<Application>>();
        _logger.LogTrace("Setting up global exception handler.");

        _logger.LogTrace("Getting common services.");
        _commonServices = host.Services.GetRequiredService<ICommonServices>();
        _features = host.Services.GetRequiredService<IOptions<ApplicationFeatures>>().Value;
        _exceptionHandlerService = host.Services.GetRequiredService<IExceptionHandlerService>();

        _logger.LogTrace("Retrieving scoped SettingsService");
        _settingsService = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>();

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
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <returns></returns>
    protected abstract IHostBuilder CreateHostBuilder();

    protected abstract void OnAuthenticationChanged(object? sender, ReturnEventArgs<bool> e);

    protected virtual void OnSoftwareEnvironmentChanged(object? sender, ReturnEventArgs<SoftwareEnvironments> e) => _commonServices.InfoService.SetTitle(e.Value);

    /// <summary>
    /// Handles the first chance exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        if (e?.Exception == null)
            return;

        // Ignore the exception if it is a TaskCanceledException and the cancellation token is requested
        if (e.Exception is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested)
            return;

        // Ignore the exception if it is an OperationCanceledException or a TaskCanceledException
        if (e.Exception is OperationCanceledException)
            return;

        // Ignore the exception if it is a TaskCanceledException inside an aggregated exception and the cancellation token is requested
        if (e.Exception is AggregateException ae && ae.InnerExceptions?.Any(ex => ex is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested) == true)
            return;

        if (e.Exception is IOException io && io.Message.Contains("The I/O operation has been aborted"))
            return;

        // Ignore the exception if it is a COMException and the message contains "Cannot find credential in Vault"
        if (e.Exception is COMException ce && ce.Message.Contains("Cannot find credential in Vault"))
            return;

        if (e.Exception.HResult != _lastErrorMessage)
        {
            _lastErrorMessage = e.Exception.HResult;
            _logger.LogError(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
        }
    }

    /// <summary>
    /// Handles the unobserved task exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual async void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            await _exceptionHandlerService.HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual async void CurrentDomain_UnhandledException(object? sender, System.UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                await _exceptionHandlerService.HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
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
        _mainWindow = WindowHelper.CreateWindow(_settingsService.LocalSettings.Theme);

        _logger.LogTrace("Loading custom resource dictionaries");

        if (Application.Current.Resources?.MergedDictionaries is not null)
        {
            foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
            {
                if (!Application.Current.Resources.MergedDictionaries.Contains(item))
                    Application.Current.Resources.MergedDictionaries.Add(item);
            }
        }

        _logger.LogTrace("Setting up theming.");

        if (_settingsService.LocalSettings is not null)
            this.SetApplicationColor(_settingsService.LocalSettings.Color);

        _logger.LogTrace("Settings initial view");

        var splashScreen = new SplashScreen();
        var viewModel = _commonServices.ScopedContextService.GetService<SplashScreenViewModel>();
        viewModel.Initialize(
            dispatcherQueue: _mainWindow.DispatcherQueue,
            onLoadingComplete: async () => await HandleApplicationInitializedAsync(),
            splashScreenOptions: _splashScreenOptions,
            async (dispatcher) =>
            {
                await HandleLaunchArgumentsAsync(args);
            },
            async (dispatcher) =>
            {
                if (_features.CheckForUpdatesInMicrosoftStore)
                    await dispatcher.EnqueueAsync(async () =>
                    {
                        try
                        {
                            _commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateCheckForUpdates");

                            var updateService = _commonServices.ScopedContextService.GetService<IUpdateService>();

                            if (await updateService.CheckForUpdateAsync() &&
                                await _commonServices.DialogService.ShowMessageAsync(
                                LanguageService.Default.GetString("UpdateFoundNewUpdate") + Environment.NewLine + LanguageService.Default.GetString("UpdateExecuteNow"),
                                "Update",
                                MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                            {
                                _commonServices.BusyService.BusyMessage = LanguageService.Default.GetString("UpdateDownloadAndInstall");
                                await updateService.DownloadAndInstallUpdateAsync();
                                Environment.Exit(Environment.ExitCode);
                            }
                        }
                        catch (Exception)
                        {
                            await _commonServices.DialogService.ShowErrorAsync("Failed to check for updates");
                        }
                    });
            },
            async (dispatcher) =>
            {
                _logger.LogTrace("Starting initialization of application");
                await InitializeApplicationAsync();
                _logger.LogTrace("Finishing initialization of application");
            });

        splashScreen.ViewModel = viewModel;

        splashScreen.Loaded += SplashScreen_Loaded;
        splashScreen.Unloaded += SplashScreen_Unloaded;

        _mainWindow.Content = splashScreen;

        _logger.LogTrace("Activate main window");
        _mainWindow.Activate();
    }


    protected virtual async void SplashScreen_Loaded(object? sender, RoutedEventArgs e)
    {
        if (sender is SplashScreen splashScreen && splashScreen.ViewModel is SplashScreenViewModel viewModel)
            await viewModel.StartInitializationAsync();
    }

    protected virtual void SplashScreen_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is SplashScreen splashScreen)
        {
            splashScreen.Loaded -= SplashScreen_Loaded;
            splashScreen.Unloaded -= SplashScreen_Unloaded;
        }
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    /// <exception cref="Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
    protected virtual void OnNavigationFailed(object? sender, NavigationFailedEventArgs e) =>
        throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

    protected virtual async Task HandleLaunchArgumentsAsync(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _logger.LogTrace("Handling launch event arguments");

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

        _logger.LogTrace("Handle command line arguments");

        if (Environment.GetCommandLineArgs().Length > 1)
            await HandleCommandLineArgumentsAsync(Environment.GetCommandLineArgs());
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
        if (disposing)
        {
            // free managed resources
            if (_commonServices.AuthenticationService is not null)
            {
                _commonServices.AuthenticationService.AuthenticationChanged -= OnAuthenticationChanged;
                _commonServices.AuthenticationService.SoftwareEnvironmentChanged -= OnSoftwareEnvironmentChanged;
            }

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
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
