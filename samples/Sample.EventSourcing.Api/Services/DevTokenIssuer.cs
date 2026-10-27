using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Sample.EventSourcing.Api.Services;

/// <summary>
/// Development-only helper that mints signed JWTs for local testing.
/// </summary>
/// <remarks>
/// This is a local stand-in for an identity provider: it signs tokens with the same
/// <c>Jwt:SymmetricKeySecret</c> the API validates against, so a token it issues is accepted by the
/// API's JwtBearer authentication. It is registered and exposed only in Development builds.
/// </remarks>
public sealed class DevTokenIssuer
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;

    /// <summary>Initializes the issuer from JWT settings in <paramref name="configuration"/>.</summary>
    /// <param name="configuration">Configuration providing <c>Jwt:Issuer</c>, <c>Jwt:Audience</c> and <c>Jwt:SymmetricKeySecret</c>.</param>
    /// <exception cref="InvalidOperationException">Thrown when <c>Jwt:SymmetricKeySecret</c> is not configured.</exception>
    public DevTokenIssuer(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"] ?? "i-synergy-sample";
        _audience = configuration["Jwt:Audience"] ?? "i-synergy-sample-api";

        var secret = configuration["Jwt:SymmetricKeySecret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "Jwt:SymmetricKeySecret is not configured. Set 'Jwt:SymmetricKeySecret' before " +
                "issuing development tokens.");
        }

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }

    /// <summary>
    /// Creates a signed JWT for the given tenant and user.
    /// </summary>
    /// <param name="tenantId">Tenant identifier written to the <c>tenant</c> claim.</param>
    /// <param name="username">User name written to the <c>username</c> and <c>sub</c> claims. Defaults to <c>demo-user</c>.</param>
    /// <param name="lifetime">How long the token remains valid. Defaults to one hour when <see cref="TimeSpan.Zero"/> or negative.</param>
    /// <returns>The serialized, signed JWT.</returns>
    public string CreateToken(Guid tenantId, string? username, TimeSpan lifetime)
    {
        if (lifetime <= TimeSpan.Zero)
        {
            lifetime = TimeSpan.FromHours(1);
        }

        var name = string.IsNullOrWhiteSpace(username) ? "demo-user" : username;

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                ["tenant"] = tenantId.ToString(),
                ["username"] = name,
                ["sub"] = name
            }
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
