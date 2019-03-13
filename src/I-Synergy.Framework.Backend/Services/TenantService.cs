using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Security.Principal;

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

        public void SetTenant(Guid tenantId)
        {
            ClaimsIdentity claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(ClaimTypes.AccountIdType, tenantId.ToString()));
            GenericPrincipal principal = new GenericPrincipal(claimIdentity, Array.Empty<string>());
            httpContextAccessor.HttpContext.User = principal;
        }

        public void SetTenant(Guid tenantId, string username)
        {
            GenericIdentity identity = new GenericIdentity(username);
            ClaimsIdentity claimIdentity = new ClaimsIdentity(identity);
            claimIdentity.AddClaim(new Claim(ClaimTypes.AccountIdType, tenantId.ToString()));
            claimIdentity.AddClaim(new Claim(ClaimTypes.UserNameType, username));
            GenericPrincipal principal = new GenericPrincipal(claimIdentity, new string[] { "Client" });
            httpContextAccessor.HttpContext.User = principal;
        }

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
