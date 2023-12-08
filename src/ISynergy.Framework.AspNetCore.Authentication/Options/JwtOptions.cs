namespace ISynergy.Framework.AspNetCore.Authentication.Options;

/// <summary>
/// Class JwtOptions.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Gets or sets the symmetric key secret.
    /// </summary>
    /// <value>The symmetric key secret.</value>
    public string SymmetricKeySecret { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the issuer.
    /// </summary>
    /// <value>The issuer.</value>
    public string Issuer { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>The audience.</value>
    public string Audience { get; set; } = string.Empty;
}
