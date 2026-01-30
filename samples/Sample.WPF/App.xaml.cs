using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Models.Results;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NugetUnlister.Extensions;
using Sample.Abstractions.Services;
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
                services.TryAddSingleton<IAuthenticationService, AuthenticationService>();
                services.TryAddSingleton<IFileService<FileResult>, FileService>();
                services.TryAddSingleton<IUnitConversionService, UnitConversionService>();

                services.AddNugetServiceIntegrations(configuration);
            },
            mainAssembly,
            f => f.Name!.StartsWith(typeof(App).Namespace!));

        return hostBuilder;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Subscribe to authentication events
        var authenticationService = _commonServices.ScopedContextService.GetRequiredService<IAuthenticationService>();
        authenticationService.AuthenticationSucceeded += async (s, args) => await OnAuthenticationSucceededAsync(args);
        authenticationService.AuthenticationFailed += async (s, args) => await OnAuthenticationFailedAsync();

        RaiseApplicationLoaded();
    }

    protected override async void OnApplicationLoaded(object? sender, ReturnEventArgs<bool> e)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            if (navigateToAuthentication)
            {
                _logger.LogTrace("Navigate to SignIn page");
                await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
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
            _logger?.LogTrace("Navigate to Shell");
            await _navigationService.NavigateModalAsync<IShellViewModel>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnAuthenticationSucceededAsync");
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
            _logger?.LogTrace("Navigate to SignIn page");
            await _navigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnAuthenticationFailedAsync");
        }
    }

    public override async Task InitializeApplicationAsync()
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
                //await _migrationService.ApplyMigrationAsync<_001>();
                await Task.Delay(2000);
                _commonServices.BusyService.UpdateMessage("Done applying migrations");
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _dialogService.ShowErrorAsync("Failed to apply migrations", "Fake error message");
            }

            RaiseApplicationInitialized();
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }
}
