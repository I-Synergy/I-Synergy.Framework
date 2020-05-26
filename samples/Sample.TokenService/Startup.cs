using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.TokenService.Business;
using ISynergy.Framework.Core.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using ISynergy.Framework.AspNetCore.Authentication.Options;
using ISynergy.Framework.AspNetCore.Authentication.Services;

namespace Sample.TokenService
{
    public class Startup
    {
        private readonly string ApiDisplayName = "Token Service Sample";
        private readonly IHostingEnvironment Environment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtOptions>(Configuration.GetSection(nameof(JwtOptions)).BindWithReload);

            services.AddTransient<ITokenManager, TokenManager>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.CustomSchemaIds(c => c.FullName);
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = ApiDisplayName,
                        Version = "v1"
                    });

                    var assembly = Assembly.GetEntryAssembly();

                    if (!assembly.FullName.Contains("testhost"))
                    {
                        var xmlDocPath = Path.ChangeExtension(assembly.Location, ".xml");
                        options.IncludeXmlComments(xmlDocPath);
                    }
                });
            }

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger(options =>
                {
                    // Adds host to OpenApi 2.0 document
                    options.PreSerializeFilters.Add((doc, req) => doc.Host = req.Host.Value);
                });

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", ApiDisplayName);
                    // To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string
                    // options.RoutePrefix = string.Empty;
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
