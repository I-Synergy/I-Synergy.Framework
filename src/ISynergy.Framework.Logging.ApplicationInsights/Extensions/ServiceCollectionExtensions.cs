using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Logging.ApplicationInsights.Options;
using ISynergy.Framework.Logging.Initializers;
using ISynergy.Framework.Logging.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

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
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddApplicationInsightsLogging(this ILoggingBuilder builder, IConfiguration configuration, string prefix = "")
    {
        builder.Services.Configure<ApplicationInsightsOptions>(configuration.GetSection($"{prefix}{nameof(ApplicationInsightsOptions)}").BindWithReload);

        builder.Services.TryAddScoped<ITelemetryInitializer, DefaultTelemetryInitializer>();

        builder.Services.RemoveAll<ILogger>();
        builder.Services.TryAddSingleton<ILogger, Logger>();

        return builder;
    }
}
