using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Sample.Synchronization.Server.Extensions;
using Sample.Synchronization.Server.Hubs;

namespace Sample.Synchronization.Server
{
    public class Startup
    {
        private const string _signalR = "signalr";

        private string ApiDisplayName => $"{GetType().Assembly.GetName().Name} v{GetType().Assembly.GetName().Version}";

        /// <summary>
        /// Configuration property.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions();

            var logger = new LoggerFactory().CreateLogger(nameof(ServiceCollectionExtensions));

            services.AddSyncService(_configuration, logger);

            /// Register SignalR service.
            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            });

            /// Add MVC
            services.AddControllersWithViews();
            services.AddMvc(options => {
                // Disable output of 204 status code (no-content) for null values
                options.OutputFormatters.RemoveType(typeof(HttpNoContentOutputFormatter));
                options.OutputFormatters.Insert(0, new HttpNoContentOutputFormatter { TreatNullValueAsNoContent = false });
            })
                .AddNewtonsoftJson(options => {
                    var jsonSettings = options.SerializerSettings;

                    jsonSettings.Formatting = Formatting.None;
                    jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    jsonSettings.MaxDepth = 5;
                    jsonSettings.NullValueHandling = NullValueHandling.Include;

                    // Serialize enums as string, instead of by their integer value.
                    var enumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();
                    jsonSettings.Converters.Add(enumConverter);
                });

            /// Add health checks
            /// - SignalR
            /// - Database
            services.AddHealthChecks()
                .AddCheck(_signalR, () => HealthCheckResult.Healthy());

            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = ApiDisplayName,
                    Version = "v1",
                    Description = "Sample Synchronization Service",
                    Contact = new OpenApiContact
                    {
                        Name = "Sample Support",
                        Email = "support@sample.com",
                        Url = new Uri("https://www.sample.com")
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/", new HealthCheckOptions()
            {
                Predicate = p => p.Tags.Contains(_signalR)
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSession();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints => {
                endpoints.MapHub<SynchronizationHub>("/synchronization");
                endpoints.MapControllers();
            });
        }
    }
}
