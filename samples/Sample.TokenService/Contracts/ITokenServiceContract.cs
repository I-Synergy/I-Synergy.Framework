using ISynergy.Framework.Core.Models;
using Sample.TokenService.Models;
using System.Security.Claims;

namespace Sample.TokenService.Contracts;

/// <summary>
/// Service contract to enforce implementation in business-, service- and client implementation.
/// </summary>
public interface ITokenServiceContract
{
    Task<Token> GenerateJwtTokenAsync(TokenRequest request);
    Task<WopiToken> GenerateWopiTokenAsync(WopiTokenInput input);
    Task<List<Claim>> GetClaimsAsync(Token token);
    Task<List<string>> GetClaimsAsync(Token token, string claimType);
    Task<string> GetSingleClaimAsync(Token token, string claimType);
}
