using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Globalization.Extensions;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.Extensions.Logging;
using Sample.ViewModels;
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
    /// Provides the custom XAML-based title bar with search and settings.
    /// </summary>
    protected override Microsoft.Maui.Controls.TitleBar SetTitleBar() => new Sample.Controls.TitleBar();

    /// <summary>
    /// Called during framework initialization. Override to perform backend initialization,
    /// load data, apply migrations, etc. Must call SignalApplicationInitialized at the end.
    /// </summary>
    protected override async Task InitializeApplicationAsync()
    {
        try
        {
            _logger?.LogTrace("Starting application initialization");

            // Perform backend initialization, data loading, migrations, etc.
            if (_commonServices?.BusyService != null)
            {
                _commonServices.BusyService.UpdateMessage("Start doing important stuff");
                await Task.Delay(100);
                _commonServices.BusyService.UpdateMessage("Done doing important stuff");
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in first part of InitializeApplicationAsync");

            if (_dialogService != null)
            {
                try
                {
                    await _dialogService.ShowErrorAsync("Failed doing important stuff", "Fake error message");
                }
                catch (Exception dialogEx)
                {
                    _logger?.LogError(dialogEx, "Error showing dialog");
                }
            }
        }

        try
        {
            if (_commonServices?.BusyService != null)
            {
                _commonServices.BusyService.UpdateMessage("Applying migrations");
                await Task.Delay(100);
                _commonServices.BusyService.UpdateMessage("Done applying migrations");
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in second part of InitializeApplicationAsync");

            if (_dialogService != null)
            {
                try
                {
                    await _dialogService.ShowErrorAsync("Failed to apply migrations", "Fake error message");
                }
                catch (Exception dialogEx)
                {
                    _logger?.LogError(dialogEx, "Error showing dialog");
                }
            }
        }

        // Signal to the framework that initialization is complete.
        // This will trigger ApplicationLoaded event if UI is also ready.
        _logger?.LogTrace("Application initialization complete, signaling framework");
        _lifecycleService?.SignalApplicationInitialized();
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

            //await _navigationService.NavigateModalAsync<SignInViewModel>();

            await _navigationService.NavigateModalAsync<ShellViewModel>();
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

            // Pop all modals back to the Shell. Since Shell is now never
            // replaced (modals are pushed on top via PushModalAsync),
            // we just dismiss every modal to reveal the original Shell.
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

    public override void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        base.CurrentDomain_FirstChanceException(sender, e);
    }

    public override void CurrentDomain_UnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        base.CurrentDomain_UnhandledException(sender, e);
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
