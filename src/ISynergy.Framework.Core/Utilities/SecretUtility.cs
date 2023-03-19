using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ISynergy.Framework.Core.Utilities
{
    /// <summary>
    /// Class SecretUtility.
    /// </summary>
    public static class SecretUtility
    {
        /// <summary>
        /// Generates the secret.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GenerateSecret()
        {
            const int byteLength = 66; // 64-bits, round up to get a string without padding in base64.
            var rng = RandomNumberGenerator.Create();

            var secretBytes = new byte[byteLength];
            rng.GetNonZeroBytes(secretBytes);
            return Regex.Replace(Convert.ToBase64String(secretBytes), "[^A-Za-z0-9]", "X", RegexOptions.None, TimeSpan.FromMilliseconds(100));
        }
    }
}
