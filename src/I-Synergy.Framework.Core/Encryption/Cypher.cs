using System.IO;
using System.Security.Cryptography;

namespace ISynergy.Framework.Core.Encryption
{
    // This class is adapted from https://www.codeproject.com/Articles/5719/Simple-encrypting-and-decrypting-data-in-C
    /// <summary>
    /// Class Cypher.
    /// </summary>
    public static class Cypher
    {
        /// <summary>
        /// The key
        /// </summary>
        public static string Key = "81C+!'dcgV`<c'XMSiR@{;+j$A6UGb}heZIcgByC,`&t&HD4d+|?=,f(2ke3wt2";

        // Encrypt a byte array into a byte array using a key and an IV 
        /// <summary>
        /// Encrypts the DES.
        /// </summary>
        /// <param name="clearData">The clear data.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The iv.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] EncryptDES(byte[] clearData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            var ms = new MemoryStream();
            var alg = TripleDES.Create();
            alg.Key = Key;
            alg.IV = IV;
            var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            var encryptedData = ms.ToArray();
            return encryptedData;
        }

        /// <summary>
        /// Encrypts the DES.
        /// </summary>
        /// <param name="clearData">The clear data.</param>
        /// <param name="Password">The password.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] EncryptDES(byte[] clearData, string Password)
        {
            var pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            return EncryptDES(clearData, pdb.GetBytes(24), pdb.GetBytes(8));
        }

        /// <summary>
        /// Decrypts the DES.
        /// </summary>
        /// <param name="cipherData">The cipher data.</param>
        /// <param name="Key">The key.</param>
        /// <param name="IV">The iv.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] DecryptDES(byte[] cipherData, byte[] Key, byte[] IV)
        {
            var ms = new MemoryStream();
            var alg = TripleDES.Create();
            alg.Key = Key;
            alg.IV = IV;
            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            var decryptedData = ms.ToArray();
            return decryptedData;
        }

        /// <summary>
        /// Decrypts the DES.
        /// </summary>
        /// <param name="cipherData">The cipher data.</param>
        /// <param name="Password">The password.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] DecryptDES(byte[] cipherData, string Password)
        {
            var pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d,
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            return DecryptDES(cipherData, pdb.GetBytes(24), pdb.GetBytes(8));
        }
    }
}
