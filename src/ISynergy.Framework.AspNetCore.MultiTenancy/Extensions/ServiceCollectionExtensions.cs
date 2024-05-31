using ISynergy.Framework.AspNetCore.MultiTenancy.Services;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;

/// <summary>
/// Service collection extensions for multi tenancy service
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds multi tenancy integration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddMultiTenancyIntegration(this IHostApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        builder.Services.TryAddTransient<ITenantService, TenantService>();
        return builder;
    }
}
