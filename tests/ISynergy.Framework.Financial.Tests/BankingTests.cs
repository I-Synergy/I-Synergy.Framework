using Xunit;

namespace ISynergy.Framework.Financial.Tests
{
    public class BankingTests
    {
        [Fact]
        public void CheckAccountBankTest()
        {
            var result = Banking.ElevenTest("150483341");
            Assert.True(result);
        }

        [Fact]
        public void CheckAccountGiroTest()
        {
            var result = Banking.ElevenTest("8318140");
            Assert.False(result);
        }

        [Fact]
        public void CheckSofinummerTest()
        {
            var result = Banking.ElevenTest("169649167");
            Assert.False(result);
        }

        [Fact]
        public void CheckAccountStringTest()
        {
            var result = Banking.ElevenTest("abcdefghijklmnopqrstuvwxyz");
            Assert.False(result);
        }

        [Fact]
        public void CheckAccountNumbersTest()
        {
            var result = Banking.ElevenTest("123456789");
            Assert.True(result);
        }
    }
}
