using ISynergy.Framework.Update.Abstractions.Services;
using ISynergy.Framework.Update.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Update.Extensions;

/// <summary>
/// Service collection extensions for Microsoft Stor updates
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds update integration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUpdatesIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<IUpdateService, UpdateService>();
        return services;
    }
}