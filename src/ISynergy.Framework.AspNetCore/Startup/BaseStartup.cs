using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.AspNetCore.Routing;
using ISynergy.Framework.Core.Abstractions.Async;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ISynergy.Framework.AspNetCore.Startup
{
    /// <summary>
    /// Class BaseStartup.
    /// Implements the <see cref="IAsyncInitialization" />
    /// </summary>
    /// <seealso cref="IAsyncInitialization" />
    public abstract class BaseStartup : IAsyncInitialization
    {
        /// <summary>
        /// Gets the display name of the API.
        /// </summary>
        /// <value>The display name of the API.</value>
        protected virtual string ApiDisplayName => $"{GetType().Assembly.GetName().Name} v{GetType().Assembly.GetName().Version}";

        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>The environment.</value>
        protected IWebHostEnvironment Environment { get; }
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        protected IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStartup"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Argument.IsNotNull(nameof(environment), environment);
            Argument.IsNotNull(nameof(configuration), configuration);

            Environment = environment;
            Configuration = configuration;
        }

        /// <summary>
        /// The result of the asynchronous initialization of this instance.
        /// </summary>
        /// <value>The initialization.</value>
        public Task Initialization { get; } = new Task(() => { });

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            AddLocalization(services);
            AddOptions(services);
            AddCaching(services);
            AddDataProtectionService(services);
            AddMessageService(services);
            AddServices(services);
            AddLogging(services);
            AddCloudStorage(services);
            AddMvc(services);
            AddRouting(services);
            AddSignalR(services);
            AddTelemetry(services);
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.ConfigureExceptionHandlerMiddleware();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization(
                app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// AddCloudStorage
        /// </summary>
        /// <param name="services">The services.</param>
        /// <example>
        ///   <code>
        /// services.Configure{AzureDocumentSetting}(_configurationRoot.GetSection(nameof(AzureDocumentSetting)).BindWithReload);
        /// services.AddScoped{ICloudStorageService, CloudStorageService}();
        /// </code>
        /// </example>
        protected virtual void AddCloudStorage(IServiceCollection services)
        {
        }

        /// <summary>
        /// Adds the options.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddOptions(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<WebsiteOptions>(Configuration.GetSection(nameof(WebsiteOptions)).BindWithReload);
        }

        /// <summary>
        /// Adds the localization.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddLocalization(IServiceCollection services)
        {
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("nl")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "nl", uiCulture: "nl");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
                options.RequestCultureProviders.Insert(1, new RouteDataRequestCultureProvider());
                options.RequestCultureProviders.Insert(2, new AcceptLanguageHeaderRequestCultureProvider());
                options.RequestCultureProviders.Insert(3, new QueryStringRequestCultureProvider());
            });
        }


        /// <summary>
        /// Adds the signal r.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddSignalR(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            // Disabled due exception of UWP client while compiling natively https://github.com/aspnet/SignalR/issues/3288
            // .AddMessagePackProtocol();
        }

        /// <summary>
        /// Adds the caching.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddCaching(IServiceCollection services)
        {
            services.AddMemoryCache();
        }

        /// <summary>
        /// Adds the data protection service.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddDataProtectionService(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        /// <summary>
        /// AddMessageService
        /// </summary>
        /// <param name="services">The services.</param>
        /// <example>
        ///   <code>
        /// services.AddScoped&lt;IEmailSender, MessageService&gt;();
        /// services.AddScoped&lt;ISmsSender, MessageService&gt;();
        /// </code>
        /// </example>
        protected virtual void AddMessageService(IServiceCollection services)
        {
        }

        /// <summary>
        /// AddServices
        /// </summary>
        /// <param name="services">The services.</param>
        /// <example>
        ///   <code>
        /// services.AddScoped&lt;IFactoryService, FactoryService&gt;();
        /// </code>
        /// </example>
        protected virtual void AddServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Adds the logging.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddLogging(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton((s) => new LoggerFactory().CreateLogger(AppDomain.CurrentDomain.FriendlyName));
        }

        /// <summary>
        /// Adds the MVC.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="authorizedRazorPages">The authorized razor pages.</param>
        protected abstract void AddMvc(IServiceCollection services, IEnumerable<string> authorizedRazorPages = null);

        /// <summary>
        /// Adds the routing.
        /// </summary>
        /// <param name="services">The services.</param>
        protected virtual void AddRouting(IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                // Auto add trailing slash to url's. This is a workaround for, for example, providing emails in the url.
                // A request won't get handled if it has a period (.) in the parameters, unless there's a trailing slash at the end.
                // /api/controller/test@user.com <- DOES NOT WORK
                // /api/controller/test@user.com/ <- DOES WORK
                options.AppendTrailingSlash = true;
            });
        }

        /// <summary>
        /// AddTelemetry
        /// </summary>
        /// <param name="services">The services.</param>
        /// <example>
        ///   <code>
        /// // Configure SnapshotCollector from application settings
        /// services.Configure&lt;SnapshotCollectorConfiguration&gt;(_configuration.GetSection(nameof(SnapshotCollectorConfiguration)));
        /// // Add SnapshotCollector telemetry processor.
        /// services.AddSingleton&lt;ITelemetryInitializer, UserInfoTelemetryInitializer&gt;();
        /// services.AddSingleton&lt;ITelemetryProcessorFactory, SnapshotCollectorTelemetryProcessorFactory&gt;();
        /// </code>
        /// </example>
        protected virtual void AddTelemetry(IServiceCollection services)
        {
        }
    }
}
