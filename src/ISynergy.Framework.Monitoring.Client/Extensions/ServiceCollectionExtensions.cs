using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Monitoring.Client.Abstractions.Services;
using ISynergy.Framework.Monitoring.Client.Services;
using ISynergy.Framework.Monitoring.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.Monitoring.Client.Extensions;

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
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static IServiceCollection AddMonitorSignalRIntegration(this IServiceCollection services, IConfiguration configuration, string prefix = "")
    {
        services.AddOptions();
        services.Configure<ClientMonitorOptions>(configuration.GetSection($"{prefix}{nameof(ClientMonitorOptions)}").BindWithReload);
        services.TryAddSingleton<IClientMonitorService, ClientMonitorService>();
        return services;
    }
}
