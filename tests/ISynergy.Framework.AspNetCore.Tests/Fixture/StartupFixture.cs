using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Tests.Fixture;

/// <summary>
/// Class StartupFixture.
/// </summary>
public class StartupFixture
{
    /// <summary>
    /// The configuration
    /// </summary>
    private IConfiguration Configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupFixture"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public StartupFixture(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.Configure<MaxConcurrentRequestsOptions>(Configuration.GetSection(nameof(MaxConcurrentRequestsOptions)).Bind);
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
