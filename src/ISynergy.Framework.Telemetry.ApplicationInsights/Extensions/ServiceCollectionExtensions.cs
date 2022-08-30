using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Telemetry.ApplicationInsights.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Telemetry.ApplicationInsights.Extensions
{
    /// <summary>
    /// Service collection extensions for Application Insights telemetry.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Application Insights telemetry integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationInsightsTelemetryIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder => builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", LogLevel.Debug));
            services.AddApplicationInsightsTelemetryWorkerService(configuration);
            services.TryAddSingleton<ITelemetryService, TelemetryService>();

            return services;
        }
    }
}
