using ISynergy.Framework.AspNetCore.Globalization.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Clipboard.Extensions
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILanguageService, LanguageService>();
            return services;
        }
    }
}
