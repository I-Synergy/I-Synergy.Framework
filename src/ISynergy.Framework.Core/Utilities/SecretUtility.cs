using System.Security.Cryptography;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Provides utilities for generating cryptographically random secrets.
/// </summary>
public static class SecretUtility
{
    /// <summary>
    /// Generates a cryptographically random secret suitable for use as a symmetric key,
    /// API key, or similar credential.
    /// </summary>
    /// <returns>
    /// A 64-character uppercase hexadecimal string (256 bits of entropy) derived from
    /// <see cref="RandomNumberGenerator"/>.
    /// </returns>
    /// <remarks>
    /// The hex encoding is URL-safe, contains no padding, and preserves full entropy from
    /// the 32 random bytes. The previous Base64-with-character-replacement approach silently
    /// reduced entropy by mapping non-alphanumeric characters to a fixed value.
    /// </remarks>
    public static string GenerateSecret()
        => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
}
