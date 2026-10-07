using System.Security.Claims;
using ISynergy.Framework.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Middleware;

/// <summary>
/// Middleware that resolves the current tenant and user name from the authenticated principal's claims
/// and stores them in <see cref="TenantContext"/> for the duration of the request.
/// The claim names, an optional fallback claim, and whether a tenant is required are configured
/// through <see cref="TenantResolutionOptions"/>.
/// Must be registered after <c>UseAuthentication()</c> and <c>UseAuthorization()</c>.
/// </summary>
public class TenantResolutionMiddleware(
    RequestDelegate next,
    IOptions<TenantResolutionOptions> options,
    ILogger<TenantResolutionMiddleware> logger)
{
    private readonly TenantResolutionOptions _options = options.Value;

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
                var tenantId = ResolveTenantId(context.User);
                var userName = ResolveUserName(context.User);

                if (tenantId == Guid.Empty)
                {
                    // An authenticated user without a tenant indicates either a misconfigured token or an
                    // incorrect middleware registration order (UseAuthentication() must run before this
                    // middleware). By default fail loudly so the misconfiguration is visible immediately
                    // rather than silently allowing un-tenanted data access; a host whose provider
                    // legitimately issues tenantless authenticated principals can set RequireTenant to false.
                    if (_options.RequireTenant)
                    {
                        throw new InvalidOperationException(
                            "Tenant ID could not be resolved for an authenticated user. " +
                            $"Ensure the JWT token contains a valid '{_options.TenantIdClaimType}' claim and that " +
                            "UseAuthentication() is registered before UseTenantResolution() in the middleware pipeline.");
                    }
                }
                else
                {
                    TenantContext.Set(tenantId, userName);
                }
            }
            catch (ClaimAuthorizationException ex)
            {
                logger.LogError(ex, "Tenant resolution failed: {Message}", ex.Message);
            }
        }

        await next(context);
    }

    private Guid ResolveTenantId(ClaimsPrincipal principal)
    {
        if (TryResolveTenantId(principal, _options.TenantIdClaimType, out var tenantId))
            return tenantId;

        if (!string.IsNullOrEmpty(_options.TenantIdFallbackClaimType)
            && TryResolveTenantId(principal, _options.TenantIdFallbackClaimType, out tenantId))
            return tenantId;

        return Guid.Empty;
    }

    private string ResolveUserName(ClaimsPrincipal principal)
    {
        if (string.IsNullOrEmpty(_options.UsernameClaimType))
            return string.Empty;

        return principal.FindFirst(_options.UsernameClaimType)?.Value ?? string.Empty;
    }

    private static bool TryResolveTenantId(ClaimsPrincipal principal, string claimType, out Guid tenantId)
    {
        tenantId = Guid.Empty;
        var value = principal.FindFirst(claimType)?.Value;

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var parsed))
            return false;

        tenantId = parsed;
        return true;
    }
}
