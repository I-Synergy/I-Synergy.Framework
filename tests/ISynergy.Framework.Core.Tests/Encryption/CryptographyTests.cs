using System.Security.Cryptography;
using ISynergy.Framework.Core.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Encryption.Tests
{
    /// <summary>
    /// Class CryptographyTests.
    /// </summary>
    [TestClass]
    public class CryptographyTests
    {
        /// <summary>
        /// The salt40
        /// </summary>
        private readonly string _salt40 = "12345";
        /// <summary>
        /// The salt64
        /// </summary>
        private readonly string _salt64 = "12345678";
        /// <summary>
        /// The salt128
        /// </summary>
        private readonly string _salt128 = "RandomSalt123456";
        /// <summary>
        /// The salt192
        /// </summary>
        private readonly string _salt192 = "RandomSalt123456789012";
        /// <summary>
        /// The salt256
        /// </summary>
        private readonly string _salt256 = "RandomSalt123456RandomSalt123456";
        /// <summary>
        /// The vector64
        /// </summary>
        private readonly string _vector64 = "12345678";
        /// <summary>
        /// The vector128
        /// </summary>
        private readonly string _vector128 = "RandomVector1234";
        /// <summary>
        /// The password
        /// </summary>
        private readonly string _password = "RandomPassword";
        /// <summary>
        /// The secret
        /// </summary>
        private readonly string _secret = "This is a secret text.";

        // https://stackoverflow.com/questions/2919228/specified-key-is-not-a-valid-size-for-this-algorithm?noredirect=1&lq=1
        // https://stackoverflow.com/questions/273452/using-aes-encryption-in-c-sharp?rq=1

        // DES

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionDESCryptoServiceProvider64Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionDESCryptoServiceProvider64Test()
        {
            var crypto = new Cryptography(_salt64, _vector64, CryptoKeySizes.Key64);
            var encryptedValue = crypto.Encrypt<DESCryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<DESCryptoServiceProvider>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        // AESManaged

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionAesManaged128Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionAesManaged128Test()
        {
            var crypto = new Cryptography(_salt128, _vector128, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt(_secret, _password);
            var result = crypto.Decrypt(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionAesManaged256Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionAesManaged256Test()
        {
            var crypto = new Cryptography(_salt256, _vector128, keySize: CryptoKeySizes.Key256);
            var encryptedValue = crypto.Encrypt(_secret, _password);
            var result = crypto.Decrypt(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        //RIJNDAEL

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionRijndaelManaged128Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionRijndaelManaged128Test()
        {
            var crypto = new Cryptography(_salt128, _vector128, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt<RijndaelManaged>(_secret, _password);
            var result = crypto.Decrypt<RijndaelManaged>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionRijndaelManaged256Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionRijndaelManaged256Test()
        {
            var crypto = new Cryptography(_salt128, _vector128, keySize: CryptoKeySizes.Key256);
            var encryptedValue = crypto.Encrypt<RijndaelManaged>(_secret, _password);
            var result = crypto.Decrypt<RijndaelManaged>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        // RC2

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionRC2CryptoServiceProvider40Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionRC2CryptoServiceProvider40Test()
        {
            var crypto = new Cryptography(_salt40, _vector64, keySize: CryptoKeySizes.Key40);
            var encryptedValue = crypto.Encrypt<RC2CryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<RC2CryptoServiceProvider>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionRC2CryptoServiceProvider64Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionRC2CryptoServiceProvider64Test()
        {
            var crypto = new Cryptography(_salt64, _vector64, keySize: CryptoKeySizes.Key64);
            var encryptedValue = crypto.Encrypt<RC2CryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<RC2CryptoServiceProvider>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionRC2CryptoServiceProvider128Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionRC2CryptoServiceProvider128Test()
        {
            var crypto = new Cryptography(_salt128, _vector64, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt<RC2CryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<RC2CryptoServiceProvider>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        // TRIPLE DES

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionTripleDESCryptoServiceProvider128Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionTripleDESCryptoServiceProvider128Test()
        {
            var crypto = new Cryptography(_salt128, _vector64, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt<TripleDESCryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<TripleDESCryptoServiceProvider>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }

        /// <summary>
        /// Defines the test method EncryptionAndDecryptionTripleDESCryptoServiceProvider192Test.
        /// </summary>
        [TestMethod]
        public void EncryptionAndDecryptionTripleDESCryptoServiceProvider192Test()
        {
            var crypto = new Cryptography(_salt192, _vector64, keySize: CryptoKeySizes.Key192);
            var encryptedValue = crypto.Encrypt<TripleDESCryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<TripleDESCryptoServiceProvider>(encryptedValue, _password);

            Assert.AreEqual(_secret, result);
        }
    }
}
