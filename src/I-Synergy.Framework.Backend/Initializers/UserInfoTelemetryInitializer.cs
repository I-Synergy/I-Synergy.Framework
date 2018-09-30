using ISynergy.Accessors;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System;

namespace ISynergy.Initializers
{
    public class UserInfoTelemetryInitializer : ITelemetryInitializer
    {
        private readonly ClaimsAccessor claimsAccessor;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserInfoTelemetryInitializer(IHttpContextAccessor httpContextAccessor, ClaimsAccessor claimsAccessor)
        {
            this.claimsAccessor = claimsAccessor ?? throw new ArgumentNullException(nameof(claimsAccessor));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public void Initialize(ITelemetry telemetry)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User is null) return;

            var userName = httpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(userName)) telemetry.Context.User.AuthenticatedUserId = userName;

            if (httpContext.User.Identity.IsAuthenticated)
                telemetry.Context.User.AccountId = claimsAccessor.GetAccountId().ToString();
        }
    }
}
