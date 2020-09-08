using System.Security.Cryptography;
using ISynergy.Framework.Core.Encryption;
using ISynergy.Framework.Core.Enumerations;
using Xunit;

namespace ISynergy.Framework.Core.Tests.Encryption
{
    public class CryptographyTests
    {
        private readonly string _salt40 = "12345";
        private readonly string _salt64 = "12345678";
        private readonly string _salt128 = "RandomSalt123456";
        private readonly string _salt192 = "RandomSalt123456789012";
        private readonly string _salt256 = "RandomSalt123456RandomSalt123456";
        private readonly string _vector64 = "12345678";
        private readonly string _vector128 = "RandomVector1234";
        private readonly string _password = "RandomPassword";
        private readonly string _secret = "This is a secret text.";

        // https://stackoverflow.com/questions/2919228/specified-key-is-not-a-valid-size-for-this-algorithm?noredirect=1&lq=1
        // https://stackoverflow.com/questions/273452/using-aes-encryption-in-c-sharp?rq=1

        // DES

        [Fact]
        public void EncryptionAndDecryptionDESCryptoServiceProvider64Test()
        {
            var crypto = new Cryptography(_salt64, _vector64, CryptoKeySizes.Key64);
            var encryptedValue = crypto.Encrypt<DESCryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<DESCryptoServiceProvider>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        // AESManaged

        [Fact]
        public void EncryptionAndDecryptionAesManaged128Test()
        {
            var crypto = new Cryptography(_salt128, _vector128, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt(_secret, _password);
            var result = crypto.Decrypt(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        [Fact]
        public void EncryptionAndDecryptionAesManaged256Test()
        {
            var crypto = new Cryptography(_salt256, _vector128, keySize: CryptoKeySizes.Key256);
            var encryptedValue = crypto.Encrypt(_secret, _password);
            var result = crypto.Decrypt(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        //RIJNDAEL

        [Fact]
        public void EncryptionAndDecryptionRijndaelManaged128Test()
        {
            var crypto = new Cryptography(_salt128, _vector128, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt<RijndaelManaged>(_secret, _password);
            var result = crypto.Decrypt<RijndaelManaged>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        [Fact]
        public void EncryptionAndDecryptionRijndaelManaged256Test()
        {
            var crypto = new Cryptography(_salt128, _vector128, keySize: CryptoKeySizes.Key256);
            var encryptedValue = crypto.Encrypt<RijndaelManaged>(_secret, _password);
            var result = crypto.Decrypt<RijndaelManaged>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        // RC2

        [Fact]
        public void EncryptionAndDecryptionRC2CryptoServiceProvider40Test()
        {
            var crypto = new Cryptography(_salt40, _vector64, keySize: CryptoKeySizes.Key40);
            var encryptedValue = crypto.Encrypt<RC2CryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<RC2CryptoServiceProvider>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        [Fact]
        public void EncryptionAndDecryptionRC2CryptoServiceProvider64Test()
        {
            var crypto = new Cryptography(_salt64, _vector64, keySize: CryptoKeySizes.Key64);
            var encryptedValue = crypto.Encrypt<RC2CryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<RC2CryptoServiceProvider>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        [Fact]
        public void EncryptionAndDecryptionRC2CryptoServiceProvider128Test()
        {
            var crypto = new Cryptography(_salt128, _vector64, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt<RC2CryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<RC2CryptoServiceProvider>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        // TRIPLE DES

        [Fact]
        public void EncryptionAndDecryptionTripleDESCryptoServiceProvider128Test()
        {
            var crypto = new Cryptography(_salt128, _vector64, keySize: CryptoKeySizes.Key128);
            var encryptedValue = crypto.Encrypt<TripleDESCryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<TripleDESCryptoServiceProvider>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }

        [Fact]
        public void EncryptionAndDecryptionTripleDESCryptoServiceProvider192Test()
        {
            var crypto = new Cryptography(_salt192, _vector64, keySize: CryptoKeySizes.Key192);
            var encryptedValue = crypto.Encrypt<TripleDESCryptoServiceProvider>(_secret, _password);
            var result = crypto.Decrypt<TripleDESCryptoServiceProvider>(encryptedValue, _password);

            Assert.Equal(_secret, result);
        }
    }
}
