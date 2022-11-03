using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Telemetry.ApplicationInsights.Options;
using ISynergy.Framework.Telemetry.ApplicationInsights.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace ISynergy.Framework.Telemetry.Extensions
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
            services.Configure<ApplicationInsightsOptions>(configuration.GetSection(nameof(ApplicationInsightsOptions)).BindWithReload);
            services.AddLogging(builder => builder.AddApplicationInsights());
            services.TryAddSingleton<ITelemetryService, TelemetryService>();

            return services;
        }
    }
}
