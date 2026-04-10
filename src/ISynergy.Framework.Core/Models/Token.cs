using System.Text.Json.Serialization;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Represents an OAuth 2.0 / OpenID Connect token response.
/// </summary>
/// <remarks>
/// <strong>Security notice:</strong> All string properties of this record are sensitive bearer
/// credentials. Never log instances of this record. <see cref="ToString"/> is overridden to return
/// a redacted placeholder, preventing accidental exposure through structured logging or debugger output.
/// </remarks>
public record Token
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    /// <value>The access token.</value>
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier token (ID token for OpenID Connect flows).
    /// </summary>
    /// <value>The identifier token.</value>
    [JsonPropertyName("id_token")] public string IdToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <value>The refresh token.</value>
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of seconds until the access token expires.
    /// </summary>
    /// <value>The expires in.</value>
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the token type (typically <c>Bearer</c>).
    /// </summary>
    /// <value>The type of the token.</value>
    [JsonPropertyName("token_type")] public string TokenType { get; set; } = string.Empty;

    /// <summary>
    /// Returns a redacted string to prevent accidental token exposure through logging or debugging.
    /// </summary>
    /// <returns>A fixed string indicating this record contains redacted sensitive data.</returns>
    public override string ToString() => $"[Token: type={TokenType}, expires_in={ExpiresIn}]";
}
