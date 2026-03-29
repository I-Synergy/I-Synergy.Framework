using ISynergy.Framework.Core.Abstractions.Services;

namespace Sample.EventSourcing.Api.Infrastructure;

/// <summary>
/// Simple tenant service for the demo that stores tenant/user in the ambient async context.
/// In a real application, register <c>ITenantService</c> via
/// <c>services.AddMultiTenancyIntegration()</c> and resolve the tenant from JWT claims.
/// </summary>
internal sealed class DemoTenantService : ITenantService
{
    // AsyncLocal ensures values flow through the async call chain of each request
    // without leaking between concurrent requests.
    private static readonly AsyncLocal<Guid> s_tenantId = new();
    private static readonly AsyncLocal<string?> s_userName = new();

    public Guid TenantId =>
        s_tenantId.Value == Guid.Empty
            ? Guid.Parse("11111111-1111-1111-1111-111111111111")  // default demo tenant
            : s_tenantId.Value;

    public string UserName => s_userName.Value ?? "demo-user";

    public void SetTenant(Guid tenantId) => s_tenantId.Value = tenantId;

    public void SetTenant(Guid tenantId, string username)
    {
        s_tenantId.Value = tenantId;
        s_userName.Value = username;
    }
}
