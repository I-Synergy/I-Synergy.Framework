using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.OpenTelemetry.Extensions;

/// <summary>
/// Extension methods for setting up OpenTelemetry integration.
/// </summary>
public static class OtlpTelemetryExtensions
{
    /// <summary>
    /// Adds an OTLP exporter to the telemetry pipeline.
    /// </summary>
    /// <param name="telemetryBuilder">The telemetry builder.</param>
    /// <param name="telemetryExporterOptions"></param>
    /// <returns>The telemetry builder for chaining.</returns>
    public static TelemetryBuilder AddOtlpExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<OtlpExporterOptions>? telemetryExporterOptions = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        var options = new OtlpExporterOptions();
        telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

        telemetryExporterOptions?.Invoke(options);

        telemetryBuilder
            .OpenTelemetryBuilder
                .WithTracing(builder => builder
                        .AddOtlpExporter(otlpOptions => options.Map(otlpOptions)))
                .WithMetrics(builder => builder
                        .AddOtlpExporter(otlpOptions => options.Map(otlpOptions)))
                .WithLogging(builder => builder
                        .AddOtlpExporter(otlpOptions => options.Map(otlpOptions)));

        return telemetryBuilder;
    }

    /// <summary>
    /// Copies all public settable properties from <paramref name="source"/> to <paramref name="target"/>.
    /// </summary>
    /// <param name="source">The source options to copy from.</param>
    /// <param name="target">The target options to copy to.</param>
    /// <remarks>
    /// Uses explicit property assignments instead of reflection to ensure AOT (Ahead-of-Time) compilation
    /// compatibility. Reflection-based property copying is blocked under AOT trimming.
    /// </remarks>
    private static void Map(this OtlpExporterOptions source, OtlpExporterOptions target)
    {
        target.Endpoint = source.Endpoint;
        target.Headers = source.Headers;
        target.TimeoutMilliseconds = source.TimeoutMilliseconds;
        target.Protocol = source.Protocol;
        target.UserAgentProductIdentifier = source.UserAgentProductIdentifier;
        target.ExportProcessorType = source.ExportProcessorType;
        target.BatchExportProcessorOptions = source.BatchExportProcessorOptions;
        target.HttpClientFactory = source.HttpClientFactory;
    }
}
