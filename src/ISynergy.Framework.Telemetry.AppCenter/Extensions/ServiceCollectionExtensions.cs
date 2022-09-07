using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Telemetry.AppCenter.Options;
using ISynergy.Framework.Telemetry.AppCenter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public static IServiceCollection AddAppCenterTelemetryIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppCenterOptions>(configuration.GetSection(nameof(AppCenterOptions)).BindWithReload);
            services.TryAddSingleton<ITelemetryService, TelemetryService>();

            return services;
        }
    }
}
