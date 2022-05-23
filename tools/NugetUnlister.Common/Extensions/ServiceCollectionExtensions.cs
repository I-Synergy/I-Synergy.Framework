using ISynergy.Framework.Core.Extensions;
using NugetUnlister.Common.Abstractions;
using NugetUnlister.Common.Options;
using NugetUnlister.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Flurl.Http;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NugetUnlister.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNugetServiceIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IFlurlClient>((s) => new FlurlClient());

            FlurlHttp.Configure(c =>
            {
                c.Timeout = TimeSpan.FromSeconds(30);
            });

            services.Configure<ConfigurationOptions>(configuration.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            services.AddScoped<INugetService, NugetService>();
            return services;
        }
    }
}
