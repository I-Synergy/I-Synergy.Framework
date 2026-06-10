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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NugetUnlister.Extensions;
using Sample.Services;
using Sample.ViewModels;
using System.Reflection;
using System.Windows;

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
                services.TryAddSingleton<Sample.Abstractions.Services.IAuthenticationService, AuthenticationService>();
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
            });

        return hostBuilder;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Subscribe to authentication messages
        _commonServices.MessengerService.Register<UserSignedInMessage>(this, OnUserSignedIn, keepTargetAlive: true);
        _commonServices.MessengerService.Register<UserSignedOutMessage>(this, OnUserSignedOut, keepTargetAlive: true);
    }

    protected override async void OnApplicationLoaded(object? sender, EventArgs e)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            _logger.LogTrace("Navigate to SignIn page");
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
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
