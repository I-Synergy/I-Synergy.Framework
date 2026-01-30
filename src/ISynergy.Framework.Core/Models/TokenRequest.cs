namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Class TokenRequest.
/// </summary>
public record TokenRequest
{
    /// <summary>
    /// Gets the username.
    /// </summary>
    /// <value>The username.</value>
    public string Username { get; }
    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <value>The claims.</value>
    public IEnumerable<KeyValuePair<string, string>> Claims { get; }
    /// <summary>
    /// Gets the roles.
    /// </summary>
    /// <value>The roles.</value>
    public IEnumerable<string> Roles { get; }
    /// <summary>
    /// Gets the expiration.
    /// </summary>
    /// <value>The expiration.</value>
    public TimeSpan Expiration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRequest"/> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="claims">The claims.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="expiration">The expiration.</param>
    public TokenRequest(string username,
      IEnumerable<KeyValuePair<string, string>> claims,
      IEnumerable<string> roles,
      TimeSpan expiration)
    {
        Username = username;
        Claims = claims;
        Roles = roles;
        Expiration = expiration;
    }
}
