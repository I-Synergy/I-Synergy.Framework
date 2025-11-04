using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.ViewModels;
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

            // Get the authentication service for authentication state changes
            var authenticationService = _commonServices.ScopedContextService.GetRequiredService<IAuthenticationService>();

            // Subscribe to authentication events
            authenticationService.AuthenticationSucceeded += async (s, e) => await OnAuthenticationSucceededAsync(e);
            authenticationService.AuthenticationFailed += async (s, e) => await OnAuthenticationFailedAsync();

            // Dialog message handlers (unchanged)
            _commonServices.MessengerService.Register<ShowInformationMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowInformationAsync(m.Content.Message, m.Content.Title);
            });

            _commonServices.MessengerService.Register<ShowWarningMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title);
            });

            _commonServices.MessengerService.Register<ShowErrorMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title);
            });
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Error in App constructor");
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

            // Perform backend initialization, data loading, migrations, etc.
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
                //await _migrationService.ApplyMigrationAsync<_001>();
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

            bool navigateToAuthentication = true;

            _logger.LogInformation("Application loaded event: checking for auto-login");

            if (navigateToAuthentication)
            {
                _logger.LogInformation("Navigate to SignIn page");
                await _navigationService.NavigateModalAsync<SignInViewModel>();
            }
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
    /// The profile has already been set in the context by the AuthenticationService.
    /// </summary>
    private async Task OnAuthenticationSucceededAsync(AuthenticationSuccessEventArgs e)
    {
        try
        {
            _logger?.LogTrace("Authentication succeeded event received");

            // Profile should already be set in context by AuthenticationService
            // Navigate to Shell to show authenticated UI
            await _navigationService.NavigateModalAsync<ShellViewModel>();
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
    /// The context has already been cleared by the AuthenticationService.
    /// </summary>
    private async Task OnAuthenticationFailedAsync()
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // Unregister message handlers
            MessengerService.Default.Unregister<ShowInformationMessage>(this);
            MessengerService.Default.Unregister<ShowWarningMessage>(this);
            MessengerService.Default.Unregister<ShowErrorMessage>(this);
        }
    }
}
