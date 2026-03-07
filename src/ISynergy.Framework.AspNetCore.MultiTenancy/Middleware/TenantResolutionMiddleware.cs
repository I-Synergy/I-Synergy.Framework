using Microsoft.AspNetCore.Http;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Middleware;

/// <summary>
/// Middleware that resolves the current tenant and user from JWT claims
/// and stores them in <see cref="TenantContext"/> for the duration of the request.
/// Must be registered after <c>UseAuthentication()</c> and <c>UseAuthorization()</c>.
/// </summary>
public class TenantResolutionMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Processes an HTTP request, populating <see cref="TenantContext"/> when the user is authenticated.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst(Claims.KeyId)?.Value;
            var userName = context.User.FindFirst(Claims.Username)?.Value ?? string.Empty;

            if (Guid.TryParse(tenantIdClaim, out var tenantId))
                TenantContext.Set(tenantId, userName);
        }

        await next(context);
    }
}
