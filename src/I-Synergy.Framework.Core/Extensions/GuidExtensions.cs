using System;
using ISynergy.Framework.Core.Encryption;

namespace ISynergy.Framework.Core.Extensions
{
    public static class GuidExtensions
    {
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
