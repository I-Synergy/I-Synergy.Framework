using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Telemetry.Options;
using ISynergy.Framework.Telemetry.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Telemetry.Extensions
{
    /// <summary>
    /// Service collection extensions for AppCenter telemetry.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds AppCenter telemetry integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddTelemetryAppCenterIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppCenterOptions>(configuration.GetSection(nameof(AppCenterOptions)).BindWithReload);
            services.AddSingleton<ITelemetryService, TelemetryService>();

            return services;
        }
    }
}
