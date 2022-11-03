using ISynergy.Framework.AspNetCore.Globalization.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions
{
    /// <summary>
    /// Service collection extensions for globalization service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds globalization integration.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGlobalizationIntegration(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ILanguageService, LanguageService>();
            return services;
        }
    }
}
