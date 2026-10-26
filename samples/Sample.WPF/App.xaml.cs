using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Messages;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.OpenTelemetry.ApplicationInsights.Extensions;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.OpenTelemetry.Sentry.Extensions;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NugetUnlister.Extensions;
using Sample.OpenTelemetry;
using Sample.Services;
using Sample.ViewModels;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
        : base()
    {
    }

    /// <summary>
    /// The sample renders its own header and caption buttons, so the OS title bar is removed.
    /// </summary>
    protected override bool UseCustomWindowChrome => true;

    protected override IHostBuilder CreateHostBuilder()
    {
        var mainAssembly = Assembly.GetExecutingAssembly();
        var infoService = new InfoService();
        infoService.LoadAssembly(mainAssembly);

        var hostBuilder = new HostBuilder();

        hostBuilder
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddJsonStream(mainAssembly.GetManifestResourceStream($"{mainAssembly.GetName().Name}.appsettings.json")!);
            })
            .ConfigureLogging((context, loggingBuilder) =>
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
            })
            .ConfigureServices<Context, CommonServices, ExceptionHandlerService, SettingsService, Properties.Resources>(infoService, (configuration, environment, services) =>
            {
                services.TryAddSingleton<ApplicationMonitor>();
                services.TryAddSingleton<Abstractions.Services.IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<IFileService<FileResult>, FileService>();
                services.TryAddSingleton<IUnitConversionService, UnitConversionService>();

                services.AddNugetServiceIntegrations(configuration);
            },
            mainAssembly,
            f => f.Name!.StartsWith(typeof(App).Namespace!))
            .ConfigureSupportedCultures(cultures =>
            {
                cultures.AddCulture("nl");
                cultures.AddCulture("de");
                cultures.AddCulture("fr");
                cultures.AddCulture("ar");
            })
            .UseThemeColors(colors => colors.Add("#0078D4"));

        return hostBuilder;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Apply the design tokens before the framework creates the window and the splash page,
        // so the splash surface is already themed on the first frame.
        if (_themeService is not null)
            ApplySampleTokens(_themeService);

        base.OnStartup(e);

        // Keep the shared sample design tokens in sync with the current theme/accent.
        if (_themeService is not null)
        {
            _themeService.ThemeChanged += (_, _) => ApplySampleTokens(_themeService);
        }

        // Subscribe to authentication messages
        _commonServices.MessengerService.Register<UserSignedInMessage>(this, OnUserSignedIn, keepTargetAlive: true);
        _commonServices.MessengerService.Register<UserSignedOutMessage>(this, OnUserSignedOut, keepTargetAlive: true);
    }

    /// <summary>
    /// Applies the shared sample design tokens (light/dark surfaces, text and accent).
    /// </summary>
    private void ApplySampleTokens(IThemeService themeService)
    {
        var isDark = themeService.Theme == ISynergy.Framework.Core.Enumerations.Themes.Dark;

        SetBrush("AppBackgroundBrush", isDark ? "#0D1614" : "#F4F6F3");
        SetBrush("SplashBackgroundBrush", isDark ? "#0D1614" : "#F4F6F3");
        SetBrush("AppSurfaceBrush", isDark ? "#14201C" : "#FFFFFF");
        SetBrush("AppSurface2Brush", isDark ? "#1C2A25" : "#E8EDE9");
        SetBrush("AppControlBrush", isDark ? "#1C2A25" : "#E8EDE9");
        SetBrush("AppBorderBrush", isDark ? "#26352F" : "#E0E6E2");
        SetBrush("AppTextBrush", isDark ? "#EEF1EA" : "#14211E");
        SetBrush("AppMutedBrush", isDark ? "#9CB0A8" : "#55665F");
        SetBrush("AppAccentBrush", isDark ? "#5ED3B0" : themeService.AccentColor);
        SetBrush("AppAccentSoftBrush", isDark ? "#295ED3B0" : "#E1EEEA");
        SetBrush("AppInkBrush", isDark ? "#04231A" : "#FFFFFF");
    }

    private void SetBrush(string key, string hex)
    {
        try
        {
            if (ColorConverter.ConvertFromString(hex) is Color color)
                Resources[key] = new SolidColorBrush(color);
        }
        catch
        {
            // Keep the previous token when a persisted color is malformed.
        }
    }

    protected override async void OnApplicationLoaded(object? sender, EventArgs e)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            // The demo sample goes straight to the shell; authentication accepts any user.
            _logger.LogTrace("Authenticating demo user");
            var authenticationService = Services.GetRequiredService<Sample.Abstractions.Services.IAuthenticationService>();
            await authenticationService.AuthenticateWithUsernamePasswordAsync("admin", "Welkom01");
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    /// <summary>
    /// Called when a user signs in.
    /// </summary>
    private async void OnUserSignedIn(UserSignedInMessage message)
    {
        try
        {
            _logger?.LogTrace("Authentication succeeded event received");
            _logger?.LogTrace("Navigate to Shell");
            await _navigationService.NavigateModalAsync<IShellViewModel>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnUserSignedIn");
        }
    }

    /// <summary>
    /// Called when a user signs out.
    /// </summary>
    private async void OnUserSignedOut(UserSignedOutMessage message)
    {
        try
        {
            _logger?.LogTrace("Authentication failed event received");
            _logger?.LogTrace("Navigate to SignIn page");
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnUserSignedOut");
        }
    }

    protected override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            try
            {
                _commonServices.BusyService.UpdateMessage("Start doing important stuff");
                await Task.Delay(2000);
                _commonServices.BusyService.UpdateMessage("Done doing important stuff");
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _dialogService.ShowErrorAsync("Failed doing important stuff", "Fake error message");
            }

            try
            {
                _commonServices.BusyService.UpdateMessage("Applying migrations");
                await Task.Delay(2000);
                _commonServices.BusyService.UpdateMessage("Done applying migrations");
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _dialogService.ShowErrorAsync("Failed to apply migrations", "Fake error message");
            }

            _lifecycleService?.SignalApplicationInitialized();
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }
}
