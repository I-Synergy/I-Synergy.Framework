using ISynergy.Framework.Logging.ApplicationInsights.Options;
using ISynergy.Framework.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Extensions
{
    /// <summary>
    /// Service collection extensions for Application Insights logging.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Application Insights logging integration.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddApplicationInsightsLogging(this ILoggingBuilder builder, Action<ApplicationInsightsOptions> configure)
        {
            builder.AddApplicationInsights();
            builder.Services.TryAddSingleton<ILogger, Logger>();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
