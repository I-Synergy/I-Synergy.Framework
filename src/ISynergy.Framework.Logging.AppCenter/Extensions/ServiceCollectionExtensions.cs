using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Logging.AppCenter.Options;
using ISynergy.Framework.Logging.AppCenter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.Logging.Extensions;

/// <summary>
/// Service collection extensions for AppCenter logging.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppCenter logging integration.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddAppCenterLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<AppCenterOptions>(configuration.GetSection(nameof(AppCenterOptions)).BindWithReload);
        builder.Services.TryAddSingleton<ILogger, Logger>();

        return builder;
    }
}
