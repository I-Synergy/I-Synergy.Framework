using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Builder;

namespace ISynergy.Framework.AspNetCore.Monitoring.Extensions;

public static class ApplicationBuilderExtensions
{
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
