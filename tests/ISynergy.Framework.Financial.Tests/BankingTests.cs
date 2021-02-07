using Xunit;

namespace ISynergy.Framework.Financial.Tests
{
    /// <summary>
    /// Class BankingTests.
    /// </summary>
    public class BankingTests
    {
        /// <summary>
        /// Defines the test method CheckAccountBankTest.
        /// </summary>
        [Fact]
        public void CheckAccountBankTest()
        {
            var result = Banking.ElevenTest("150483341");
            Assert.True(result);
        }

        /// <summary>
        /// Defines the test method CheckAccountGiroTest.
        /// </summary>
        [Fact]
        public void CheckAccountGiroTest()
        {
            var result = Banking.ElevenTest("8318140");
            Assert.False(result);
        }

        /// <summary>
        /// Defines the test method CheckSofinummerTest.
        /// </summary>
        [Fact]
        public void CheckSofinummerTest()
        {
            var result = Banking.ElevenTest("169649167");
            Assert.False(result);
        }

        /// <summary>
        /// Defines the test method CheckAccountStringTest.
        /// </summary>
        [Fact]
        public void CheckAccountStringTest()
        {
            var result = Banking.ElevenTest("abcdefghijklmnopqrstuvwxyz");
            Assert.False(result);
        }

        /// <summary>
        /// Defines the test method CheckAccountNumbersTest.
        /// </summary>
        [Fact]
        public void CheckAccountNumbersTest()
        {
            var result = Banking.ElevenTest("123456789");
            Assert.True(result);
        }
    }
}
