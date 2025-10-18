using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Messages;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Sample.ViewModels;
using System.Globalization;
using Application = ISynergy.Framework.UI.Application;

namespace Sample;

public partial class App : Application
{
    public App()
        : base()
    {
        try
        {
            InitializeComponent();

            MessengerService.Default.Register<ApplicationLoadedMessage>(this, async (m) => await ApplicationLoadedAsync(m));
            MessengerService.Default.Register<AuthenticationChangedMessage>(this, async m => await AuthenticationChangedAsync(m));

            MessengerService.Default.Register<ShowInformationMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowInformationAsync(m.Content.Message, m.Content.Title);

                //if (dialogResult is not null)
                //{
                //    var result = await dialogResult.Result;

                //    //if (result.Cancelled)
                //    //    return MessageBoxResult.Cancel;

                //    //return MessageBoxResult.OK;
                //}

                //return MessageBoxResult.None;
            });

            MessengerService.Default.Register<ShowWarningMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowWarningAsync(m.Content.Message, m.Content.Title);

                //if (dialogResult is not null)
                //{
                //    var result = await dialogResult.Result;

                //    //if (result.Cancelled)
                //    //    return MessageBoxResult.Cancel;

                //    //return MessageBoxResult.OK;
                //}

                //return MessageBoxResult.None;
            });

            MessengerService.Default.Register<ShowErrorMessage>(this, async m =>
            {
                var dialogResult = await _dialogService.ShowErrorAsync(m.Content.Message, m.Content.Title);

                //if (dialogResult is not null)
                //{
                //    var result = await dialogResult.Result;

                //    //if (result.Cancelled)
                //    //    return MessageBoxResult.Cancel;

                //    //return MessageBoxResult.OK;
                //}

                //return MessageBoxResult.None;
            });
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Error in App constructor");
        }
    }

    private async Task AuthenticationChangedAsync(AuthenticationChangedMessage authenticationChangedMessage)
    {
        if (authenticationChangedMessage.Content)
            await _navigationService.NavigateModalAsync<ShellViewModel>();
        else
            await _navigationService.NavigateModalAsync<SignInViewModel>();
    }

    protected override async Task InitializeApplicationAsync()
    {
        try
        {
            _logger?.LogTrace("Starting application initialization");

            // Initialize exception handling first
            await InitializeExceptionHandlingAsync();

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

        _logger?.LogTrace("Sending ApplicationInitializedMessage");
        MessengerService.Default.Send(new ApplicationInitializedMessage());
    }

    private async Task ApplicationLoadedAsync(ApplicationLoadedMessage message)
    {
        try
        {
#if WINDOWS
            //commonServices.BusyService.StartBusy(commonServices.LanguageService.GetString("UpdateCheckForUpdates"));

            //if (await ServiceLocator.Default.GetInstance<IUpdateService>().CheckForUpdateAsync() && await commonServices.DialogService.ShowMessageAsync(
            //    commonServices.LanguageService.GetString("UpdateFoundNewUpdate") + System.Environment.NewLine + commonServices.LanguageService.GetString("UpdateExecuteNow"),
            //    "Update",
            //    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    commonServices.BusyService.BusyMessage = commonServices.LanguageService.GetString("UpdateDownloadAndInstall");
            //    await ServiceLocator.Default.GetInstance<IUpdateService>().DownloadAndInstallUpdateAsync();
            //    Environment.Exit(Environment.ExitCode);
            //}
#endif
            _commonServices.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            _logger.LogInformation("Retrieve default user and check for auto login");

            //if (!string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser) && _settingsService.LocalSettings.IsAutoLogin)
            //{
            //    string username = _settingsService.LocalSettings.DefaultUser;
            //    string password = await ServiceLocator.Default.GetRequiredService<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

            //    if (!string.IsNullOrEmpty(password))
            //    {
            //        await _authenticationService.AuthenticateWithUsernamePasswordAsync(username, password, _settingsService.LocalSettings.IsAutoLogin);
            //        navigateToAuthentication = false;
            //    }
            //}

            if (navigateToAuthentication)
            {
                _logger.LogInformation("Navigate to SignIn page");
                await _navigationService.NavigateModalAsync<SignInViewModel>();
            }
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    protected override async void AuthenticationChanged(object? sender, ReturnEventArgs<bool> e)
    {
        // Suppress backstack change event during sign out
        await _navigationService.CleanBackStackAsync(suppressEvent: !e.Value);

        try
        {
            if (_commonServices?.BusyService != null)
                _commonServices.BusyService.StartBusy();

            if (e.Value)
            {
                try
                {
                    var context = _commonServices?.ScopedContextService.GetRequiredService<IContext>();

                    if (context != null && _settingsService?.LocalSettings != null)
                    {
                        _logger?.LogTrace("Saving refresh token");
                        _settingsService.LocalSettings.RefreshToken = context.ToEnvironmentalRefreshToken();
                        _settingsService.SaveLocalSettings();
                    }

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
                    _logger?.LogError(ex, "Error handling authentication success");

                    if (_dialogService != null)
                    {
                        try
                        {
                            await _dialogService.ShowErrorAsync("Error occurred after login. The application may need to be restarted.");
                        }
                        catch
                        {
                            // Can't show dialog
                        }
                    }
                }
            }
            else
            {
                try
                {
                    Baggage.ClearBaggage();

                    _logger?.LogTrace("Navigate to SignIn page");
                    if (_navigationService != null)
                    {
                        await _navigationService.NavigateModalAsync<SignInViewModel>();
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error handling authentication failure");

                    if (_dialogService != null)
                    {
                        try
                        {
                            await _dialogService.ShowErrorAsync("Error occurred during logout. The application may need to be restarted.");
                        }
                        catch
                        {
                            // Can't show dialog
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unhandled error in OnAuthenticationChanged");
        }
        finally
        {
            if (_commonServices?.BusyService != null)
                _commonServices.BusyService.StopBusy();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            MessengerService.Default.Unregister<ApplicationLoadedMessage>(this);
            MessengerService.Default.Unregister<AuthenticationChangedMessage>(this);
            MessengerService.Default.Unregister<ShowInformationMessage>(this);
            MessengerService.Default.Unregister<ShowWarningMessage>(this);
            MessengerService.Default.Unregister<ShowErrorMessage>(this);
        }

    }
}
