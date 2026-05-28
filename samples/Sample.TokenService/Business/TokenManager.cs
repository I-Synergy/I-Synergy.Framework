using ISynergy.Framework.AspNetCore.Abstractions.Services;
using ISynergy.Framework.Core.Models;
using ISynergy.Framework.Core.Validation;
using Sample.TokenService.Models;
using System.Security.Claims;

namespace Sample.TokenService.Business;

/// <summary>
/// Token manager to be used in the business layer.
/// </summary>
public class TokenManager : ITokenManager
{
    private readonly IJwtTokenService _jwtTokenService;

    /// <summary>
    /// Constructor with dependency injection for the JwtTokenService.
    /// </summary>
    public TokenManager(IJwtTokenService tokenService)
    {
        _jwtTokenService = tokenService;
    }

    /// <summary>
    /// Generates a HmacSha256 encoded <see cref="Token"/> from a <see cref="TokenRequest"/>.
    /// </summary>
    public Task<Token> GenerateJwtTokenAsync(TokenRequest request) =>
        Task.FromResult(_jwtTokenService.GenerateJwtToken(request));

    /// <summary>
    /// Generates a <see cref="WopiToken"/> from <see cref="WopiTokenInput"/>.
    /// </summary>
    public Task<WopiToken> GenerateWopiTokenAsync(WopiTokenInput input)
    {
        Argument.IsNotNull(input);

        var claims = new List<KeyValuePair<string, string>>
        {
            new(CustomClaimTypes.ApplicationIdType, input.ApplicationId),
            new(CustomClaimTypes.DocumentIdType, input.DocumentId.ToString()),
            new(CustomClaimTypes.AccountIdType, input.TenantId.ToString()),
            new(CustomClaimTypes.UserIdType, input.UserId.ToString())
        };

        var token = _jwtTokenService.GenerateJwtToken(new TokenRequest(
            input.Username,
            claims,
            input.Roles,
            input.Expiration));

        return Task.FromResult(new WopiToken(
            input.ApplicationId,
            input.TenantId,
            input.UserId,
            input.Username,
            input.DocumentId,
            input.BrandName,
            input.BrandUrl,
            input.FriendlyName,
            input.Roles,
            input.Expiration,
            token.AccessToken));
    }

    /// <summary>
    /// Returns all claims from the given token.
    /// </summary>
    public Task<List<Claim>> GetClaimsAsync(Token token) =>
        Task.FromResult(_jwtTokenService.GetClaims(token));

    /// <summary>
    /// Returns all claims of the specified type from the given token.
    /// </summary>
    public Task<List<string>> GetClaimsAsync(Token token, string claimType) =>
        Task.FromResult(_jwtTokenService.GetClaims(token, claimType));

    /// <summary>
    /// Returns the single claim of the specified type from the given token.
    /// Throws if there is not exactly one matching claim.
    /// </summary>
    public Task<string> GetSingleClaimAsync(Token token, string claimType) =>
        Task.FromResult(_jwtTokenService.GetSingleClaim(token, claimType));
}
