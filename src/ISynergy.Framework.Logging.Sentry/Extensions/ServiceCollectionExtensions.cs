using ISynergy.Framework.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sentry;

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
        public static ILoggingBuilder AddApplicationInsightsLogging(this ILoggingBuilder builder, Action<SentryOptions> configure)
        {
            builder.Services.TryAddSingleton<ILogger, Logger>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
