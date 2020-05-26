using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using ISynergy.Framework.AspNetCore.Routing;
using ISynergy.Framework.Core;
using ISynergy.Framework.Core.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ISynergy.Framework.Core.Validation;

namespace ISynergy.Framework.AspNetCore
{
    public abstract class BaseStartup : IAsyncInitialization
    {
        protected virtual string ApiDisplayName => $"{GetType().Assembly.GetName().Name} v{GetType().Assembly.GetName().Version}";

        protected IWebHostEnvironment Environment { get; }
        protected IConfiguration Configuration { get; }

        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Argument.IsNotNull(nameof(environment), environment);
            Argument.IsNotNull(nameof(configuration), configuration);

            Environment = environment;
            Configuration = configuration;
        }

        public Task Initialization { get; } = new Task(() => { });

        // This method gets called by the runtime. Use this method to add services to the container.
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
        /// <param name="services"></param>
        /// /// <example>
        /// <code>
        /// services.Configure{AzureDocumentSetting}(_configurationRoot.GetSection(nameof(AzureDocumentSetting)).BindWithReload);
        /// services.AddScoped{ICloudStorageService, CloudStorageService}();
        /// </code>
        /// </example>
        protected virtual void AddCloudStorage(IServiceCollection services)
        {
        }

        protected virtual void AddOptions(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<WebsiteOptions>(Configuration.GetSection(nameof(WebsiteOptions)).BindWithReload);
        }

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


        protected virtual void AddSignalR(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            // Disabled due exception of UWP client while compiling natively https://github.com/aspnet/SignalR/issues/3288
            // .AddMessagePackProtocol();
        }

        protected virtual void AddCaching(IServiceCollection services)
        {
            services.AddMemoryCache();
        }

        protected virtual void AddDataProtectionService(IServiceCollection services)
        {
            services.AddDataProtection();
        }

        /// <summary>
        /// AddMessageService
        /// </summary>
        /// <param name="services"></param>
        /// <example>
        /// <code>
        /// services.AddScoped&lt;IEmailSender, MessageService>();
        /// services.AddScoped&lt;ISmsSender, MessageService>();
        /// </code>
        /// </example>
        protected virtual void AddMessageService(IServiceCollection services)
        {
        }

        /// <summary>
        /// AddServices
        /// </summary>
        /// <param name="services"></param>
        /// <example>
        /// <code>
        /// services.AddScoped&lt;IFactoryService, FactoryService>();
        /// </code>
        /// </example>
        protected virtual void AddServices(IServiceCollection services)
        {
        }

        protected virtual void AddLogging(IServiceCollection services)
        {
            services.AddLogging();
        }

        protected virtual void AddMvc(IServiceCollection services, IEnumerable<string>? authorizedRazorPages = null)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();

            services.Configure<MvcOptions>(options =>
            {
                if (!Environment.IsDevelopment())
                    options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson(options =>
                {
                    var jsonSettings = options.SerializerSettings;

                    jsonSettings.Formatting = Formatting.None;
                    jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    // Don't include null objects in Json, but only for production environments.
                    jsonSettings.NullValueHandling =
                        !Environment.IsDevelopment()
                            ? NullValueHandling.Ignore
                            : NullValueHandling.Include;

                    // Treat datetime as unspecified in Json serializer.
                    // This means that there will be no offset information (Z/+00:00) in the output Json.
                    jsonSettings.DateFormatString = GenericConstants.DateTimeOffsetFormat;
                    jsonSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    jsonSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    jsonSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                    // Serialize enums as string, instead of by their integer value.
                    var enumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();
                    jsonSettings.Converters.Add(enumConverter);
                });
        }

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
        /// <param name="services"></param>
        /// <example>
        /// <code>
        /// // Configure SnapshotCollector from application settings
        /// services.Configure&lt;SnapshotCollectorConfiguration>(_configuration.GetSection(nameof(SnapshotCollectorConfiguration)));
        /// 
        /// // Add SnapshotCollector telemetry processor.
        /// services.AddSingleton&lt;ITelemetryInitializer, UserInfoTelemetryInitializer>();
        /// services.AddSingleton&lt;ITelemetryProcessorFactory, SnapshotCollectorTelemetryProcessorFactory>();
        /// </code>
        /// </example>
        protected virtual void AddTelemetry(IServiceCollection services)
        {
        }
    }
}
