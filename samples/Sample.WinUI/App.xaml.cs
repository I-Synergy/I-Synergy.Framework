using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.UI.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Sample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    private readonly ICredentialLockerService? _credentialLockerService;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
        : base()
    //: base(new SplashScreenOptions
    //{
    //    AssetStreamProvider = () => Task.FromResult(
    //        Assembly.GetAssembly(typeof(App))?.GetManifestResourceStream("Sample.Assets.gta.mp4")!),
    //    ContentType = "video/mp4",
    //    SplashScreenType = SplashScreenTypes.Video
    //})
    {
        try
        {
            InitializeComponent();

            _credentialLockerService = _commonServices?.ScopedContextService.GetService<ICredentialLockerService>();
        }
        catch (Exception ex)
        {
            _logger?.LogCritical(ex, "Error in App constructor");
        }
    }

    protected override IHostBuilder CreateHostBuilder()
    {
        try
        {
            var mainAssembly = Assembly.GetExecutingAssembly();
            var infoService = new InfoService();
            infoService.LoadAssembly(mainAssembly);

            var hostBuilder = new HostBuilder();

            hostBuilder
                .ConfigureHostConfiguration(builder =>
                {
                    try
                    {
                        var resourceStream = mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json");
                        if (resourceStream != null)
                        {
                            builder.AddJsonStream(resourceStream);
                        }
                        else
                        {
                            throw new InvalidOperationException("Could not find appsettings.json resource");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error configuring host: {ex}");
                        throw;
                    }
                })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    try
                    {
                        loggingBuilder
                            .AddTelemetry(context, infoService)
                            .AddOtlpExporter()
                            .AddApplicationInsightsExporter()
                            .AddSentryExporter(
                                options =>
                                {
                                    options.Environment = context.HostingEnvironment.EnvironmentName;
                                    options.Debug = context.HostingEnvironment.IsDevelopment();
                                });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error configuring logging: {ex}");
                        throw;
                    }
                })
                .ConfigureServices<Context, CommonServices, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>(infoService, (configuration, environment, services) =>
                {
                    services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                    services.TryAddSingleton<IFileService<FileResult>, FileService>();
                    services.TryAddSingleton<ICameraService, CameraService>();
                },
                mainAssembly,
                f => f.Name!.StartsWith(typeof(App).Namespace!));

            return hostBuilder;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical error creating host builder: {ex}");
            throw;
        }
    }

    protected override async Task HandleApplicationInitializedAsync()
    {
        try
        {
            _commonServices?.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            _logger?.LogTrace("Retrieve default user and check for auto login");

            if (_settingsService?.LocalSettings != null &&
                !string.IsNullOrEmpty(_settingsService.LocalSettings.DefaultUser) &&
                _settingsService.LocalSettings.IsAutoLogin &&
                _credentialLockerService != null)
            {
                string username = _settingsService.LocalSettings.DefaultUser;

                try
                {
                    string? password = await _credentialLockerService.GetPasswordFromCredentialLockerAsync(username);

                    if (!string.IsNullOrEmpty(password) && _commonServices.ScopedContextService.GetRequiredService<IAuthenticationService>() is AuthenticationService authenticationService)
                    {
                        await authenticationService.AuthenticateWithUsernamePasswordAsync(
                            username,
                            password,
                            _settingsService.LocalSettings.IsAutoLogin);

                        navigateToAuthentication = false;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error retrieving credentials");
                    // Continue to authentication screen on credential error
                    navigateToAuthentication = true;
                }
            }

            if (navigateToAuthentication && _navigationService != null)
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

    protected override async Task InitializeApplicationAsync()
    {
        try
        {
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
    }

    protected override async void OnAuthenticationChanged(object? sender, ReturnEventArgs<bool> e)
    {
        // Suppress backstack change event during sign out
        _navigationService.CleanBackStack(suppressEvent: !e.Value);

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
                        await _settingsService.SaveLocalSettingsAsync();
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
                        await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
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

    protected override void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        // Add specific handling for App-level exceptions if needed
        base.CurrentDomain_FirstChanceException(sender, e);
    }
}

