using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using ISynergy.Framework.AspNetCore.Monitoring.Services;
using ISynergy.Framework.Monitoring.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.AspNetCore.Monitoring.Extensions;

/// <summary>
/// Service collection extensions for monitoring with SignalR
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds monitoring with SignalR integration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMonitorSignalR<TEntity>(this IServiceCollection services)
        where TEntity : class
    {
        services.AddLogging();
        services.AddRouting();
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddSingleton<MonitorHub>();
        services.TryAddSingleton<IMonitorService<TEntity>, MonitorService<TEntity>>();

        return services;
    }
}
