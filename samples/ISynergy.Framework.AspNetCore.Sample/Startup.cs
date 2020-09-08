using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.AspNetCore.Sample
{
    public class Startup : BaseStartup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
            : base(environment, configuration)
        {
        }

        protected override void AddMvc(IServiceCollection services, IEnumerable<string> authorizedRazorPages = null)
        {
            services.AddControllersWithViews();

            services
                .AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    //Use the default property(Pascal) casing.
                    options.UseMemberCasing();
                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                });
        }
    }
}
