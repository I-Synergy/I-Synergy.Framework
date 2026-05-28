namespace ISynergy.Framework.AspNetCore.MultiTenancy;

/// <summary>
/// Ambient, async-safe store for the current tenant and user name.
/// Values flow with the async execution context, so they work correctly
/// inside HTTP requests, background workers, and parallel tasks.
/// </summary>
public static class TenantContext
{
    private static readonly AsyncLocal<Guid> _tenantId = new();
    private static readonly AsyncLocal<string?> _userName = new();

    /// <summary>
    /// Gets the current tenant identifier.
    /// Returns <see cref="Guid.Empty"/> when no tenant has been set.
    /// </summary>
    public static Guid TenantId => _tenantId.Value;

    /// <summary>
    /// Gets the current user name.
    /// Returns <see cref="string.Empty"/> when no user name has been set.
    /// </summary>
    public static string UserName => _userName.Value ?? string.Empty;

    /// <summary>
    /// Sets the tenant identifier and optional user name for the current execution context.
    /// Intended for use in middleware where the values remain valid for the duration of the request.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="userName">The user name. Defaults to <see cref="string.Empty"/>.</param>
    public static void Set(Guid tenantId, string userName = "")
    {
        _tenantId.Value = tenantId;
        _userName.Value = userName;
    }

    /// <summary>
    /// Temporarily overrides the tenant identifier and user name for the current execution context
    /// and returns a scope that restores the previous values on disposal.
    /// Intended for background workers that need to impersonate a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier to apply.</param>
    /// <param name="userName">The user name to apply. Defaults to <c>"System"</c>.</param>
    /// <returns>An <see cref="IDisposable"/> that restores the previous values when disposed.</returns>
    public static IDisposable Use(Guid tenantId, string userName = "System")
    {
        var previousTenantId = _tenantId.Value;
        var previousUserName = _userName.Value;
        _tenantId.Value = tenantId;
        _userName.Value = userName;
        return new RestoreContext(previousTenantId, previousUserName);
    }

    private sealed class RestoreContext(Guid previousTenantId, string? previousUserName) : IDisposable
    {
        /// <inheritdoc />
        public void Dispose()
        {
            _tenantId.Value = previousTenantId;
            _userName.Value = previousUserName;
        }
    }
}
