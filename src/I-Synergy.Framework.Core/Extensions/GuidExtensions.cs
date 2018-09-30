using ISynergy.Encryption;

namespace System
{
    public static class GuidExtensions
    {
        public static uint ToUInt(this Guid guid)
        {
            string strGuid = guid.ToString().Remove("-{}");
            byte[] EncryptedBytes = strGuid.GetBytes(9, 8);
            var decrypted = Cypher.DecryptDES(EncryptedBytes, Cypher.Key);
            uint DecryptedUint = BitConverter.ToUInt32(decrypted, 0);
            return DecryptedUint;
        }
    }
}
