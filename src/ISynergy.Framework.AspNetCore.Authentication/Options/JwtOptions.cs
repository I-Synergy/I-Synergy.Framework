using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.Authentication.Options;

/// <summary>
/// Configuration options for JWT token generation and validation.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Security notice:</strong> <see cref="SymmetricKeySecret"/> must be a cryptographically
/// random string of at least 32 characters (256 bits) when using HMAC-SHA256.
/// Never store this value in plain-text configuration files; source it from a Key Vault service
/// or a platform-provided secret (environment variable, Kubernetes secret, managed identity).
/// </para>
/// </remarks>
public class JwtOptions
{
    /// <summary>
    /// The minimum number of characters required for the <see cref="SymmetricKeySecret"/>.
    /// Enforces a minimum of 256 bits of key material for HMAC-SHA256 signing.
    /// </summary>
    public const int MinimumKeyLength = 32;

    /// <summary>
    /// Gets or sets the symmetric key secret used to sign and verify JWT tokens.
    /// </summary>
    /// <value>The symmetric key secret. Must be at least <see cref="MinimumKeyLength"/> characters.</value>
    /// <remarks>
    /// <strong>Security notice:</strong> This is a sensitive credential. Source it exclusively from
    /// a Key Vault service or environment variable — never from <c>appsettings.json</c>.
    /// </remarks>
    public string SymmetricKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issuer claim (<c>iss</c>) included in generated tokens and validated on incoming tokens.
    /// </summary>
    /// <value>The issuer.</value>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience claim (<c>aud</c>) included in generated tokens and validated on incoming tokens.
    /// </summary>
    /// <value>The audience.</value>
    public string Audience { get; set; } = string.Empty;
}

/// <summary>
/// Validates <see cref="JwtOptions"/> at application startup to catch misconfigurations early.
/// Register with <c>services.AddSingleton&lt;IValidateOptions&lt;JwtOptions&gt;, JwtOptionsValidator&gt;()</c>.
/// </summary>
public sealed class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.SymmetricKeySecret))
            return ValidateOptionsResult.Fail(
                $"{nameof(JwtOptions)}.{nameof(JwtOptions.SymmetricKeySecret)} must not be null or empty.");

        if (options.SymmetricKeySecret.Length < JwtOptions.MinimumKeyLength)
            return ValidateOptionsResult.Fail(
                $"{nameof(JwtOptions)}.{nameof(JwtOptions.SymmetricKeySecret)} must be at least {JwtOptions.MinimumKeyLength} characters (256-bit key material for HMAC-SHA256).");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            return ValidateOptionsResult.Fail(
                $"{nameof(JwtOptions)}.{nameof(JwtOptions.Issuer)} must not be null or empty.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            return ValidateOptionsResult.Fail(
                $"{nameof(JwtOptions)}.{nameof(JwtOptions.Audience)} must not be null or empty.");

        return ValidateOptionsResult.Success;
    }
}
