using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Telemetry.Options;
using ISynergy.Framework.Telemetry.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public static IServiceCollection AddTelemetryApplicationInsightsIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ApplicationInsightsOptions>(configuration.GetSection(nameof(ApplicationInsightsOptions)).BindWithReload);

//#if UAP10_0_18362 || NET6_0_WINDOWS10_0_18362_0 || NETSTANDARD2_0

//#else
//            services.AddApplicationInsightsTelemetry(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
//#endif
            services.TryAddSingleton<ITelemetryService, TelemetryService>();

            return services;
        }
    }
}
