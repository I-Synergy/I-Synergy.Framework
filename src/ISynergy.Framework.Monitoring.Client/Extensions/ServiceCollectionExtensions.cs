using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Monitoring.Client.Abstractions.Services;
using ISynergy.Framework.Monitoring.Common.Options;
using ISynergy.Framework.Monitoring.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Monitoring.Client.Extensions
{
    /// <summary>
    /// Service collection extensions for monitoring with SignalR
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds monitoring with SignalR integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddMonitorSignalRIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ClientMonitorOptions>(configuration.GetSection(nameof(ClientMonitorOptions)).BindWithReload);
            services.TryAddSingleton<IClientMonitorService, ClientMonitorService>();
            return services;
        }
    }
}
