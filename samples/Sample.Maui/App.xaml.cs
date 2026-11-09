using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.Extensions.Logging;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System.Globalization;
using System.Runtime.ExceptionServices;
using Application = ISynergy.Framework.UI.Application;

namespace Sample;

public partial class App : Application
{
    public App()
        : base()
    //: base(new SplashScreenOptions(SplashScreenTypes.Video, "gta.mp4"))
    {
        try
        {
            InitializeComponent();

            // Subscribe to authentication messages
            _commonServices?.MessengerService.Register<UserSignedInMessage>(this, OnUserSignedIn, keepTargetAlive: true);
            _commonServices?.MessengerService.Register<UserSignedOutMessage>(this, OnUserSignedOut, keepTargetAlive: true);
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Error in App constructor");
        }
    }

    /// <summary>
    /// Icon shown in the title bar.
    /// </summary>
    protected override ImageSource? Icon => ImageSource.FromFile("icon.png");

    /// <summary>
    /// Opts out of a custom title bar.
    /// </summary>
    /// <remarks>
    /// The platform default title bar is used, as on the Apple heads: it shows the window title and
    /// follows the operating system appearance. The shell header (menu, back, breadcrumb, theme and
    /// settings) lives in the page, so the blade pane overlays it and blocks those commands while a
    /// blade is open. Hosting them in the title bar put them outside the page, where the pane could
    /// neither dim them nor stop a second blade or a theme switch from being triggered underneath one.
    /// </remarks>
    /// <param name="withClock">Ignored; kept for the base signature.</param>
    /// <returns>Always <see langword="null"/>: the platform title bar renders the window title.</returns>
    protected override TitleBar? SetTitleBar(bool withClock = false) => null;

