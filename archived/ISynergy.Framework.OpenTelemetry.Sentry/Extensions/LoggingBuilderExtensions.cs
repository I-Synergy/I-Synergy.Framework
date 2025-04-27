using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.OpenTelemetry.Extensions;
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds OpenTelemetry to the logging builder.
    /// </summary>
    public static ILoggingBuilder AddSentryTelemetry(
        this ILoggingBuilder builder,
        IConfiguration configuration,
        IHostEnvironment environment,
        IInfoService infoService,
        string defaultTelemetrySection = "Telemetry",
        Action<TracerProviderBuilder>? tracingAction = null,
        Action<MeterProviderBuilder>? metricsAction = null,
        Action<LoggerProviderBuilder>? loggerAction = null)
    {
        builder.Services.AddSentryTelemetry(configuration, environment, infoService, defaultTelemetrySection, tracingAction, metricsAction, loggerAction);
        return builder;
    }
}
