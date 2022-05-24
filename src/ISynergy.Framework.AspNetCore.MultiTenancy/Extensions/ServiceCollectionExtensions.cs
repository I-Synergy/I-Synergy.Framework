using ISynergy.Framework.AspNetCore.MultiTenancy.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ITenantService, TenantService>();
            return services;
        }
    }
}
