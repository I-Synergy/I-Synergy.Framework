using ISynergy.Framework.Clipboard.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Clipboard.Extensions
{
    /// <summary>
    /// Service collection extensions for clipboard service
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds clipboard integration.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClipboardIntegration(this IServiceCollection services)
        {
            services.TryAddSingleton<IClipboardService, ClipboardService>();
            return services;
        }
    }
}
