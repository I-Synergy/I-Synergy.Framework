using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
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
    /// <remarks>
    /// Configuration keys are bound manually (trim-safe, no reflection): Endpoint, Headers,
    /// TimeoutMilliseconds, Protocol, ExportProcessorType and
    /// BatchExportProcessorOptions.{MaxQueueSize, ScheduledDelayMilliseconds, ExporterTimeoutMilliseconds, MaxExportBatchSize}.
    /// <c>Headers</c> keeps the OTLP <c>key1=value1,key2=value2</c> string format.
    /// NOTE: any property added to <see cref="OtlpExporterOptions"/> binding later must be bound explicitly here.
    /// </remarks>
    public static TelemetryBuilder AddOtlpExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<OtlpExporterOptions>? telemetryExporterOptions = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        var section = telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection);
        var options = new OtlpExporterOptions();

        if (Uri.TryCreate(section[nameof(OtlpExporterOptions.Endpoint)], UriKind.Absolute, out var endpoint))
            options.Endpoint = endpoint;
        if (section[nameof(OtlpExporterOptions.Headers)] is { Length: > 0 } headers)
            options.Headers = headers;
        if (int.TryParse(section[nameof(OtlpExporterOptions.TimeoutMilliseconds)], out var timeoutMilliseconds))
            options.TimeoutMilliseconds = timeoutMilliseconds;
        if (Enum.TryParse<OtlpExportProtocol>(section[nameof(OtlpExporterOptions.Protocol)], ignoreCase: true, out var protocol))
            options.Protocol = protocol;
        if (Enum.TryParse<ExportProcessorType>(section[nameof(OtlpExporterOptions.ExportProcessorType)], ignoreCase: true, out var exportProcessorType))
            options.ExportProcessorType = exportProcessorType;

        var batchSection = section.GetSection(nameof(OtlpExporterOptions.BatchExportProcessorOptions));
        if (batchSection.Exists())
        {
            var batch = options.BatchExportProcessorOptions;
            if (int.TryParse(batchSection[nameof(batch.MaxQueueSize)], out var maxQueueSize)) batch.MaxQueueSize = maxQueueSize;
            if (int.TryParse(batchSection[nameof(batch.ScheduledDelayMilliseconds)], out var scheduledDelay)) batch.ScheduledDelayMilliseconds = scheduledDelay;
            if (int.TryParse(batchSection[nameof(batch.ExporterTimeoutMilliseconds)], out var exporterTimeout)) batch.ExporterTimeoutMilliseconds = exporterTimeout;
            if (int.TryParse(batchSection[nameof(batch.MaxExportBatchSize)], out var maxExportBatchSize)) batch.MaxExportBatchSize = maxExportBatchSize;
        }

        telemetryExporterOptions?.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Endpoint?.ToString()))
        {
            System.Diagnostics.Debug.WriteLine("OTLP endpoint is not configured. Skipping OTLP exporter registration.");
            return telemetryBuilder;
        }

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
