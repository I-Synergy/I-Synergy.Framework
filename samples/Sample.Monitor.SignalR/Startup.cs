using ISynergy.Framework.AspNetCore.Startup;
using ISynergy.Framework.Monitoring.Extensions;

namespace Sample.Monitor.SignalR;

public class Startup(IWebHostEnvironment environment, IConfiguration configuration) : BaseStartup(environment, configuration)
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddMonitorSignalR<object>(Configuration);
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMonitorSignalR();
    }

    protected override void AddMvc(IServiceCollection services, IEnumerable<string> authorizedRazorPages = null)
    {
    }
}
