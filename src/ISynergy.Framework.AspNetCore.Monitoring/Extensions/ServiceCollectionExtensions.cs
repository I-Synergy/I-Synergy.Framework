using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using ISynergy.Framework.AspNetCore.Monitoring.Services;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Monitoring.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddMonitorSignalR<TEntity>(this IServiceCollection services, IConfiguration configuration)
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

#if NET8_0_OR_GREATER
    /// <summary>
    /// Adds monitoring with SignalR integration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddMonitorSignalR<TEntity>(this IHostApplicationBuilder builder)
        where TEntity : class
    {
        builder.Services.AddMonitorSignalR<TEntity>(builder.Configuration);
        return builder;
    }
#endif

    /// <summary>
    /// Uses monitoring with SignalR integration.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMonitorSignalR(this IApplicationBuilder app)
    {
        Argument.IsNotNull(app);
        return app.UseEndpoints(endpoints => endpoints.MapHub<MonitorHub>("/monitor"));
    }
}
