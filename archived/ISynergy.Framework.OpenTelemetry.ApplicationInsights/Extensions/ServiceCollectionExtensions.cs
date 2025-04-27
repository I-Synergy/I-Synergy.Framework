using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.OpenTelemetry.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ISynergy.Framework.OpenTelemetry.Extensions;
/// <summary>
/// Extension methods for setting up Application Insights in .NET applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Application Insights to the service collection directly.
    /// </summary>
    public static IServiceCollection AddApplicationInsightsTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        IInfoService infoService,
        string defaultTelemetrySection = "Telemetry",
        Action<TracerProviderBuilder>? tracingAction = null,
        Action<MeterProviderBuilder>? metricsAction = null,
        Action<LoggerProviderBuilder>? loggerAction = null)
    {
        return services.AddTelemetry<ApplicationInsightsProvider>(configuration, environment, infoService, defaultTelemetrySection, tracingAction, metricsAction, loggerAction);
    }
}
