using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Logging.ApplicationInsights.Options;
using ISynergy.Framework.Logging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ILoggingBuilder AddApplicationInsightsLogging(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.AddApplicationInsights();
            builder.Services.Configure<ApplicationInsightsOptions>(configuration.GetSection(nameof(ApplicationInsightsOptions)).BindWithReload);
            builder.Services.AddSingleton<ILogger, Logger>();

            return builder;
        }
    }
}
