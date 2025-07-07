using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.UI.Messages;
using Microsoft.AspNetCore.Components.Authorization;
using OpenIddict.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ISynergy.Framework.UI.Providers;
public class MauiAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly IScopedContextService _scopedContextService;

    private static readonly ClaimsPrincipal _anonymousPrincipal = new(new ClaimsIdentity());
    private static readonly Task<AuthenticationState> _anonymousState = Task.FromResult(new AuthenticationState(_anonymousPrincipal));

    public MauiAuthenticationStateProvider(IScopedContextService scopedContextService)
    {
        _scopedContextService = scopedContextService;

        MessageService.Default.Register<AuthenticationChangedMessage>(this, m =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync()));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var context = _scopedContextService.GetRequiredService<IContext>();

        if (!context.IsAuthenticated || context.Profile == null || context.Profile.Token == null || string.IsNullOrEmpty(context.Profile.Token.IdToken))
            return _anonymousState;

        var principal = GetClaimsPrincipalFromJwt(context.Profile.Token.IdToken);
        return Task.FromResult(new AuthenticationState(principal));
    }

    /// <summary>
    /// Manually trigger authentication state change notification
    /// </summary>
    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    /// Clear authentication state and notify change
    /// </summary>
    public void ClearAuthenticationState()
    {
        // Clear the context authentication state
        var context = _scopedContextService.GetRequiredService<IContext>();
        context.Profile = null;

        // Notify that authentication state has changed
        NotifyAuthenticationStateChanged(_anonymousState);
    }

    private ClaimsPrincipal GetClaimsPrincipalFromJwt(string jwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);

        // Create a list to hold the mapped claims
        var mappedClaims = new List<Claim>();

        foreach (var claim in jwtSecurityToken.Claims)
        {
            // Add the original claim
            mappedClaims.Add(claim);

            // Map OpenID Connect claims to .NET ClaimTypes
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Name:
                    mappedClaims.Add(new Claim(ClaimTypes.Name, claim.Value));
                    break;
                case OpenIddictConstants.Claims.Subject:
                    mappedClaims.Add(new Claim(ClaimTypes.NameIdentifier, claim.Value));
                    break;
                case OpenIddictConstants.Claims.Email:
                    mappedClaims.Add(new Claim(ClaimTypes.Email, claim.Value));
                    break;
                case OpenIddictConstants.Claims.Role:
                    mappedClaims.Add(new Claim(ClaimTypes.Role, claim.Value));
                    break;
            }
        }

        var identity = new ClaimsIdentity(mappedClaims, OpenIddictConstants.Schemes.Bearer);
        return new ClaimsPrincipal(identity);
    }

    public void Dispose() => MessageService.Default.Unregister<AuthenticationChangedMessage>(this);
}