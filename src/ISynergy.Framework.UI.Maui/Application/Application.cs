using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Controls;
using ISynergy.Framework.UI.Enumerations;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

#if WINDOWS
using Microsoft.UI.Xaml.Media;
using ISynergy.Framework.UI.Helpers;
#endif

#pragma warning disable IDE0130, S1200

[assembly: ExportFont("opendyslexic3-bold.ttf", Alias = "OpenDyslexic3-Bold")]
[assembly: ExportFont("opendyslexic3-regular.ttf", Alias = "OpenDyslexic3-Regular")]
[assembly: ExportFont("segoemdl2.ttf", Alias = "SegoeMdl2")]
[assembly: ExportFont("segoesb.ttf", Alias = "SegoeSemiBold")]
[assembly: ExportFont("segoeui.ttf", Alias = "SegoeUI")]
[assembly: ExportFont("segoeuib.ttf", Alias = "SegoeUIBold")]
[assembly: ExportFont("segoeuil.ttf", Alias = "SegoeUILight")]
[assembly: ExportFont("segoeuisl.ttf", Alias = "SegoeUISemiLight")]
[assembly: ExportFont("segoeuisb.ttf", Alias = "SegoeUISemiBold")]

namespace ISynergy.Framework.UI;

public abstract class Application : Microsoft.Maui.Controls.Application, IDisposable
{
    protected readonly ICommonServices? _commonServices;
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;
    protected readonly ISettingsService? _settingsService;
    protected readonly IExceptionHandlerService? _exceptionHandlerService;
    protected readonly IThemeService _themeService;
    protected readonly ILogger<Application>? _logger;

    protected readonly ApplicationFeatures? _features;
    protected readonly SplashScreenOptions _splashScreenOptions;
    protected LoadingView? _loadingView;

    private Microsoft.Maui.Controls.Window _window;
    private Task? Initialize { get; set; }
    private bool _isDisposing = false;
    private IApplicationLifecycleService? _lifecycleService;


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
                _logger = ServiceLocator.Default.GetRequiredService<ILogger<Application>>();
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

                _commonServices = ServiceLocator.Default.GetRequiredService<ICommonServices>();
                _dialogService = ServiceLocator.Default.GetRequiredService<IDialogService>();
                _navigationService = ServiceLocator.Default.GetRequiredService<INavigationService>();

                _features = ServiceLocator.Default.GetRequiredService<IOptions<ApplicationFeatures>>().Value;

                // This single line sets up ALL exception handling for the entire application
                _exceptionHandlerService = ServiceLocator.Default.GetRequiredService<IExceptionHandlerService>();

                _logger.LogTrace("Retrieving scoped SettingsService");
                _settingsService = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>();

                _logger.LogTrace("Starting application");

                // Pass a timeout to limit the execution time.
                // Not specifying a timeout for regular expressions is security - sensitivecsharpsquid:S6444
                AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(100));

                _logger.LogTrace("Setting up theming service.");
                _themeService = ServiceLocator.Default.GetRequiredService<IThemeService>();

                _logger.LogTrace("Setting up localization service.");

                if (_settingsService.LocalSettings is not null)
                    _settingsService.LocalSettings.Language.SetLocalizationLanguage();

                _logger.LogTrace("Starting initialization of application");
                _commonServices.BusyService.StartBusy();

                // Get or create the ApplicationLifecycleService for event-driven coordination
                _lifecycleService = _commonServices.ScopedContextService.GetRequiredService<IApplicationLifecycleService>();

                // Subscribe to lifecycle events
                _lifecycleService.ApplicationLoaded += OnApplicationLoaded;

                _logger.LogTrace("Setting up main page.");

                var mainPage = _splashScreenOptions.SplashScreenType switch
                {
                    SplashScreenTypes.Video => CreateLoadingViewPage(),
                    SplashScreenTypes.Image => CreateLoadingViewPage(),
                    _ => new NavigationPage(new EmptyView(_commonServices))
                };

                Application.Current.MainPage = mainPage;
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
    /// Called when the application is fully loaded (UI ready + initialization complete).
    /// Override or subscribe to this event to handle post-load operations.
    /// </summary>
    protected virtual void OnApplicationLoaded(object? sender, EventArgs e)
    {
        _logger?.LogTrace("Application lifecycle event: ApplicationLoaded raised");
    }

    /// <summary>
    /// Handles the authentication changed event.   
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected abstract void AuthenticationChanged(object? sender, ReturnEventArgs<bool> e);

    /// <summary>
    /// Creates a loading view page and stores a reference for completion tracking.
    /// </summary>
    private NavigationPage CreateLoadingViewPage()
    {
        _loadingView = new LoadingView(_commonServices, _splashScreenOptions);
        return new NavigationPage(_loadingView);
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
    /// Base implementation of InitializeApplicationAsync that handles exception service initialization.
    /// Derived classes should override InitializeApplicationAsync and call this method first if needed.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task InitializeExceptionHandlingAsync()
    {
        try
        {
            _logger?.LogTrace("Initializing exception handling");

            // Signal that application is ready for exception dialogs
            _exceptionHandlerService?.SetApplicationInitialized();

            _logger?.LogTrace("Exception handling initialization completed");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during exception handling initialization");
            throw;
        }
    }

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
        _window = base.CreateWindow(activationState);
        _window.Title = InfoService.Default.ProductName ?? string.Empty;

        _themeService.ApplyTheme();

        return _window;
    }

    /// <summary>
    /// Allows to add or update platform specific handlers.
    /// </summary>
    public virtual void UpdateMauiHandlers(Style style)
    {
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
            _isDisposing = true;

            // Unregister lifecycle event handlers
            if (_lifecycleService is not null)
            {
                _lifecycleService.ApplicationLoaded -= OnApplicationLoaded;
                _lifecycleService.Dispose();
            }

            // Let ExceptionHandlerService clean up its own handlers
            if (_exceptionHandlerService is IDisposable disposableHandler)
            {
                disposableHandler.Dispose();
            }
        }
    }
    #endregion
}
