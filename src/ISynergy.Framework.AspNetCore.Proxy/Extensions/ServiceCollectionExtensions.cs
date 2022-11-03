using ISynergy.Framework.AspNetCore.Proxy.Middleware;
using ISynergy.Framework.AspNetCore.Proxy.Options;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.AspNetCore.Proxy.Extensions
{
    /// <summary>
    /// Service collection extensions for proxy service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Proxy integration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddProxyIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddHttpClient();

            services.Configure<GatewayProxyOptions>(configuration.GetSection(nameof(GatewayProxyOptions)).BindWithReload);

            return services;
        }

        /// <summary>
        /// Uses proxy integration.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseProxy(this IApplicationBuilder app)
        {
            Argument.IsNotNull(app);
            return app.UseMiddleware<GatewayProxyMiddleware>();
        }
    }
}
