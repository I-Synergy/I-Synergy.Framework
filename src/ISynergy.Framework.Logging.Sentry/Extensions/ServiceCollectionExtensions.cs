using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Logging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sentry;

namespace ISynergy.Framework.Logging.Extensions;

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
    public static ILoggingBuilder AddSentryLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<SentryOptions>(configuration.GetSection(nameof(SentryOptions)).BindWithReload);
        builder.Services.RemoveAll<ILogger>();
        builder.Services.TryAddSingleton<ILogger, Logger>();
        return builder;
    }
}
