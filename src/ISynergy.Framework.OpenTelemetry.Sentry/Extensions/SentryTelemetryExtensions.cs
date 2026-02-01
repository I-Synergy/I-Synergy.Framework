using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.OpenTelemetry.Constants;
using ISynergy.Framework.OpenTelemetry.Models;
using ISynergy.Framework.OpenTelemetry.Processors;
using OpenTelemetry.Logs;
using Sentry.OpenTelemetry;

namespace ISynergy.Framework.OpenTelemetry.Extensions;
/// <summary>
/// Extension methods for adding Sentry exporter to telemetry.
/// </summary>
public static class SentryTelemetryExtensions
{
    /// <summary>
    /// Adds the Sentry exporter to the telemetry pipeline.
    /// </summary>
    /// <param name="telemetryBuilder">The telemetry builder.</param>
    /// <param name="telemetryExporterOptions">Optional action to configure exporter options.</param>
    /// <returns>The telemetry builder for chaining.</returns>
    public static TelemetryBuilder AddSentryExporter(
        this TelemetryBuilder telemetryBuilder,
        Action<SentryOptions>? telemetryExporterOptions = null)
    {
        Argument.IsNotNull(telemetryBuilder);

        var options = new SentryOptions();
        telemetryBuilder.Configuration.GetSection(TelemetryConstants.TelemetrySection).BindWithReload(options);

        telemetryExporterOptions?.Invoke(options);

        SentrySdk.Init(sentryOptions =>
        {
            options.Map(sentryOptions);

            sentryOptions.UseOpenTelemetry();
        });

        telemetryBuilder.OpenTelemetryBuilder.WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder.AddSentry();
        });

        telemetryBuilder.OpenTelemetryBuilder.WithMetrics(meterProviderBuilder =>
        {
            // Sentry doesn't yet have direct OpenTelemetry metrics integration
            // Default implementation is empty and can be expanded when Sentry adds metrics support
        });

        telemetryBuilder.OpenTelemetryBuilder.WithLogging(loggerProviderBuilder =>
        {
            // Check if there's a specific extension method for OpenTelemetry logging
            // If not available, add a processor to convert logs to Sentry events
            loggerProviderBuilder.AddProcessor(new OpenTelemetryLogProcessor());
        });

        return telemetryBuilder;
    }

    private static void Map(this SentryOptions source, SentryOptions target)
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