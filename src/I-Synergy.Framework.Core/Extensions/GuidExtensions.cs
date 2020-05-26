using System;
using ISynergy.Framework.Core.Encryption;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class GuidExtensions.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Converts to uint.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>System.UInt32.</returns>
        public static uint ToUInt(this Guid guid)
        {
            var strGuid = guid.ToString().Remove("-{}");
            var EncryptedBytes = strGuid.GetBytes(9, 8);
            var decrypted = Cypher.DecryptDES(EncryptedBytes, Cypher.Key);
            var DecryptedUint = BitConverter.ToUInt32(decrypted, 0);
            return DecryptedUint;
        }
    }
}
