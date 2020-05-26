using System;

namespace ISynergy.Framework.Core.Services
{
    public interface ITenantService
    {
        Guid TenantId { get; }
        string UserName { get; }

        void SetTenant(Guid tenantId);
        void SetTenant(Guid tenantId, string username);
    }
}
