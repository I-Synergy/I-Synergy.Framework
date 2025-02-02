using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Logging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.Logging.Extensions;
public static class ServiceCollectionExtensions
{
    private const string DockerEnvironment = "Docker";

    public static ILoggingBuilder AddOpenTelemetryLogging(
        this ILoggingBuilder builder,
        IConfiguration configuration,
        Action<OpenTelemetryLoggerOptions> optionsAction = null,
        string prefix = "")
    {
        builder.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;

            optionsAction?.Invoke(options);
        });

        builder.Services.RemoveAll<ILogger>();
        builder.Services.TryAddSingleton<IScopedContextService, ScopedContextService>();
        builder.Services.TryAddSingleton<ILogger, Logger>();

        return builder;
    }

    public static IHostBuilder ConfigureOpenTelemetryLogging(
        this IHostBuilder builder,
        IInfoService infoService,
        Action<TracerProviderBuilder> tracingAction = null,
        Action<MeterProviderBuilder> metricsAction = null,
        Action<LoggerProviderBuilder> loggerAction = null)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddServiceDiscovery();

            services.TryAddSingleton<IInfoService>(s => infoService);

            services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

            if (!context.HostingEnvironment.IsEnvironment(DockerEnvironment) && !context.HostingEnvironment.IsDevelopment())
            {
                services.Configure<ServiceDiscoveryOptions>(options =>
                {
                    options.AllowedSchemes = ["https"];
                });
            }

            services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource
                        .AddService(
                            serviceName: infoService.ProductName,
                            serviceVersion: infoService.ProductVersion.ToString())
                        .AddTelemetrySdk()
                        .AddEnvironmentVariableDetector();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddHttpClientInstrumentation();

                    // Custom tracing
                    tracingAction?.Invoke(tracing);
                })
                .WithMetrics(metrics =>
                {
                    // System metrics
                    metrics.AddRuntimeInstrumentation();    // .NET Runtime metrics (GC, CPU, etc)
                    metrics.AddProcessInstrumentation();    // Process metrics (memory, CPU, etc)

                    // HTTP Client metrics
                    metrics.AddHttpClientInstrumentation(); // Outgoing HTTP requests

                    // Custom metrics
                    metricsAction?.Invoke(metrics);
                })
                .WithLogging(logging =>
                {
                    // Custom logging
                    loggerAction?.Invoke(logging);
                });
        });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostBuilder AddOpenTelemetryExporters(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddOpenTelemetry().UseOtlpExporter();
        });


        return builder;
    }
}
