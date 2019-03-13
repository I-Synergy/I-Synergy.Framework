using System;

namespace ISynergy.Services
{
    public interface ITenantService
    {
        Guid TenantId { get; }
        string UserName { get; }

        void SetTenant(Guid tenantId);
        void SetTenant(Guid tenantId, string username);
    }
}
