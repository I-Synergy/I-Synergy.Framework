using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.Monitoring.Extensions;

public static class HostApplicationBuilderExtensions
{

    /// <summary>
    /// Adds monitoring with SignalR integration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddMonitorSignalR<TEntity>(this IHostApplicationBuilder builder)
        where TEntity : class
    {
        builder.Services.AddMonitorSignalR<TEntity>();
        return builder;
    }
}
