using ISynergy.Framework.Core.Abstractions.Services;

namespace ISynergy.Framework.EventSourcing.EntityFramework.TestDoubles;

/// <summary>Simple in-memory ITenantService for tests.</summary>
internal sealed class TestTenantService : ITenantService
{
    public Guid TenantId { get; private set; } = Guid.NewGuid();
    public string UserName { get; private set; } = "test-user";

    public void SetTenant(Guid tenantId) => TenantId = tenantId;
    public void SetTenant(Guid tenantId, string username)
    {
        TenantId = tenantId;
        UserName = username;
    }
}
