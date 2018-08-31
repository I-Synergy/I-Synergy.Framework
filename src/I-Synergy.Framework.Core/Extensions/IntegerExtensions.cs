using ISynergy.Encryption;
using ISynergy.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ISynergy.Extensions
{
    public static class IntegerExtensions
    {
        public static Guid ToGuid(this uint value)
        {
            if (value >= Math.Pow(2, 24))
                throw new ArgumentOutOfRangeException("Unsigned integer is greater than 24bit");

            string strGuid = Guid.NewGuid().ToString().Remove("-{}"); //Remove any '-' and '{}' characters
            byte[] bytes = BitConverter.GetBytes(value);
            var encryptedarray = Cypher.EncryptDES(bytes, Cypher.Key);
            string EncryptedGuid = ByteUtility.WriteBytesToString(strGuid, encryptedarray, 9);
            Guid.TryParse(EncryptedGuid, out Guid outg);
            return outg;
        }
    }
}
