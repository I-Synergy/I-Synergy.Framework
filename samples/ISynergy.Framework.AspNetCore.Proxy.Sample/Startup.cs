using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.AspNetCore.Proxy.Sample.Middleware;
using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.Proxy.Sample
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup : BaseStartup
    {
        /// <summary>
        /// Display name for service.
        /// </summary>
        protected override string ApiDisplayName => $"{GetType().Assembly.GetName().Name} v{GetType().Assembly.GetName().Version}";

        /// <summary>
        /// Constructor for Startup class
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configuration"></param>
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        {
        }

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

            services.Configure<GatewayProxyOptions>(Configuration.GetSection(nameof(GatewayProxyOptions)).BindWithReload);

            services.AddHttpClient();
            services.AddLogging();
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
                foreach (var header in context.Request.Headers)
                {
                    Console.WriteLine($"Header: {header.Key}: {header.Value}");
                }

                // Connection: RemoteIp
                Console.WriteLine($"Request RemoteIp: {context.Connection.RemoteIpAddress}");

                await next();
            });

            app.UseHttpsRedirection();
            app.UseMiddleware<GatewayProxyMiddleware>();
        }

        protected override void AddMvc(IServiceCollection services, IEnumerable<string> authorizedRazorPages = null)
        {
        }
    }
}
