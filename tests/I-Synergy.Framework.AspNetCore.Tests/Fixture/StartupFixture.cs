using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.Extensions;
using ISynergy.Framework.AspNetCore.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.AspNetCore.Tests.Fixture
{
    public class StartupFixture
    {
        private IConfiguration Configuration;

        public StartupFixture(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<MaxConcurrentRequestsOptions>(Configuration.GetSection(nameof(MaxConcurrentRequestsOptions)).Bind);
        }

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
}
