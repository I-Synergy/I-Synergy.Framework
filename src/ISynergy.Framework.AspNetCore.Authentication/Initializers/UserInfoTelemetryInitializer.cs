using ISynergy.Framework.AspNetCore.Authentication.Accessors;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System;

namespace ISynergy.Framework.AspNetCore.Authentication.Initializers
{
    /// <summary>
    /// Class UserInfoTelemetryInitializer.
    /// Implements the <see cref="ITelemetryInitializer" />
    /// </summary>
    /// <seealso cref="ITelemetryInitializer" />
    public class UserInfoTelemetryInitializer : ITelemetryInitializer
    {
        /// <summary>
        /// The claims accessor
        /// </summary>
        private readonly ClaimsAccessor _claimsAccessor;
        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoTelemetryInitializer"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="claimsAccessor">The claims accessor.</param>
        /// <exception cref="ArgumentNullException">claimsAccessor</exception>
        /// <exception cref="ArgumentNullException">httpContextAccessor</exception>
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
