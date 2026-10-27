using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.UI.Extensions;

/// <summary>
/// Extension methods for adding OpenTelemetry to a WinUI application.
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry to the service collection with WinUI specific defaults.
    /// </summary>
    /// <param name="loggingBuilder"></param>
    /// <param name="builder"></param>
    /// <param name="infoService"></param>
    /// <param name="tracerProviderBuilderAction"></param>
    /// <param name="meterProviderBuilderAction"></param>
    /// <param name="loggerProviderBuilderAction"></param>
    /// <returns>The service collection for chaining.</returns>
    public static TelemetryBuilder AddTelemetry(
        this ILoggingBuilder loggingBuilder,
        IHostApplicationBuilder builder,
        IInfoService infoService,
        Action<TracerProviderBuilder>? tracerProviderBuilderAction = null,
        Action<MeterProviderBuilder>? meterProviderBuilderAction = null,
        Action<LoggerProviderBuilder>? loggerProviderBuilderAction = null)
    {
        // Create the resource builder with basic service information
        var telemetryBuilder = loggingBuilder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder
                    .AddService(
                        serviceName: infoService.ProductName,
                        serviceVersion: infoService.ProductVersion.ToString())
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector();
            });

        // Configure OpenTelemetry Logging
        telemetryBuilder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddOpenTelemetry(loggerOptions =>
            {
                // Add default console exporter in development
                if (builder.Environment.IsDevelopment())
                    loggerOptions.AddConsoleExporter();

            });
        });

        telemetryBuilder.WithTracing(tracerProviderBuilder =>
        {
            // Add instrumentation for common libraries
            tracerProviderBuilder
                .AddHttpClientInstrumentation();

            // Add default console exporter in development
            if (builder.Environment.IsDevelopment())
                tracerProviderBuilder.AddConsoleExporter();

            tracerProviderBuilderAction?.Invoke(tracerProviderBuilder);
        });

        telemetryBuilder.WithMetrics(meterProviderBuilder =>
        {
            // Add instrumentation for common libraries
            meterProviderBuilder
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            // Add default console exporter in development
            if (builder.Environment.IsDevelopment())
                meterProviderBuilder.AddConsoleExporter();

            meterProviderBuilderAction?.Invoke(meterProviderBuilder);
        });

        telemetryBuilder.WithLogging(loggerProviderBuilder =>
        {
            loggerProviderBuilderAction?.Invoke(loggerProviderBuilder);
        });

        return new TelemetryBuilder(telemetryBuilder, builder.Configuration, infoService);
    }
}
