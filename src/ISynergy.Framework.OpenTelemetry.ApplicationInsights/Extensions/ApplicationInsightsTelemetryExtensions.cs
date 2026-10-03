using Azure.Monitor.OpenTelemetry.Exporter;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using Microsoft.Extensions.Configuration;

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

        // IConfiguration.Bind(object) uses reflection over the target's property graph, which
        // NativeAOT trimming silently breaks (properties are left at their defaults instead of
        // throwing) -- most importantly ConnectionString, which then reads as null/empty below
        // and disables this exporter entirely with no error anywhere. Read each value explicitly
        // instead. Credential/Transport/Diagnostics/Retry/RetryPolicy are complex Azure SDK types
        // that IConfiguration.Bind could never populate from flat JSON config either way, so they
        // aren't included here -- they keep whatever AzureMonitorExporterOptions defaults to.
        var options = new AzureMonitorExporterOptions();
        var telemetrySection = telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection);
        options.ConnectionString = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.ConnectionString), options.ConnectionString);
        options.SamplingRatio = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.SamplingRatio), options.SamplingRatio);
        options.TracesPerSecond = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.TracesPerSecond), options.TracesPerSecond);
        options.Version = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.Version), options.Version);
        options.StorageDirectory = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.StorageDirectory), options.StorageDirectory);
        options.DisableOfflineStorage = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.DisableOfflineStorage), options.DisableOfflineStorage);
        options.EnableLiveMetrics = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnableLiveMetrics), options.EnableLiveMetrics);
        options.EnableTraceBasedLogsSampler = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnableTraceBasedLogsSampler), options.EnableTraceBasedLogsSampler);
        options.EnableStandardMetrics = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnableStandardMetrics), options.EnableStandardMetrics);
        options.EnablePerformanceCounters = telemetrySection.GetValue(nameof(AzureMonitorExporterOptions.EnablePerformanceCounters), options.EnablePerformanceCounters);


        telemetryExporterOptions?.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            return telemetryBuilder;

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
        target.EnableStandardMetrics = source.EnableStandardMetrics;
        target.EnablePerformanceCounters = source.EnablePerformanceCounters;
        target.EnableTraceBasedLogsSampler = source.EnableTraceBasedLogsSampler;
        target.Transport = source.Transport;
        target.RetryPolicy = source.RetryPolicy;
    }
}
