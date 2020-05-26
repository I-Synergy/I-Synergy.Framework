using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Wopi.Options;
using ISynergy.Framework.Wopi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Wopi.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static void AddWopiFramework(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddMemoryCache();
            services.AddHttpClient(Constants.HttpClientDefault);

            services.Configure<WopiOptions>(configuration.GetSection(nameof(WopiOptions)).BindWithReload);

            services.AddSingleton<IWopiDiscoveryService, WopiDiscoveryService>();
            services.AddSingleton<IWopiValidationService, WopiValidationService>();
        }
    }
}
