using Flurl.Http;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NugetUnlister.Abstractions;
using NugetUnlister.Options;
using NugetUnlister.Services;
using System;

namespace NugetUnlister.Extensions
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

            services.Configure<NugetOptions>(configuration.GetSection(nameof(NugetOptions)).BindWithReload);

            services.TryAddScoped<INugetService, NugetService>();
            return services;
        }
    }
}
