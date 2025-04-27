using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.OpenTelemetry.Extensions;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Sentry to the service collection directly.
    /// </summary>
    public static IServiceCollection AddSentryTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        IInfoService infoService,
        string defaultTelemetrySection = "Telemetry",
        Action<TracerProviderBuilder>? tracingAction = null,
        Action<MeterProviderBuilder>? metricsAction = null,
        Action<LoggerProviderBuilder>? loggerAction = null)
    {
        return services.AddTelemetry<SentryProvider>(configuration, environment, infoService, defaultTelemetrySection, tracingAction, metricsAction, loggerAction);
    }
}
