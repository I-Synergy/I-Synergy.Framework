﻿using ISynergy.Framework.Core.Abstractions;
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
using ISynergy.Framework.UI.Exceptions;
using Microsoft.Extensions.Logging;
using System.Globalization;

[assembly: ExportFont("opendyslexic3-bold.ttf", Alias = "OpenDyslexic3-Bold")]
[assembly: ExportFont("opendyslexic3-regular.ttf", Alias = "OpenDyslexic3-Regular")]
[assembly: ExportFont("segoemdl2.ttf", Alias = "SegoeMdl2")]

namespace ISynergy.Framework.UI;

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

    private Microsoft.Maui.Controls.Window _window;

    private Task Initialize { get; set; }

    public AppTheme Theme { get; set; } = AppTheme.Dark;

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected BaseApplication(Func<Page> initialView = null)
        : base()
    {
        _logger = ServiceLocator.Default.GetInstance<ILogger>() ?? LoggerFactory.Create(config =>
        {
#if DEBUG
            config.AddDebug();
#endif
            config.SetMinimumLevel(LogLevel.Trace);
        }).CreateLogger(AppDomain.CurrentDomain.FriendlyName);

        _logger.LogInformation("Setting up global exception handler.");

        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        MauiExceptions.UnhandledException += CurrentDomain_UnhandledException;
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

        if (initialView is not null)
            Application.Current.MainPage = new NavigationPage(initialView.Invoke());
        else
            Application.Current.MainPage = new NavigationPage(new EmptyView(_commonServices));

        _logger.LogInformation("Setting up context.");
        _context = ServiceLocator.Default.GetInstance<IContext>();

        _logger.LogInformation("Setting up authentication service.");
        _authenticationService = ServiceLocator.Default.GetInstance<IAuthenticationService>();
        _authenticationService.AuthenticationChanged += AuthenticationChanged;

        _logger.LogInformation("Setting up theming service.");
        _themeService = ServiceLocator.Default.GetInstance<IThemeService>();

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

        MessageService.Default.Register<EnvironmentChangedMessage>(this, m =>
        {
            _window.Title = InfoService.Default.Title ?? string.Empty;
        });

        _logger.LogInformation("Finishing initialization of application");
    }

    /// <summary>
    /// Handles the authentication changed event.   
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public abstract void AuthenticationChanged(object sender, ReturnEventArgs<bool> e);

    private int lastErrorMessage = 0;

    /// <summary>
    /// Handles the first chance exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
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
    protected virtual void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        if (_exceptionHandlerService is not null)
            _exceptionHandlerService.HandleExceptionAsync(e.Exception);
        else
            _logger.LogCritical(e.Exception, e.Exception.ToMessage(Environment.StackTrace));

        e.SetObserved();
    }

    /// <summary>
    /// Handles the unhandled exception.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
            if (_exceptionHandlerService is not null)
                _exceptionHandlerService.HandleExceptionAsync(exception);
            else
                _logger.LogCritical(exception, exception.ToMessage(Environment.StackTrace));
    }

    /// <summary>
    /// Initializes the application asynchronously.
    /// </summary>
    public void InitializeApplication() => Initialize = InitializeApplicationAsync();

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

    protected override void OnStart() =>
        InitializeApplication();

    protected override Microsoft.Maui.Controls.Window CreateWindow(IActivationState activationState)
    {
        _window = base.CreateWindow(activationState);

        _logger.LogInformation("Setting style.");
        MessageService.Default.Register<StyleChangedMessage>(this, m => StyleChanged(m));
        _themeService.SetStyle();

        _window.Title = InfoService.Default.Title ?? string.Empty;
        return _window;
    }

    /// <summary>
    /// Handles the style changed event.
    /// </summary>
    /// <param name="m"></param>
    public virtual void StyleChanged(StyleChangedMessage m) =>
        UpdateMauiHandlers(m.Content);

    /// <summary>
    /// Allows to add or update platform specific handlers.
    /// </summary>
    public virtual void UpdateMauiHandlers(Style style)
    {
    }

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

    // NOTE: Leave out the finalizer altogether if this class doesn't
    // own unmanaged resources, but leave the other methods
    // exactly as they are.
    //~ObservableClass()
    //{
    //    // Finalizer calls Dispose(false)
    //    Dispose(false);
    //}

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
            MessageService.Default.Unregister<StyleChangedMessage>(this);

            if (_authenticationService is not null)
                _authenticationService.AuthenticationChanged -= AuthenticationChanged;

            AppDomain.CurrentDomain.FirstChanceException -= CurrentDomain_FirstChanceException;
            MauiExceptions.UnhandledException -= CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }

        // free native resources if there are any.
    }
    #endregion
}
