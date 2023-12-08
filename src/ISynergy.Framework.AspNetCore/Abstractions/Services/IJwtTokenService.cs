using ISynergy.Framework.Core.Models;
using System.Security.Claims;

namespace ISynergy.Framework.AspNetCore.Abstractions.Services;

/// <summary>
/// Interface IJwtTokenService
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates the JWT token.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Token.</returns>
    Token GenerateJwtToken(TokenRequest request);
    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>List&lt;Claim&gt;.</returns>
    List<Claim> GetClaims(Token token);
    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>List&lt;System.String&gt;.</returns>
    List<string> GetClaims(Token token, string claimType);
    /// <summary>
    /// Gets the single claim.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="claimType">Type of the claim.</param>
    /// <returns>System.String.</returns>
    string GetSingleClaim(Token token, string claimType);
}
