using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Activation;
using Application = Microsoft.UI.Xaml.Application;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class BaseApplication : Application, IBaseApplication, IDisposable
{
    protected readonly ILogger _logger;
    protected readonly ICommonServices _commonServices;
    protected readonly Func<ILoadingView> _initialView;

    public event EventHandler<ReturnEventArgs<bool>> ApplicationInitialized;
    public event EventHandler<ReturnEventArgs<bool>> ApplicationLoaded;

    public virtual void RaiseApplicationInitialized()
    {
        ApplicationInitialized?.Invoke(this, new ReturnEventArgs<bool>(true));
    }

    public virtual void RaiseApplicationLoaded()
    {
        ApplicationLoaded?.Invoke(this, new ReturnEventArgs<bool>(true));
    }

    private int lastErrorMessage = 0;

    private Task Initialize { get; set; }

    /// <summary>
    /// Main Application Window.
    /// </summary>
    /// <value>The main window.</value>
    public Microsoft.UI.Xaml.Window MainWindow { get; private set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected BaseApplication(Func<ILoadingView> initialView = null)
        : base()
    {
        var host = CreateHostBuilder()
            .ConfigureLogging(config =>
            {
#if DEBUG
                config.AddDebug();
#endif
            })
            .Build();

        _initialView = initialView;

        _logger = host.Services.GetService<ILoggerFactory>().CreateLogger<BaseApplication>();
        _logger.LogTrace("Setting up global exception handler.");

        this.ApplicationLoaded += OnApplicationLoaded;

        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        _logger.LogTrace("Starting application");

        // Pass a timeout to limit the execution time.
        // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

        _logger.LogTrace("Starting initialization of application");

        _logger.LogTrace("Getting common services.");
        _commonServices = host.Services.GetService<ICommonServices>();

        _logger.LogTrace("Setting up main page.");
        _commonServices.BusyService.StartBusy();

        _logger.LogTrace("Setting up authentication service.");
        _commonServices.AuthenticationService.AuthenticationChanged += OnAuthenticationChanged;
        _commonServices.AuthenticationService.SoftwareEnvironmentChanged += OnSoftwareEnvironmentChanged;

        _logger.LogTrace("Setting up localization service.");
        if (_commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings is not null)
            _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.Language.SetLocalizationLanguage();

        _logger.LogTrace("Setting up theming.");
        if (_commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings is not null &&
            _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.IsLightThemeEnabled)
            RequestedTheme = ApplicationTheme.Light;
        else
            RequestedTheme = ApplicationTheme.Dark;

        _logger.LogTrace("Starting initialization of application");

        InitializeApplication();

        _logger.LogTrace("Finishing initialization of application");
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <returns></returns>
    protected abstract IHostBuilder CreateHostBuilder();

    protected abstract void OnAuthenticationChanged(object sender, ReturnEventArgs<bool> e);
    protected abstract void OnApplicationLoaded(object sender, ReturnEventArgs<bool> e);

    protected virtual void OnSoftwareEnvironmentChanged(object sender, ReturnEventArgs<SoftwareEnvironments> e)
    {
        _commonServices.InfoService.SetTitle(e.Value);
    }

    /// <summary>
    /// Handles the first chance exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        if (e?.Exception == null)
            return;

        // Ignore the exception if it is a TaskCanceledException and the cancellation token is requested
        if (e.Exception is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested)
            return;

        // Ignore the exception if it is a TaskCanceledException inside an aggregated exception and the cancellation token is requested
        if (e.Exception is AggregateException ae && ae.InnerExceptions?.Any(ex => ex is TaskCanceledException tce && tce.CancellationToken.IsCancellationRequested) == true)
            return;

        // Ignore the exception if it is a COMException and the message contains "Cannot find credential in Vault"
        if (e.Exception is COMException ce && ce.Message.Contains("Cannot find credential in Vault"))
            return;

        if (e.Exception.HResult != lastErrorMessage)
        {
            lastErrorMessage = e.Exception.HResult;
            _logger?.LogError(e.Exception, e.Exception.ToMessage(Environment.StackTrace));
        }
    }

    /// <summary>
    /// Handles the unobserved task exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_commonServices.ScopedContextService.GetService<IExceptionHandlerService>() is not null)
            await _commonServices.ScopedContextService.GetService<IExceptionHandlerService>().HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_commonServices.ScopedContextService.GetService<IExceptionHandlerService>() is not null)
                await _commonServices.ScopedContextService.GetService<IExceptionHandlerService>().HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Initializes the application.
    /// </summary>
    private void InitializeApplication() => Initialize = InitializeApplicationAsync();

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
    /// <param name="args">Event data for the event.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        MainWindow = WindowHelper.CreateWindow(_commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.Theme, _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.Color);

        _logger.LogTrace("Loading custom resource dictionaries");

        if (Application.Current.Resources?.MergedDictionaries is not null)
        {
            foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
            {
                if (!Application.Current.Resources.MergedDictionaries.Contains(item))
                    Application.Current.Resources.MergedDictionaries.Add(item);
            }
        }

        if (_initialView is not null && _initialView.Invoke() is View loadingView)
        {
            loadingView.ViewModel = _commonServices.ScopedContextService.GetService<LoadingViewModel>();
            MainWindow.Content = loadingView;
        }
        else
            MainWindow.Content = new BusyIndicatorControl(_commonServices);

        if (args.UWPLaunchActivatedEventArgs is not null)
        {
            switch (args.UWPLaunchActivatedEventArgs.Kind)
            {
                case ActivationKind.Launch:
                    await HandleLaunchActivationAsync(args.UWPLaunchActivatedEventArgs.Arguments);
                    break;
                case ActivationKind.Protocol:
                    await HandleProtocolActivationAsync(args.UWPLaunchActivatedEventArgs.Arguments);
                    break;
            }
        }

        if (Environment.GetCommandLineArgs().Length > 1)
            await HandleCommandLineArgumentsAsync(Environment.GetCommandLineArgs());

        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    /// <exception cref="Exception">Failed to load {e.SourcePageType.FullName}: {e.Exception}</exception>
    public virtual void OnNavigationFailed(object sender, NavigationFailedEventArgs e) =>
        throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

    public virtual Task HandleProtocolActivationAsync(string e) =>
        Task.CompletedTask;

    public virtual Task HandleLaunchActivationAsync(string e) =>
        Task.CompletedTask;

    public virtual Task HandleCommandLineArgumentsAsync(string[] e) =>
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

            this.ApplicationLoaded -= OnApplicationLoaded;
        }

        // free native resources if there are any.
    }
#endif

    ~BaseApplication()
    {
        Dispose(false);
    }
    #endregion
}
