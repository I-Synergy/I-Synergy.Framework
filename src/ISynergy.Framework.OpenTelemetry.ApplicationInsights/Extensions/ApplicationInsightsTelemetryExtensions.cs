using Azure.Monitor.OpenTelemetry.Exporter;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;

namespace ISynergy.Framework.OpenTelemetry.Extensions;
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
        telemetryBuilder.HostBuilderContext.Configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

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

    private static void Map(this AzureMonitorExporterOptions source, AzureMonitorExporterOptions target)
    {
        foreach (var prop in source.GetType().GetProperties())
        {
            if (prop.CanRead)
            {
                // Check if the property exists in the target type
                var targetProp = target.GetType().GetProperty(prop.Name);

                // Only proceed if the property exists in the target and is writable
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = prop.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }
        }
    }
}
