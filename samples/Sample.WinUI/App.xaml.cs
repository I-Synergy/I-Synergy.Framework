using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Sample.Abstractions.Services;
using Sample.ViewModels;
using System.Globalization;
using System.Runtime.ExceptionServices;

namespace Sample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
        : base()
    {
        try
        {
            InitializeComponent();

            // Subscribe to authentication events after component initialization
            var authenticationService = _commonServices?.ScopedContextService.GetRequiredService<IAuthenticationService>();

            if (authenticationService != null)
            {
                authenticationService.AuthenticationSucceeded += async (s, args) => await OnAuthenticationSucceededAsync(args);
                authenticationService.AuthenticationFailed += async (s, args) => await OnAuthenticationFailedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Error in App constructor");
        }
    }

    protected override async Task InitializeApplicationAsync()
    {
        try
        {
            _logger?.LogTrace("Starting application initialization");

            if (_commonServices?.BusyService != null)
            {
                _commonServices.BusyService.UpdateMessage("Start doing important stuff");
                await Task.Delay(2000);
                _commonServices.BusyService.UpdateMessage("Done doing important stuff");
                await Task.Delay(2000);
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
                await Task.Delay(2000);
                _commonServices.BusyService.UpdateMessage("Done applying migrations");
                await Task.Delay(2000);
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
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
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
    /// Called when authentication succeeds (user has logged in).
    /// </summary>
    private async Task OnAuthenticationSucceededAsync(AuthenticationSuccessEventArgs e)
    {
        try
        {
            _logger?.LogTrace("Authentication succeeded event received");
            _logger?.LogTrace("Setting culture");

            if (_settingsService?.GlobalSettings is not null &&
                CultureInfo.DefaultThreadCurrentCulture != null &&
                CultureInfo.DefaultThreadCurrentCulture.Clone() is CultureInfo culture)
            {
                try
                {
                    culture.NumberFormat.CurrencySymbol = "€";

                    culture.NumberFormat.CurrencyDecimalDigits = _settingsService.GlobalSettings.Decimals;
                    culture.NumberFormat.NumberDecimalDigits = _settingsService.GlobalSettings.Decimals;

                    culture.NumberFormat.CurrencyNegativePattern = 1;
                    culture.NumberFormat.NumberNegativePattern = 1;
                    culture.NumberFormat.PercentNegativePattern = 1;

                    CultureInfo.DefaultThreadCurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error setting culture");
                }
            }

            _logger?.LogTrace("Navigate to Shell");
            if (_navigationService != null)
            {
                await _navigationService.NavigateModalAsync<IShellViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnAuthenticationSucceededAsync");

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
    /// Called when authentication fails or user logs out.
    /// </summary>
    private async Task OnAuthenticationFailedAsync()
    {
        try
        {
            _logger?.LogTrace("Authentication failed event received");
            Baggage.ClearBaggage();

            _logger?.LogTrace("Navigate to SignIn page");
            if (_navigationService != null)
            {
                await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnAuthenticationFailedAsync");

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

    protected override async Task HandleApplicationInitializedAsync()
    {
        try
        {
            _commonServices?.BusyService.StartBusy();

            if (_navigationService != null)
            {
                _logger?.LogTrace("Navigate to SignIn page");
                try
                {
                    await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error navigating to authentication");

                    // Show error dialog as last resort
                    if (_dialogService != null)
                    {
                        await _dialogService.ShowErrorAsync("Failed to navigate to login screen. The application may need to be restarted.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in HandleApplicationInitializedAsync");

            // Show error dialog if possible
            if (_dialogService != null)
            {
                try
                {
                    await _dialogService.ShowErrorAsync("Application initialization failed. Please restart the application.");
                }
                catch
                {
                    // Last resort - can't even show an error dialog
                }
            }
        }
        finally
        {
            _commonServices?.BusyService.StopBusy();
        }
    }





    protected override void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        // Add specific handling for App-level exceptions if needed
        base.CurrentDomain_FirstChanceException(sender, e);
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
        }
    }
}

