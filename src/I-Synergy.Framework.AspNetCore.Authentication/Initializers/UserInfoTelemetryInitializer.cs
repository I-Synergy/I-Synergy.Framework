using ISynergy.Framework.AspNetCore.Authentication.Accessors;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Initializers
{
    public class UserInfoTelemetryInitializer : ITelemetryInitializer
    {
        private readonly ClaimsAccessor _claimsAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoTelemetryInitializer(IHttpContextAccessor httpContextAccessor, ClaimsAccessor claimsAccessor)
        {
            _claimsAccessor = claimsAccessor ?? throw new ArgumentNullException(nameof(claimsAccessor));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public void Initialize(ITelemetry telemetry)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User is null) return;

            var userName = httpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(userName)) telemetry.Context.User.AuthenticatedUserId = userName;

            if (httpContext.User.Identity.IsAuthenticated)
                telemetry.Context.User.AccountId = _claimsAccessor.GetAccountId().ToString();
        }
    }
}
