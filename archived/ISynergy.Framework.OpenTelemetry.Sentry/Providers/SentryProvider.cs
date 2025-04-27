using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Sentry.Extensions.Logging.Extensions.DependencyInjection;
using Sentry.OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Providers;
/// <summary>
/// Sentry implementation of ITelemetryProvider.
/// </summary>
public class SentryProvider : OpenTelemetryProvider
{
    /// <summary>
    /// Configures Sentry with the specified service collection and options.
    /// </summary>
    public override void Configure(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        IInfoService infoService,
        string defaultTelemetrySection,
        Action<TracerProviderBuilder>? tracingAction = null,
        Action<MeterProviderBuilder>? metricsAction = null,
        Action<LoggerProviderBuilder>? loggerAction = null)
    {
        base.Configure(services, configuration, environment, infoService, defaultTelemetrySection, tracingAction, metricsAction, loggerAction);

        // Configure logging integration
        services.AddLogging(logging =>
        {
            logging
                .AddOpenTelemetry()
                .AddSentry(sentryOptions =>
                {
                    configuration.GetSection(defaultTelemetrySection).Bind(sentryOptions);

                    if (string.IsNullOrEmpty(sentryOptions.Environment))
                        sentryOptions.Environment = environment.EnvironmentName;

                    sentryOptions.Debug = environment.IsDevelopment();

                    if (string.IsNullOrEmpty(sentryOptions.ServerName))
                        sentryOptions.ServerName = infoService.ProductName;

                    if (string.IsNullOrEmpty(sentryOptions.Release))
                        sentryOptions.Release = infoService.ProductVersion.ToString();

                    sentryOptions.AddProfilingIntegration();
                });
        });

        // Register the ActivityAdapter to bridge between OpenTelemetry and Sentry
        services.AddSingleton<ActivityAdapter>(provider =>
        {
            var hub = provider.GetRequiredService<IHub>();
            return new ActivityAdapter(hub, _activitySource!);
        });
    }
}