using Microsoft.Extensions.Hosting;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Extensions;
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds multi tenancy integration.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="configure">
    /// Optional configuration of tenant resolution (claim names, fallback claim, whether a tenant is required).
    /// When omitted, the defaults in <see cref="TenantResolutionOptions"/> apply.
    /// </param>
    /// <returns>The host application builder.</returns>
    public static IHostApplicationBuilder AddMultiTenancyIntegration(
        this IHostApplicationBuilder builder,
        Action<TenantResolutionOptions>? configure = null)
    {
        builder.Services.AddMultiTenancyIntegration(configure);
        return builder;
    }
}
