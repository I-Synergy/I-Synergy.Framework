using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Monitoring.Abstractions.Services;
using ISynergy.Framework.Monitoring.Hubs;
using ISynergy.Framework.Monitoring.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Monitoring.Extensions
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
        public static IServiceCollection AddMonitorSignalR<TEntity>(this IServiceCollection services, IConfiguration configuration)
            where TEntity : class
        {
            services.AddLogging();
            services.AddRouting();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<MonitorHub>();
            services.TryAddSingleton<IMonitorService<TEntity>, MonitorService<TEntity>>();

            return services;
        }

        /// <summary>
        /// Uses monitoring with SignalR integration.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMonitorSignalR(this IApplicationBuilder app)
        {
            Argument.IsNotNull(app);
            return app.UseEndpoints(endpoints => endpoints.MapHub<MonitorHub>("/monitor"));
        }
    }
}
