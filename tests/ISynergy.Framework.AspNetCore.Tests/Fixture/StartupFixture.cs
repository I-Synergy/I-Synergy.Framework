using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.AspNetCore.Tests.Fixture;

/// <summary>
/// Class StartupFixture.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="StartupFixture"/> class.
/// </remarks>
/// <param name="configuration">The configuration.</param>
public class StartupFixture(IConfiguration configuration)
{
    /// <summary>
    /// The configuration
    /// </summary>
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.Configure<MaxConcurrentRequestsOptions>(_configuration.GetSection(nameof(MaxConcurrentRequestsOptions)).Bind);
    }

    /// <summary>
    /// Configures the specified application.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The env.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMaxConcurrentRequests()
            .Run(async (context) =>
            {
                await Task.Delay(500);
                await context.Response.WriteAsync("-- MaxConcurrentConnections --");
            });
    }
}
