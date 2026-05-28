using ISynergy.Framework.Core.Exceptions;
using ISynergy.Framework.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Middleware;

/// <summary>
/// Middleware that resolves the current tenant and user from JWT claims
/// and stores them in <see cref="TenantContext"/> for the duration of the request.
/// Must be registered after <c>UseAuthentication()</c> and <c>UseAuthorization()</c>.
/// </summary>
public class TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
{
    /// <summary>
    /// Processes an HTTP request, populating <see cref="TenantContext"/> when the user is authenticated.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var tenantId = context.User.GetTenantId();
                var userName = context.User.GetUsername() ?? string.Empty;

                if (tenantId == Guid.Empty)
                {
                    // An authenticated user without a TenantId claim indicates either a misconfigured
                    // token or an incorrect middleware registration order (UseAuthentication() must be
                    // called before this middleware). Fail loudly so the misconfiguration is visible
                    // immediately rather than silently allowing un-tenanted data access.
                    throw new InvalidOperationException(
                        "Tenant ID could not be resolved for an authenticated user. " +
                        "Ensure the JWT token contains a valid tenant claim and that " +
                        "UseAuthentication() is registered before UseTenantResolution() in the middleware pipeline.");
                }

                if (!string.IsNullOrEmpty(userName))
                    TenantContext.Set(tenantId, userName);
            }
            catch (ClaimAuthorizationException ex)
            {
                logger.LogError(ex, "Tenant resolution failed: {Message}", ex.Message);
            }
        }

        await next(context);
    }
}
