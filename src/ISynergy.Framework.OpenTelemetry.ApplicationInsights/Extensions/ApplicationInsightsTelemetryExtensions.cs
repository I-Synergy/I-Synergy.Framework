using Azure.Monitor.OpenTelemetry.Exporter;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;

namespace ISynergy.Framework.OpenTelemetry.ApplicationInsights.Extensions;
/// <summary>
/// Extension methods for adding Application Insights exporter to telemetry.
/// </summary>
public static class ApplicationInsightsTelemetryExtensions
{
    /// <summary>
    /// Adds the Application Insights exporter to the telemetry pipeline.
    /// </summary>
    /// <param name="telemetryBuilder">The telemetry builder.</param>
    /// <param name="telemetryExporterOptions"></param>
    /// <returns>The telemetry builder for chaining.</returns>
    public static TelemetryBuilder AddApplicationInsightsExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<AzureMonitorExporterOptions>? telemetryExporterOptions = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        var options = new AzureMonitorExporterOptions();
        telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

        telemetryExporterOptions?.Invoke(options);

        // Configure all telemetry types with the same options
        telemetryBuilder
            .OpenTelemetryBuilder
                .WithTracing(builder => builder
                    .AddAzureMonitorTraceExporter(target => options.Map(target)))
                .WithMetrics(builder => builder
                    .AddAzureMonitorMetricExporter(target => options.Map(target)))
                .WithLogging(builder => builder
                    .AddAzureMonitorLogExporter(target => options.Map(target)));

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
    private static void Map(this AzureMonitorExporterOptions source, AzureMonitorExporterOptions target)
    {
        target.ConnectionString = source.ConnectionString;
        target.Credential = source.Credential;
        target.SamplingRatio = source.SamplingRatio;
        target.TracesPerSecond = source.TracesPerSecond;
        target.Version = source.Version;
        target.StorageDirectory = source.StorageDirectory;
        target.DisableOfflineStorage = source.DisableOfflineStorage;
        target.EnableLiveMetrics = source.EnableLiveMetrics;
        target.EnableTraceBasedLogsSampler = source.EnableTraceBasedLogsSampler;
        target.Transport = source.Transport;
        target.RetryPolicy = source.RetryPolicy;
    }
}
