using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Utilities;

namespace ISynergy.Framework.Core.Encryption
{
    /// <summary>
    /// Generic cryptography class.
    /// </summary>
    public class Cryptography
    {
        //AesManaged
        //        Legal min key size = 128
        //        Legal max key size = 256
        //        Legal min block size = 128
        //        Legal max block size = 128
        //DESCryptoServiceProvider
        //        Legal min key size = 64
        //        Legal max key size = 64
        //        Legal min block size = 64
        //        Legal max block size = 64
        //RC2CryptoServiceProvider
        //        Legal min key size = 40
        //        Legal max key size = 128
        //        Legal min block size = 64
        //        Legal max block size = 64
        //RijndaelManaged
        //        Legal min key size = 128
        //        Legal max key size = 256
        //        Legal min block size = 128
        //        Legal max block size = 256
        //TripleDESCryptoServiceProvider
        //        Legal min key size = 128
        //        Legal max key size = 192
        //        Legal min block size = 64
        //        Legal max block size = 64

        private readonly int _keyBitSize;
        private readonly int _iterations;
        private readonly string _hash;
        private readonly byte[] _saltBytes; // Random aselrias38490a32
        private readonly byte[] _vectorBytes; // Random 8947az34awl34kjq

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="salt"></param>
        /// <param name="vector"></param>
        /// <param name="keySize"></param>
        /// <param name="hash"></param>
        /// <param name="iterations"></param>
        public Cryptography(string salt, string vector, CryptoKeySizes keySize, string hash = "SHA1", int iterations = 2)
        {
            _keyBitSize = keySize.GetHashCode();
            _saltBytes = Encoding.UTF8.GetBytes(salt);
            _vectorBytes = Encoding.UTF8.GetBytes(vector);
            _hash = hash;
            _iterations = iterations;
        }

        /// <summary>
        /// Encrypt default with Managed AES.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Encrypt(string value, string password) =>
            Encrypt<AesManaged>(value, password);

        /// <summary>
        /// Encrypt with custom SymmetricAlgoritm.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="password"></param>
        /// <returns>string</returns>
        public string Encrypt<T>(string value, string password) 
            where T : SymmetricAlgorithm, new() =>
            Convert.ToBase64String(Encrypt<T>(StringUtility.GetBytes<UTF8Encoding>(value), password));

        /// <summary>
        /// Encrypt with custom SymmetricAlgoritm.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="password"></param>
        /// <returns>byte[]</returns>
        public byte[] Encrypt<T>(byte[] value, string password)
                where T : SymmetricAlgorithm, new()
        {
            byte[] encrypted = default;

            using (T cipher = new T())
            {
                var _passwordBytes = new PasswordDeriveBytes(password, _saltBytes, _hash, _iterations);
                var keyBytes = _passwordBytes.GetBytes(_keyBitSize / 8);

                if (CheckIfKeySizeIsValid(cipher, keyBytes.Length * 8) && CheckIfBlockSizeIsValid(cipher, _vectorBytes.Length * 8))
                {
                    cipher.Mode = CipherMode.CBC;

                    using (ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, _vectorBytes))
                    {
                        using MemoryStream to = new MemoryStream();
                        using CryptoStream writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write);
                        writer.Write(value, 0, value.Length);
                        writer.FlushFinalBlock();
                        encrypted = to.ToArray();
                    }
                    cipher.Clear();
                }
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypt string default with AES Managed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Decrypt(string value, string password) =>
            Decrypt<AesManaged>(value, password);

        /// <summary>
        /// Decrypt with custom SymmetricAlgoritm.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Decrypt<T>(string value, string password) 
            where T : SymmetricAlgorithm, new() =>
            Decrypt<T>(Convert.FromBase64String(value), password);

        /// <summary>
        /// Decrypt with custom SymmetricAlgoritm.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Decrypt<T>(byte[] value, string password)
            where T : SymmetricAlgorithm, new()
        {
            byte[] decrypted = default;
            var decryptedByteCount = 0;

            using (T cipher = new T())
            {
                var _passwordBytes = new PasswordDeriveBytes(password, _saltBytes, _hash, _iterations);
                
                var keyBytes = _passwordBytes.GetBytes(_keyBitSize / 8);

                if (CheckIfKeySizeIsValid(cipher, keyBytes.Length * 8) && CheckIfBlockSizeIsValid(cipher, _vectorBytes.Length * 8))
                {
                    cipher.Mode = CipherMode.CBC;

                    try
                    {
                        using ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, _vectorBytes);
                        using MemoryStream from = new MemoryStream(value);
                        using CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read);
                        decrypted = new byte[value.Length];
                        decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }

                    cipher.Clear();
                }
            }

            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        private bool CheckIfKeySizeIsValid<T>(T cipher, int keySize)
        {
            var errorString = new StringBuilder("Legal min or max key size exceeded.");

            switch (cipher)
            {
                case AesManaged aes:
                    foreach (var item in aes.LegalKeySizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && keySize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if(keySize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in aes.LegalKeySizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case DESCryptoServiceProvider des:
                    foreach (var item in des.LegalKeySizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && keySize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (keySize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in des.LegalKeySizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case RC2CryptoServiceProvider rc2:
                    foreach (var item in rc2.LegalKeySizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && keySize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (keySize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in rc2.LegalKeySizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case RijndaelManaged rijndael:
                    foreach (var item in rijndael.LegalKeySizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && keySize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (keySize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in rijndael.LegalKeySizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case TripleDESCryptoServiceProvider tripleDES:
                    foreach (var item in tripleDES.LegalKeySizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && keySize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (keySize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in tripleDES.LegalKeySizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
            }

            throw new ArgumentOutOfRangeException(errorString.ToString());
        }

        private bool CheckIfBlockSizeIsValid<T>(T cipher, int blockSize)
        {
            var errorString = new StringBuilder("Legal min or max block size exceeded.");

            switch (cipher)
            {
                case AesManaged aes:
                    foreach (var item in aes.LegalBlockSizes)
                    {
                        if(item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && blockSize.Equals(item.MaxSize))
                            return true;
                        else if(!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (blockSize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in aes.LegalBlockSizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case DESCryptoServiceProvider des:
                    foreach (var item in des.LegalBlockSizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && blockSize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (blockSize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in des.LegalBlockSizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case RC2CryptoServiceProvider rc2:
                    foreach (var item in rc2.LegalBlockSizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && blockSize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (blockSize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in rc2.LegalBlockSizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case RijndaelManaged rijndael:
                    foreach (var item in rijndael.LegalBlockSizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && blockSize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (blockSize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in rijndael.LegalBlockSizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
                case TripleDESCryptoServiceProvider tripleDES:
                    foreach (var item in tripleDES.LegalBlockSizes)
                    {
                        if (item.SkipSize.Equals(0) && item.MinSize.Equals(item.MaxSize) && blockSize.Equals(item.MaxSize))
                            return true;
                        else if (!item.SkipSize.Equals(0))
                            for (int i = item.MinSize; i <= item.MaxSize; i += item.SkipSize)
                            {
                                if (blockSize.Equals(i))
                                    return true;
                            }
                    }

                    foreach (var item in tripleDES.LegalBlockSizes)
                    {
                        errorString.AppendLine($"Legal min key size = {item.MinSize}");
                        errorString.AppendLine($"Legal max key size = {item.MaxSize}");
                    }

                    break;
            }

            throw new ArgumentOutOfRangeException(errorString.ToString());
        }
    }
}
