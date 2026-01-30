using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.OpenTelemetry.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.AspNetCore.Extensions;
/// <summary>
/// Extension methods for adding OpenTelemetry to an ASP.NET Core application.
/// </summary>
public static class TelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry to the service collection with ASP.NET Core specific defaults.
    /// </summary>
    /// <param name="loggingBuilder">The service collection to add to.</param>
    /// <param name="hostBuilderContext"></param>
    /// <param name="infoService"></param>
    /// <param name="tracerProviderBuilderAction"></param>
    /// <param name="meterProviderBuilderAction"></param>
    /// <param name="loggerProviderBuilderAction"></param>
    /// <returns>The service collection for chaining.</returns>
    public static TelemetryBuilder AddTelemetry(
        this ILoggingBuilder loggingBuilder,
        IHostApplicationBuilder hostBuilderContext,
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
            loggingBuilder.SetMinimumLevel(hostBuilderContext.Configuration.GetDefaultLogLevel());

            loggingBuilder.AddOpenTelemetry(loggerOptions =>
            {
                // Add default console exporter in development
                if (hostBuilderContext.Environment.IsDevelopment())
                    loggerOptions.AddConsoleExporter();

                //loggerOptions.AddProcessor(new UserContextEnrichingLogProcessor());
            });
        });

        telemetryBuilder.WithTracing(tracerProviderBuilder =>
        {
            // Add instrumentation for common libraries
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            // Add default console exporter in development
            if (hostBuilderContext.Environment.IsDevelopment())
                tracerProviderBuilder.AddConsoleExporter();

            tracerProviderBuilderAction?.Invoke(tracerProviderBuilder);
        });

        telemetryBuilder.WithMetrics(meterProviderBuilder =>
        {
            // Add instrumentation for common libraries
            meterProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            // Add default console exporter in development
            if (hostBuilderContext.Environment.IsDevelopment())
                meterProviderBuilder.AddConsoleExporter();

            meterProviderBuilderAction?.Invoke(meterProviderBuilder);
        });

        telemetryBuilder.WithLogging(loggerProviderBuilder =>
        {
            loggerProviderBuilderAction?.Invoke(loggerProviderBuilder);
        });

        return new TelemetryBuilder(telemetryBuilder, hostBuilderContext.Configuration, infoService);
    }
}