using ISynergy.Framework.Core.Locators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.AspNetCore.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapDefaultHealthEndpoints(this WebApplication app)
    {
        var healthChecks = app.MapGroup("");

        healthChecks
            .CacheOutput("HealthChecks")
            .WithRequestTimeout("HealthChecks");

        // All health checks must pass for app to be
        // considered ready to accept traffic after starting
        healthChecks.MapHealthChecks("/health");

        // Only health checks tagged with the "live" tag
        // must pass for app to be considered alive
        healthChecks.MapHealthChecks("/alive", new()
        {
            Predicate = static r => r.Tags.Contains("live")
        });
        return app;
    }


    public static WebApplication SetLocatorProvider(this WebApplication app)
    {
        ServiceLocator.SetLocatorProvider(app.Services);
        return app;
    }
}