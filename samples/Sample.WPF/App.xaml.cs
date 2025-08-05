using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.OpenTelemetry.Extensions;
using ISynergy.Framework.Physics.Abstractions;
using ISynergy.Framework.Physics.Services;
using ISynergy.Framework.UI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NugetUnlister.Extensions;
using Sample.Abstractions.Services;
using Sample.Models;
using Sample.Services;
using Sample.ViewModels;
using System.Reflection;
using System.Windows;

namespace Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : ISynergy.Framework.UI.Application
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
            .ConfigureServices<Context, CommonServices, SettingsService<LocalSettings, RoamingSettings, GlobalSettings>, Properties.Resources>(infoService, (configuration, environment, services) =>
            {
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

        RaiseApplicationLoaded();
    }

    protected override async void OnApplicationLoaded(object? sender, ReturnEventArgs<bool> e)
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            bool navigateToAuthentication = true;

            _logger.LogTrace("Retrieve default user and check for auto login");

            if (!string.IsNullOrEmpty(_commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser) && _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.IsAutoLogin)
            {
                string username = _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.DefaultUser;
                string password = await _commonServices.ScopedContextService.GetRequiredService<ICredentialLockerService>().GetPasswordFromCredentialLockerAsync(username);

                if (!string.IsNullOrEmpty(password) && _commonServices.ScopedContextService.GetRequiredService<IAuthenticationService>() is AuthenticationService authenticationService)
                {
                    await authenticationService.AuthenticateWithUsernamePasswordAsync(username, password, _commonServices.ScopedContextService.GetRequiredService<ISettingsService>().LocalSettings.IsAutoLogin);
                    navigateToAuthentication = false;
                }
            }

            if (navigateToAuthentication)
            {
                _logger.LogTrace("Navigate to SignIn page");
                await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
            }
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }

    protected override async void OnAuthenticationChanged(object? sender, ReturnEventArgs<bool> e)
    {
        // Suppress backstack change event during sign out
        _commonServices.NavigationService.CleanBackStack(suppressEvent: !e.Value);

        if (e.Value)
        {
            _logger.LogTrace("Navigate to Shell");
            await _commonServices.NavigationService.NavigateModalAsync<IShellViewModel>();
        }
        else
        {
            _logger.LogTrace("Navigate to SignIn page");
            await _commonServices.NavigationService.NavigateModalAsync<AuthenticationViewModel>();
        }
    }

    public override async Task InitializeApplicationAsync()
    {
        try
        {
            _commonServices.BusyService.StartBusy();

            try
            {
                _commonServices.BusyService.BusyMessage = "Start doing important stuff";
                await Task.Delay(2000);
                _commonServices.BusyService.BusyMessage = "Done doing important stuff";
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _commonServices.DialogService.ShowErrorAsync("Failed doing important stuff", "Fake error message");
            }

            try
            {
                _commonServices.BusyService.BusyMessage = "Applying migrations";
                //await _migrationService.ApplyMigrationAsync<_001>();
                await Task.Delay(2000);
                _commonServices.BusyService.BusyMessage = "Done applying migrations";
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await _commonServices.DialogService.ShowErrorAsync("Failed to apply migrations", "Fake error message");
            }

            RaiseApplicationInitialized();
        }
        finally
        {
            _commonServices.BusyService.StopBusy();
        }
    }
}
