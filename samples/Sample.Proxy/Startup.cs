using ISynergy.Framework.AspNetCore.Proxy.Extensions;
using ISynergy.Framework.AspNetCore.Startup;

namespace Sample.Proxy;

/// <summary>
/// Startup class.
/// </summary>
/// <remarks>
/// Constructor for Startup class
/// </remarks>
/// <param name="environment"></param>
/// <param name="configuration"></param>
public class Startup(IWebHostEnvironment environment, IConfiguration configuration) : BaseStartup(environment, configuration)
{
    /// <summary>
    /// Display name for service.
    /// </summary>
    protected override string ApiDisplayName => $"{GetType().Assembly.GetName().Name} v{GetType().Assembly.GetName().Version}";

    /// <summary>
    /// Adds DataProtection.
    /// </summary>
    /// <param name="services"></param>
    protected override void AddDataProtectionService(IServiceCollection services)
    {
        if (Environment.IsDevelopment())
        {
            services.AddDataProtection(options => options.ApplicationDiscriminator = "ApiClient");
        }
    }

    private const string CorsOrigins = "CorsOrigins";

    /// <summary>
    /// Configures required services.
    /// </summary>
    /// <param name="services"></param>
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOrigins, builder =>
            {
                builder.WithOrigins("https://localhost:5000", "https://localhost:5003", "https://localhost:5006")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddProxyIntegration(Configuration);
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseCors(CorsOrigins);
        app.Use(async (context, next) =>
        {
            // Request method, scheme, and path
            Console.WriteLine($"Request Method: {context.Request.Method}");
            Console.WriteLine($"Request Scheme: {context.Request.Scheme}");
            Console.WriteLine($"Request Path: {context.Request.Path}");

            // Headers
            foreach (System.Collections.Generic.KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> header in context.Request.Headers)
            {
                Console.WriteLine($"Header: {header.Key}: {header.Value}");
            }

            // Connection: RemoteIp
            Console.WriteLine($"Request RemoteIp: {context.Connection.RemoteIpAddress}");

            await next();
        });

        app.UseHttpsRedirection();
        app.UseProxy();
    }

    protected override void AddMvc(IServiceCollection services, System.Collections.Generic.IEnumerable<string> authorizedRazorPages = null)
    {
    }
}
