using System.Collections.Generic;
using ISynergy.Framework.Core;
using ISynergy.Framework.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ISynergy.Framework.AspNetCore.Startup
{
    public abstract class BaseStartup<TDbContext> : BaseStartup, IAsyncInitialization
        where TDbContext : DbContext
    {
        protected BaseStartup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        {
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            AddDbServices(services);
        }

        /// <summary>
        /// Adds data context.
        /// </summary>
        /// <param name="services"></param>
        /// <example>
        /// services.AddDbContext<TDbContext>(options =>
        /// {
        ///    options.UseSqlServer(Configuration.GetConnectionString("ConnectionString"));
        ///    //sqlServerOptionsAction: sqlOptions =>
        ///    //{
        ///    //    // Configuring Connection Resilience
        ///    //    // Doesn't work because of transactions!!!
        ///    //    // sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        ///    //});
        /// };
        /// </example>
        protected abstract void AddDbServices(IServiceCollection services);

        protected new virtual void AddMvc(IServiceCollection services, IEnumerable<string>? authorizedRazorPages = null)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<MvcOptions>(options =>
            {
                if (!Environment.IsDevelopment())
                    options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddRazorPagesOptions(options =>
                {
                    if (authorizedRazorPages != null)
                    {
                        foreach (var page in authorizedRazorPages)
                        {
                            options.Conventions.AuthorizePage(page);
                        }
                    }
                })
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
    }
}
