using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.OpenTelemetry.Abstractions.Exporters;
/// <summary>
/// Interface for OpenTelemetry exporters that can be registered with the telemetry system.
/// </summary>
public interface ITelemetryExporter
{
    /// <summary>
    /// Configures the OpenTelemetry logging with this exporter.
    /// </summary>
    /// <param name="loggerProviderBuilder">The logging options to configure.</param>
    /// <param name="options"></param>
    void ConfigureLogging(LoggerProviderBuilder? loggerProviderBuilder, OpenTelemetryLoggerOptions? options);

    /// <summary>
    /// Configures the OpenTelemetry tracing with this exporter.
    /// </summary>
    /// <param name="tracerProviderBuilder"></param>
    void ConfigureTracing(TracerProviderBuilder tracerProviderBuilder);

    /// <summary>
    /// Configures the OpenTelemetry metrics with this exporter.
    /// </summary>
    /// <param name="meterProviderBuilder"></param>
    void ConfigureMetrics(MeterProviderBuilder meterProviderBuilder);
}
