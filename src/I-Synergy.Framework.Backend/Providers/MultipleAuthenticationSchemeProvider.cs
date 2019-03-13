using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ISynergy.Providers
{
    public class MultipleAuthenticationSchemeProvider : AuthenticationSchemeProvider
    {
        protected readonly IHttpContextAccessor httpContextAccessor;

        public MultipleAuthenticationSchemeProvider(IHttpContextAccessor httpContextAccessor, IOptions<AuthenticationOptions> options)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected virtual Task<AuthenticationScheme> GetDefaultSchemeAsync()
        {
            var request = httpContextAccessor.HttpContext?.Request;

            if (request is null)
            {
                throw new ArgumentNullException("The HTTP request cannot be retrieved.");
            }

            // For API requests, use authentication tokens.
            if (request.Path.StartsWithSegments("/api") ||
                request.Path.StartsWithSegments("/monitor") ||
                request.Path.StartsWithSegments("/oauth") ||
                request.Path.StartsWithSegments("/account"))
            {
                return GetSchemeAsync(OAuthValidationDefaults.AuthenticationScheme);
            }

            // For the other requests, use authentication cookies.
            return GetSchemeAsync(IdentityConstants.ApplicationScheme);
        }

        public override Task<AuthenticationScheme> GetDefaultAuthenticateSchemeAsync() => GetDefaultSchemeAsync();

        public override Task<AuthenticationScheme> GetDefaultChallengeSchemeAsync() => GetDefaultSchemeAsync();

        public override Task<AuthenticationScheme> GetDefaultForbidSchemeAsync() => GetDefaultSchemeAsync();

        public override Task<AuthenticationScheme> GetDefaultSignInSchemeAsync() => GetDefaultSchemeAsync();

        public override Task<AuthenticationScheme> GetDefaultSignOutSchemeAsync() => GetDefaultSchemeAsync();
    }
}