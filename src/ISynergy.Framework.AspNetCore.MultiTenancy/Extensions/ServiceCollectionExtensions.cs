using ISynergy.Framework.AspNetCore.MultiTenancy.Middleware;
using ISynergy.Framework.AspNetCore.MultiTenancy.Services;
using ISynergy.Framework.Core.Abstractions.Services;
using Microsoft.AspNetCore.Builder;
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
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMultiTenancyIntegration(this IServiceCollection services)
    {
        services.TryAddTransient<ITenantService, TenantService>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="TenantResolutionMiddleware"/> to the application pipeline.
    /// Must be called after <c>UseAuthentication()</c> and <c>UseAuthorization()</c>.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseMultiTenancyMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantResolutionMiddleware>();
}
