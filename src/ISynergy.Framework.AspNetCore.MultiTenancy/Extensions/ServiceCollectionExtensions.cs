using ISynergy.Framework.AspNetCore.MultiTenancy.Services;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;

/// <summary>
/// Service collection extensions for multi tenancy service
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds multi tenancy integration.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMultiTenancyIntegration(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.TryAddTransient<ITenantService, TenantService>();
        return services;
    }
}
