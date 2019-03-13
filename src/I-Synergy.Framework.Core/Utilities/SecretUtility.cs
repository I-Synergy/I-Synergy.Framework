using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ISynergy.Utilities
{
    public static class SecretUtility
    {
        public static string GenerateSecret()
        {
            const int byteLength = 66; // 64-bits, round up to get a string without padding in base64.
            var rng = RandomNumberGenerator.Create();

            var secretBytes = new byte[byteLength];
            rng.GetNonZeroBytes(secretBytes);
            return Regex.Replace(Convert.ToBase64String(secretBytes), "[^A-Za-z0-9]", "X");
        }
    }
}
