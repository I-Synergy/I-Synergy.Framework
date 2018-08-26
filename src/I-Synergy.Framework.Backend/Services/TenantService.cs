using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid TenantId => RetrieveTenantId();
        public string UserName => RetrieveUserName();

        public void SetTenant(Guid tenantId, string username) => throw new NotImplementedException();

        private Guid RetrieveTenantId()
        {
            var principal = httpContextAccessor.HttpContext.User;
            Guid.TryParse(principal?.FindFirst(ClaimTypes.AccountIdType)?.Value, out Guid parsedtenant);
            return parsedtenant;
        }
        private string RetrieveUserName()
        {
            var principal = httpContextAccessor.HttpContext.User;
            return principal?.Identity?.Name;
        }
    }
}
