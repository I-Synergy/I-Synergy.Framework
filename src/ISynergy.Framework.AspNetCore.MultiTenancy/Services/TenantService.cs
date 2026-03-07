using ISynergy.Framework.Core.Abstractions.Services;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Services;

/// <summary>
/// Class TenantService.
/// Implements the <see cref="ITenantService" />
/// </summary>
/// <remarks>
/// Reads from and writes to the ambient <see cref="TenantContext"/>, which is
/// populated per-request by <see cref="Middleware.TenantResolutionMiddleware"/>
/// and can be overridden in background workers via <see cref="TenantContext.Use"/>.
/// </remarks>
/// <seealso cref="ITenantService" />
internal class TenantService : ITenantService
{
    /// <summary>
    /// Gets the tenant identifier from the current <see cref="TenantContext"/>.
    /// </summary>
    /// <value>The tenant identifier.</value>
    public Guid TenantId => TenantContext.TenantId;

    /// <summary>
    /// Gets the name of the user from the current <see cref="TenantContext"/>.
    /// </summary>
    /// <value>The name of the user.</value>
    public string UserName => TenantContext.UserName;

    /// <summary>
    /// Sets the tenant identifier in the current <see cref="TenantContext"/>.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    public void SetTenant(Guid tenantId) =>
        TenantContext.Set(tenantId);

    /// <summary>
    /// Sets the tenant identifier and user name in the current <see cref="TenantContext"/>.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="username">The username.</param>
    public void SetTenant(Guid tenantId, string username) =>
        TenantContext.Set(tenantId, username);
}
