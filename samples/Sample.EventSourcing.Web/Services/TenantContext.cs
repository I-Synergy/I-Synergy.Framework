namespace Sample.EventSourcing.Web.Services;

/// <summary>
/// Scoped service that holds the active tenant ID for the current Blazor Server circuit.
/// Persists across page navigations within the same browser session.
/// </summary>
public sealed class TenantContext
{
    /// <summary>The active tenant ID. Defaults to the demo tenant.</summary>
    public Guid TenantId { get; set; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
}
