using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Telemetry.Options;
using ISynergy.Framework.Telemetry.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sentry;

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
        public static IServiceCollection AddTelemetrySentryIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SentryOptions>(configuration.GetSection(nameof(SentryOptions)).BindWithReload);
            services.TryAddSingleton<SentryOptions>(s => s.GetService<IOptions<SentryOptions>>().Value);
            services.TryAddSingleton<ISentryClient, SentryClient>();
            services.TryAddSingleton<ITelemetryService, TelemetryService>();

            return services;
        }
    }
}
