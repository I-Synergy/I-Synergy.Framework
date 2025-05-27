# I-Synergy Framework CQRS

## Example of CQRS implementation
```csharp
using ISynergy.Framework.CQRS.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;

namespace ISynergy.Framework.AspNetCore.Sample;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register core services
        services.AddScoped<IMessageService, MessageService>();
        
        // Add CQRS
        services.AddCQRS();
        
        // Register handlers from current assembly
        services.AddHandlers(Assembly.GetExecutingAssembly());
        
        // Add notifications after command execution
        services.AddCommandNotifications();
        
        // Add logging for command and query handlers
        services.AddCQRSLogging();
        
        // Other services
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        });
    }
}
```