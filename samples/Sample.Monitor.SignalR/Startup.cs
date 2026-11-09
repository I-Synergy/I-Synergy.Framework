using ISynergy.Framework.AspNetCore.Monitoring.Extensions;

namespace Sample.Monitor.SignalR;

public class Startup()
{
    // S2325: these are ASP.NET Core Startup convention methods. The host instantiates this class via
    // UseStartup<Startup>() and invokes them on the instance. Making them static would leave the type
    // with only static members, which (combined with UseStartup's required public constructor) trips
    // S1118, and a static class cannot be supplied as the generic argument to UseStartup<TStartup>.
#pragma warning disable S2325 // ASP.NET Core Startup convention method invoked on the instance
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
#pragma warning restore S2325
    {
        // Override if you want to use custom implementation.
        services.AddMonitorSignalR<object>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable S2325 // ASP.NET Core Startup convention method invoked on the instance
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore S2325
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseMonitorSignalR();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        });
    }
}