    /// <summary>
    /// Applies the language chosen in Site Settings to the current and default cultures.
    /// Must be called before any resource is read, so the shell derives its flow direction
    /// (RTL for Arabic) from the correct culture.
    /// </summary>
    private void ApplySavedLanguage()
    {
        var language = _settingsService?.LocalSettings?.Language;

        if (string.IsNullOrWhiteSpace(language))
            return;

        try
        {
            var culture = new CultureInfo(language);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        catch (CultureNotFoundException ex)
        {
            _logger?.LogWarning(ex, "Unknown language '{Language}' in settings", language);
        }
    }

    /// <summary>
    /// Called during framework initialization. Override to perform backend initialization,
    /// load data, apply migrations, etc. Must call SignalApplicationInitialized at the end.
    /// </summary>
    protected override async Task InitializeApplicationAsync()
    {
        try
        {
            _logger?.LogTrace("Starting application initialization");

            // The framework's language service resolves strings through CultureInfo, and the shell
            // derives its flow direction from it, so the saved language has to be applied before
            // anything reads a resource.
            ApplySavedLanguage();

            await ApplyThemeAndAccentAsync();

            // Perform backend initialization, data loading, migrations, etc.
            await RunBusyStepAsync("Start doing important stuff", "Done doing important stuff");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in first part of InitializeApplicationAsync");
            await ShowInitializationErrorAsync("Failed doing important stuff");
        }

        try
        {
            await RunBusyStepAsync("Applying migrations", "Done applying migrations");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in second part of InitializeApplicationAsync");
            await ShowInitializationErrorAsync("Failed to apply migrations");
        }

        // Signal to the framework that initialization is complete.
        // This will trigger ApplicationLoaded event if UI is also ready.
        _logger?.LogTrace("Application initialization complete, signaling framework");
        _lifecycleService?.SignalApplicationInitialized();
    }

    private static async Task ApplyThemeAndAccentAsync()
    {
        // Apply the user's saved theme and accent colour via IThemeService before any view can render.
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var themeService = IPlatformApplication.Current?.Services.GetService<IThemeService>();
            if (themeService is not null)
            {
                themeService.ApplyTheme();

                // Initialize dynamic accent tokens after theme is applied
                var accentTokens = IPlatformApplication.Current?.Services.GetService<ShellAccentTokens>();
                accentTokens?.ApplyAccentTokens();
            }
        });
    }

    private async Task RunBusyStepAsync(string startMessage, string doneMessage)
    {
        if (_commonServices?.BusyService != null)
        {
            _commonServices.BusyService.UpdateMessage(startMessage);
            await Task.Delay(100);
            _commonServices.BusyService.UpdateMessage(doneMessage);
            await Task.Delay(100);
        }
    }

    private async Task ShowInitializationErrorAsync(string message)
    {
        if (_dialogService != null)
        {
            try
            {
                await _dialogService.ShowErrorAsync(message, "Fake error message");
            }
            catch (Exception dialogEx)
            {
                _logger?.LogError(dialogEx, "Error showing dialog");
            }
        }
    }

    /// <summary>
    /// Called when both UI is ready AND application initialization is complete.
    /// This replaces the legacy ApplicationLoadedMessage handler.
    /// Handle post-load navigation, auto-login, and other startup operations here.
    /// </summary>
    protected override async void OnApplicationLoaded(object? sender, EventArgs e)
    {
        base.OnApplicationLoaded(sender, e);

        try
        {
            _commonServices.BusyService.StartBusy();

            // Wait for the loading view to complete if it's a video (for Image and None types, this returns immediately)
            await WaitForLoadingViewAsync();

            _logger.LogInformation("Application loaded event: checking for auto-login");
            _logger.LogInformation("Navigate to SignIn page");


            // The shell is the window root, not a modal page: it stays alive for the lifetime of
            // the window so its blade pane (the last child of the root grid) can cover every
            // section. Navigation happens inside the shell's content host (the MAUI equivalent of
            // the WinUI content frame), so the root itself is a plain page.
            var shellView = IPlatformApplication.Current!.Services.GetRequiredService<ShellView>();

            Microsoft.Maui.Controls.Application.Current!.Windows[0].Page = shellView;

            await shellView.ViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ApplicationLoaded");
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    /// <summary>
    /// Customizes the <see cref="NumberFormatInfo"/> after the culture is applied.
    /// </summary>
    protected override void ConfigureNumberFormat(CultureInfo culture)
    {
        base.ConfigureNumberFormat(culture);

        if (_settingsService?.GlobalSettings is not null)
        {
            culture.NumberFormat.CurrencySymbol = _settingsService.GlobalSettings.CurrencySymbol;
            culture.NumberFormat.CurrencyDecimalDigits = _settingsService.GlobalSettings.Decimals;
            culture.NumberFormat.NumberDecimalDigits = _settingsService.GlobalSettings.Decimals;
            culture.NumberFormat.CurrencyNegativePattern = 1;
            culture.NumberFormat.NumberNegativePattern = 1;
            culture.NumberFormat.PercentNegativePattern = 1;
        }
    }

    /// <summary>
    /// Called when a user signs in.
    /// The profile has already been set in the context by the AuthenticationService.
    /// </summary>
    private async void OnUserSignedIn(UserSignedInMessage message)
    {
        try
        {
            _logger?.LogTrace("Authentication succeeded event received");

            // Re-apply the culture so ConfigureNumberFormat picks up the
            // latest GlobalSettings (decimals, etc.). The framework handles
            // both DefaultThreadCurrentCulture and UI-thread CurrentCulture.
            var culture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture;
            culture.ApplyCulture(ConfigureNumberFormat);

            // Pop all modals to reveal the shell. The shell itself is never replaced
            // (modals are pushed on top via PushModalAsync).
            while (_navigationService.CanGoBack)
                await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnUserSignedIn");

            if (_dialogService != null)
            {
                try
                {
                    await _dialogService.ShowErrorAsync("Error occurred during authentication success handling. The application may need to be restarted.");
                }
                catch
                {
                    // Can't show dialog
                }
            }
        }
    }

    /// <summary>
    /// Called when a user signs out.
    /// The context has already been cleared by the AuthenticationService.
    /// </summary>
    private async void OnUserSignedOut(UserSignedOutMessage message)
    {
        try
        {
            _logger?.LogTrace("Authentication failed event received");

            // Context should already be cleared by AuthenticationService
            // Navigate to SignIn page
            await _navigationService.NavigateModalAsync<SignInViewModel>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnUserSignedOut");

            if (_dialogService != null)
            {
                try
                {
                    await _dialogService.ShowErrorAsync("Error occurred during authentication failure handling. The application may need to be restarted.");
                }
                catch
                {
                    // Can't show dialog
                }
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // Unregister message handlers
            _commonServices.MessengerService.Unregister<ShowInformationMessage>(this);
            _commonServices.MessengerService.Unregister<ShowWarningMessage>(this);
            _commonServices.MessengerService.Unregister<ShowErrorMessage>(this);
            _commonServices.MessengerService.Unregister<UserSignedInMessage>(this);
            _commonServices.MessengerService.Unregister<UserSignedOutMessage>(this);
        }
    }
}
