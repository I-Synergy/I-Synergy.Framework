using ISynergy.Framework.Core.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace ISynergy.Framework.AspNetCore.Authentication.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid TenantId => RetrieveTenantId();
        public string UserName => RetrieveUserName();

        public void SetTenant(Guid tenantId)
        {
            var claimIdentity = new ClaimsIdentity();
            claimIdentity.AddClaim(new Claim(Core.Constants.ClaimTypes.AccountIdType, tenantId.ToString()));
            var principal = new GenericPrincipal(claimIdentity, Array.Empty<string>());
            _httpContextAccessor.HttpContext.User = principal;
        }

        public void SetTenant(Guid tenantId, string username)
        {
            var identity = new GenericIdentity(username);
            var claimIdentity = new ClaimsIdentity(identity);
            claimIdentity.AddClaim(new Claim(Core.Constants.ClaimTypes.AccountIdType, tenantId.ToString()));
            claimIdentity.AddClaim(new Claim(Core.Constants.ClaimTypes.UserNameType, username));
            var principal = new GenericPrincipal(claimIdentity, new string[] { "Client" });
            _httpContextAccessor.HttpContext.User = principal;
        }

        private Guid RetrieveTenantId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            Guid.TryParse(principal?.FindFirst(Core.Constants.ClaimTypes.AccountIdType)?.Value, out var parsedtenant);
            return parsedtenant;
        }
        private string RetrieveUserName()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            return principal?.Identity?.Name ?? string.Empty;
        }
    }
}
