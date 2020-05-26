using System;
using ISynergy.Framework.Core.Encryption;
using ISynergy.Framework.Core.Utilities;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class IntegerExtensions.
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        /// Converts to guid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Guid.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Unsigned integer is greater than 24bit</exception>
        public static Guid ToGuid(this uint value)
        {
            if (value >= Math.Pow(2, 24))
                throw new ArgumentOutOfRangeException("Unsigned integer is greater than 24bit");

            var strGuid = Guid.NewGuid().ToString().Remove("-{}"); //Remove any '-' and '{}' characters
            var bytes = BitConverter.GetBytes(value);
            var encryptedarray = Cypher.EncryptDES(bytes, Cypher.Key);
            var EncryptedGuid = ByteUtility.WriteBytesToString(strGuid, encryptedarray, 9);
            Guid.TryParse(EncryptedGuid, out var outg);
            return outg;
        }

        /// <summary>
        /// Generates the alpha numeric key.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string GenerateAlphaNumericKey(this int self)
        {
            var vRawChars = "23456789abcdefghjkmnpqrstuwvxyzABCDEFGHJKMNPQRSTUVWXYZ";
            var vResult = new System.Text.StringBuilder();

            for (var i = 1; i <= self; i++)
            {
                vResult.Append(vRawChars.Trim().Substring(
                    Convert.ToInt32(
                            new Random().Next(int.MaxValue) * (vRawChars.Length - 1)
                            )
                        , 1)
                    );
            }

            return vResult.ToString();
        }

        /// <summary>
        /// Generates the numeric key.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>System.String.</returns>
        public static string GenerateNumericKey(this int self)
        {
            var vRawChars = "0123456789";
            var vResult = new System.Text.StringBuilder();

            for (var i = 1; i <= self; i++)
            {
                vResult.Append(vRawChars.Trim().Substring(
                    Convert.ToInt32(
                        new Random().Next(int.MaxValue) * (vRawChars.Length - 1)
                        )
                    , 1)
                );
            }

            return vResult.ToString();
        }
    }
}
