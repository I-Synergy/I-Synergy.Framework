﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Validation;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ISynergy.Framework.AspNetCore.MultiTenancy.Services;

/// <summary>
/// Class TenantService.
/// Implements the <see cref="ITenantService" />
/// </summary>
/// <seealso cref="ITenantService" />
internal class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string[] _clientRoles = ["Client"];

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <exception cref="ArgumentNullException">httpContextAccessor</exception>
    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        Argument.IsNotNull(httpContextAccessor);

        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the tenant identifier.
    /// </summary>
    /// <value>The tenant identifier.</value>
    public Guid TenantId => RetrieveTenantId();
    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    /// <value>The name of the user.</value>
    public string UserName => RetrieveUserName();

    /// <summary>
    /// Sets the tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    public void SetTenant(Guid tenantId)
    {
        var claimIdentity = new ClaimsIdentity();
        claimIdentity.AddClaim(new Claim(Claims.KeyId, tenantId.ToString()));
        var principal = new GenericPrincipal(claimIdentity, Array.Empty<string>());
        _httpContextAccessor.HttpContext!.User = principal;
    }

    /// <summary>
    /// Sets the tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="username">The username.</param>
    public void SetTenant(Guid tenantId, string username)
    {
        var identity = new GenericIdentity(username);
        var claimIdentity = new ClaimsIdentity(identity);
        claimIdentity.AddClaim(new Claim(Claims.KeyId, tenantId.ToString()));
        claimIdentity.AddClaim(new Claim(Claims.Username, username));
        var principal = new GenericPrincipal(claimIdentity, _clientRoles);
        _httpContextAccessor.HttpContext!.User = principal;
    }

    /// <summary>
    /// Retrieves the tenant identifier.
    /// </summary>
    /// <returns>Guid.</returns>
    private Guid RetrieveTenantId()
    {
        if (Guid.TryParse(_httpContextAccessor.HttpContext!.User?.FindFirst(Claims.KeyId)?.Value, out var parsedtenant))
            return parsedtenant;

        throw new UnauthorizedAccessException("Tenant could not be retrieved.");
    }
    /// <summary>
    /// Retrieves the name of the user.
    /// </summary>
    /// <returns>System.String.</returns>
    private string RetrieveUserName() =>
        _httpContextAccessor.HttpContext!.User?.Identity?.Name ?? string.Empty;
}
