using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Abstractions.Services.Base;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services.Base;
using ISynergy.Framework.UI.Abstractions;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Services;
using ISynergy.Framework.UI.ViewModels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System.Globalization;
using Windows.ApplicationModel.Activation;
using Application = Microsoft.UI.Xaml.Application;
using IThemeService = ISynergy.Framework.Mvvm.Abstractions.Services.IThemeService;

namespace ISynergy.Framework.UI;

/// <summary>
/// Class BaseApplication.
/// </summary>
public abstract class BaseApplication : Application, IBaseApplication, IDisposable
{
    protected readonly IExceptionHandlerService _exceptionHandlerService;
    protected readonly ILogger _logger;
    protected readonly IContext _context;
    protected readonly IThemeService _themeService;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IApplicationSettingsService _applicationSettingsService;
    protected readonly INavigationService _navigationService;
    protected readonly IBaseCommonServices _commonServices;

    private Func<ILoadingView> _initialView;
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
        _initialView = initialView;

        var host = CreateHostBuilder()
            .ConfigureLogging(config =>
            {
#if DEBUG
                config.AddDebug();
#endif
                config.SetMinimumLevel(LogLevel.Trace);
            })
            .Build();

        ServiceLocator.SetLocatorProvider(host.Services);

        _logger = ServiceLocator.Default.GetInstance<ILogger>();
        _logger.LogInformation("Setting up global exception handler.");

        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        _logger.LogInformation("Starting application");

        // Pass a timeout to limit the execution time.
        // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

        _logger.LogInformation("Starting initialization of application");

        _logger.LogInformation("Setting up main page.");

        _logger.LogInformation("Getting common services.");
        _commonServices = ServiceLocator.Default.GetInstance<IBaseCommonServices>();
        _commonServices.BusyService.StartBusy();

        _logger.LogInformation("Setting up context.");
        _context = ServiceLocator.Default.GetInstance<IContext>();

        _logger.LogInformation("Setting up authentication service.");
        _authenticationService = ServiceLocator.Default.GetInstance<IAuthenticationService>();
        _authenticationService.AuthenticationChanged += AuthenticationChanged;

        _logger.LogInformation("Setting up theming service.");
        _themeService = ServiceLocator.Default.GetInstance<IThemeService>();

        if (_themeService.IsLightThemeEnabled)
            RequestedTheme = ApplicationTheme.Light;
        else
            RequestedTheme = ApplicationTheme.Dark;

        _logger.LogInformation("Setting up navigation service.");
        _navigationService = ServiceLocator.Default.GetInstance<INavigationService>();

        _logger.LogInformation("Setting up exception handler service.");
        _exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

        _logger.LogInformation("Setting up application settings service.");
        _applicationSettingsService = ServiceLocator.Default.GetInstance<IApplicationSettingsService>();

        _logger.LogInformation("Setting up localization service.");
        _localizationService = ServiceLocator.Default.GetInstance<ILocalizationService>();

        if (_applicationSettingsService.Settings is not null)
            _localizationService.SetLocalizationLanguage(_applicationSettingsService.Settings.Language);
        
        _logger.LogInformation("Starting initialization of application");
        InitializeApplication();

        _logger.LogInformation("Finishing initialization of application");
    }

    /// <summary>
    /// Creates the host builder.
    /// </summary>
    /// <returns></returns>
    protected abstract IHostBuilder CreateHostBuilder();

    /// <summary>
    /// Handles the authentication changed event.   
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public abstract void AuthenticationChanged(object sender, ReturnEventArgs<bool> e);

    /// <summary>
    /// Handles the first chance exception event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
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
    public virtual async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
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
    public virtual async void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                await _exceptionHandlerService.HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Initializes the application.
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
    ///     await ServiceLocator.Default.GetInstance{INavigationService}().ReplaceMainWindowAsync{IShellView}();
    /// </code>
    /// </example>
    /// <returns></returns>
    public virtual Task InitializeApplicationAsync()
    {
        var culture = CultureInfo.CurrentCulture;
        var numberFormat = (NumberFormatInfo)culture.NumberFormat.Clone();
        numberFormat.CurrencySymbol = $"{_context.CurrencySymbol} ";
        numberFormat.CurrencyNegativePattern = 1;
        _context.NumberFormat = numberFormat;

        return Task.CompletedTask;
    }

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
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
    {
        MainWindow = WindowHelper.CreateWindow();

        MessageService.Default.Register<StyleChangedMessage>(this, m => StyleChanged(m));

        _logger.LogInformation("Loading custom resource dictionaries");

        if (Application.Current.Resources?.MergedDictionaries is not null)
        {
            foreach (var item in GetAdditionalResourceDictionaries().EnsureNotNull())
            {
                if (!Application.Current.Resources.MergedDictionaries.Contains(item))
                    Application.Current.Resources.MergedDictionaries.Add(item);
            }
        }

        _logger.LogInformation("Loading theme");

        if (_themeService is ThemeService themeService)
        {
            themeService.InitializeMainWindow(MainWindow);
            themeService.SetStyle();
        }

        if (_initialView is not null)
        {
            var loadingView = _initialView.Invoke() as View;
            loadingView.ViewModel = ServiceLocator.Default.GetInstance<LoadingViewModel>();
            MainWindow.Content = loadingView;
        }
        else
            MainWindow.Content = new BusyIndicatorControl(_commonServices);
        
        if (e.UWPLaunchActivatedEventArgs is not null)
        {
            switch (e.UWPLaunchActivatedEventArgs.Kind)
            {
                case ActivationKind.Launch:
                    await HandleLaunchActivationAsync(e.UWPLaunchActivatedEventArgs.Arguments);
                    break;
                case ActivationKind.Protocol:
                    await HandleProtocolActivationAsync(e.UWPLaunchActivatedEventArgs.Arguments);
                    break;
            }
        }

        if (Environment.GetCommandLineArgs().Length > 1)
            await HandleCommandLineArgumentsAsync(Environment.GetCommandLineArgs());

        if (MainWindow is not null)
        {
            MessageService.Default.Register<EnvironmentChangedMessage>(this, m => MainWindow.Title = InfoService.Default.Title ?? string.Empty);
            MainWindow.Title = InfoService.Default.Title ?? string.Empty;
            MainWindow.Activate();
        }
    }

    /// <summary>
    /// Handles the style changed event.
    /// </summary>
    /// <param name="m"></param>
    public virtual void StyleChanged(StyleChangedMessage m)
    {
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

    // NOTE: Leave out the finalizer altogether if this class doesn't
    // own unmanaged resources, but leave the other methods
    // exactly as they are.
    //~ObservableClass()
    //{
    //    // Finalizer calls Dispose(false)
    //    Dispose(false);
    //}

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
            MessageService.Default.Unregister<EnvironmentChangedMessage>(this);
            MessageService.Default.Unregister<StyleChangedMessage>(this);

            if (_authenticationService is not null)
                _authenticationService.AuthenticationChanged += AuthenticationChanged;

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }

        // free native resources if there are any.
    }
#endif
    #endregion
}
