using ISynergy.Framework.AspNetCore.Startup;
using ISynergy.Framework.Monitoring.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Sample.Monitor.SignalR;

public class Startup : BaseStartup
{
    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        : base(environment, configuration)
    {
    }

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
