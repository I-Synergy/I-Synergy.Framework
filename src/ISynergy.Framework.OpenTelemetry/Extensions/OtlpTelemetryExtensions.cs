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

    private static void Map(this OtlpExporterOptions source, OtlpExporterOptions target)
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
