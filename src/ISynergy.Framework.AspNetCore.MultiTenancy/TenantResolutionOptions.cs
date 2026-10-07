using ISynergy.Framework.Core.Constants;

namespace ISynergy.Framework.AspNetCore.MultiTenancy;

/// <summary>
/// Options that control how <see cref="Middleware.TenantResolutionMiddleware"/> resolves the current
/// tenant and user name from the authenticated principal.
/// </summary>
/// <remarks>
/// The defaults preserve the historical behaviour: the tenant is read from
/// <see cref="Claims.TenantId"/> (<c>"tenant"</c>) and the user name from <see cref="Claims.Username"/>
/// (<c>"username"</c>), and an authenticated principal that resolves to no tenant is rejected.
/// Consumers whose identity provider emits different claim names (for example Identity, which emits
/// <c>tenant_id</c> / <c>preferred_username</c>) override <see cref="TenantIdClaimType"/> and
/// <see cref="UsernameClaimType"/> when registering multi-tenancy.
/// </remarks>
public sealed class TenantResolutionOptions
{
    /// <summary>
    /// Gets or sets the claim type that carries the tenant identifier.
    /// Defaults to <see cref="Claims.TenantId"/> (<c>"tenant"</c>).
    /// </summary>
    public string TenantIdClaimType { get; set; } = Claims.TenantId;

    /// <summary>
    /// Gets or sets the claim type that carries the user name. Set to <see langword="null"/> to skip
    /// user-name resolution entirely.
    /// Defaults to <see cref="Claims.Username"/> (<c>"username"</c>).
    /// </summary>
    public string? UsernameClaimType { get; set; } = Claims.Username;

    /// <summary>
    /// Gets or sets an optional fallback claim type consulted when <see cref="TenantIdClaimType"/> is
    /// absent from the principal (for example <c>"sub"</c>). When <see langword="null"/> no fallback is used.
    /// </summary>
    public string? TenantIdFallbackClaimType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an authenticated principal that resolves to no tenant
    /// makes <see cref="Middleware.TenantResolutionMiddleware"/> throw, so the misconfiguration is visible
    /// immediately rather than silently allowing un-tenanted data access. Defaults to <see langword="true"/>.
    /// </summary>
    public bool RequireTenant { get; set; } = true;
}
