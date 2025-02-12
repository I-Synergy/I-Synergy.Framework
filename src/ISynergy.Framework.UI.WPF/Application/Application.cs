using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class Application : System.Windows.Application, IDisposable
{
    protected readonly ILogger _logger;
    protected readonly ICommonServices _commonServices;

    public event EventHandler<ReturnEventArgs<bool>> ApplicationInitialized;
    public event EventHandler<ReturnEventArgs<bool>> ApplicationLoaded;

    public virtual void RaiseApplicationInitialized() => ApplicationInitialized?.Invoke(this, new ReturnEventArgs<bool>(true));
    public virtual void RaiseApplicationLoaded() => ApplicationLoaded?.Invoke(this, new ReturnEventArgs<bool>(true));

    private Task Initialize { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Application(ICommonServices commonServices)
        : base()
    {
        _commonServices = commonServices;

        _logger = _commonServices.ScopedContextService.GetService<ILoggerFactory>().CreateLogger<Application>();
        _logger.LogTrace("Starting application");

        this.ApplicationLoaded += OnApplicationLoaded;

        // Pass a timeout to limit the execution time.
        // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

        _logger.LogTrace("Setting up global exception handler.");
        SetGlobalExceptionHandler();

        _logger.LogTrace("Starting initialization of application");

        _logger.LogTrace("Setting up main page.");

        _logger.LogTrace("Getting common services.");
        _commonServices.BusyService.StartBusy();

        _logger.LogInformation("Setting up authentication service.");
        _commonServices.AuthenticationService.AuthenticationChanged += OnAuthenticationChanged;
        _commonServices.AuthenticationService.SoftwareEnvironmentChanged += OnSoftwareEnvironmentChanged;

        _logger.LogTrace("Setting up localization service.");

        if (_commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings is not null)
            _commonServices.ScopedContextService.GetService<ISettingsService>().LocalSettings.Language.SetLocalizationLanguage();

        //_logger.LogTrace("Setting up theming.");

        //if (_settingsService.LocalSettings is not null)
        //{
        //    Application.Primary = Color.FromArgb(_settingsService.LocalSettings.Color);

        //    if (_settingsService.LocalSettings.IsLightThemeEnabled)
        //        Application.Current.Resources.ApplyLightTheme();
        //    else
        //        Application.Current.Resources.ApplyDarkTheme();
        //}

        _logger.LogTrace("Starting initialization of application");

        InitializeApplication();

        _logger.LogTrace("Finishing initialization of application");
    }

    protected abstract void OnAuthenticationChanged(object sender, ReturnEventArgs<bool> e);
    protected abstract void OnApplicationLoaded(object sender, ReturnEventArgs<bool> e);

    protected virtual void OnSoftwareEnvironmentChanged(object sender, ReturnEventArgs<SoftwareEnvironments> e)
    {
        _commonServices.InfoService.SetTitle(e.Value);
    }

    /// <summary>
    /// Sets the global exception handler.
    /// </summary>
    protected virtual void SetGlobalExceptionHandler()
    {
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        DispatcherUnhandledException += BaseApplication_DispatcherUnhandledException;
    }

    /// <summary>
    /// Handles the dispatcher unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void BaseApplication_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        if (_commonServices.ScopedContextService.GetService<IExceptionHandlerService>() is not null)
            await _commonServices.ScopedContextService.GetService<IExceptionHandlerService>().HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.Handled = true;
    }

    /// <summary>
    /// Handles the first chance exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        Debug.WriteLine(e.Exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Handles the unobserved task exception.
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
    /// Handles the unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_commonServices.ScopedContextService.GetService<IExceptionHandlerService>() is not null)
                await _commonServices.ScopedContextService.GetService<IExceptionHandlerService>().HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Initializes the application asynchronously.
    /// </summary>
    private void InitializeApplication() => Initialize = InitializeApplicationAsync();

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
            if (Application.Current.Resources?.MergedDictionaries is not null)
            {
                foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
                {
                    if (!Application.Current.Resources.MergedDictionaries.Contains(item))
                        Application.Current.Resources.MergedDictionaries.Add(item);
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

    public virtual Task HandleCommandLineArgumentsAsync(string[] e) =>
        Task.CompletedTask;

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
            // free managed resources
            if (_commonServices.AuthenticationService is not null)
            {
                _commonServices.AuthenticationService.AuthenticationChanged -= OnAuthenticationChanged;
                _commonServices.AuthenticationService.SoftwareEnvironmentChanged -= OnSoftwareEnvironmentChanged;
            }

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            DispatcherUnhandledException -= BaseApplication_DispatcherUnhandledException;

            this.ApplicationLoaded -= OnApplicationLoaded;
        }

        // free native resources if there are any.
    }
    #endregion
}
