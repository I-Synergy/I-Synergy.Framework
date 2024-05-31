using ISynergy.Framework.AspNetCore.Globalization.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.Globalization.Extensions;

/// <summary>
/// Service collection extensions for globalization service
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds globalization integration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddGlobalizationIntegration(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.TryAddSingleton<ILanguageService, LanguageService>();
        return builder;
    }
}
