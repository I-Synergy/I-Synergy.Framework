using ISynergy.Framework.AspNetCore.Abstractions.Services;
using ISynergy.Framework.AspNetCore.Authentication.Options;
using ISynergy.Framework.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Sample.TokenService.Services;

/// <summary>
/// Sample implementation of <see cref="IJwtTokenService"/> that issues and validates
/// HMAC-SHA-256 signed JWT tokens using <see cref="JwtOptions"/> configuration.
/// </summary>
internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public Token GenerateJwtToken(TokenRequest request)
    {
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_options.SymmetricKeySecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var (type, value) in request.Claims)
            claims.Add(new Claim(type, value));

        foreach (var role in request.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(request.Expiration),
            signingCredentials: credentials);

        return new Token
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            TokenType = "Bearer",
            ExpiresIn = (int)request.Expiration.TotalSeconds
        };
    }

    /// <inheritdoc />
    public List<Claim> GetClaims(Token token)
        => [.. new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken).Claims];

    /// <inheritdoc />
    public List<string> GetClaims(Token token, string claimType)
        => GetClaims(token)
            .Where(c => c.Type == claimType)
            .Select(c => c.Value)
            .ToList();

    /// <inheritdoc />
    public string GetSingleClaim(Token token, string claimType)
        => GetClaims(token, claimType).Single();
}
