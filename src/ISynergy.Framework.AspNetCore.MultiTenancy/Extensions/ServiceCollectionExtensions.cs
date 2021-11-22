using ISynergy.Framework.AspNetCore.MultiTenancy.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Clipboard.Extensions
{
    /// <summary>
    /// Service collection extensions for multi tenancy service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds multi tenancy integration.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMultiTenancyIntegration(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITenantService, TenantService>();
            return services;
        }
    }
}
