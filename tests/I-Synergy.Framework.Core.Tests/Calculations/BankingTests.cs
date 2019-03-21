using ISynergy.Framework.Tests.Base;
using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class BankingTests : UnitTest
    {
        [Fact]
        public void CheckAccountBankTest()
        {
            var result = Bank.CheckAccount("150483341");
            Assert.True(result);
        }

        [Fact]
        public void CheckAccountGiroTest()
        {
            var result = Bank.CheckAccount("8318140");
            Assert.False(result);
        }

        [Fact]
        public void CheckSofinummerTest()
        {
            var result = Bank.CheckAccount("169649167");
            Assert.False(result);
        }

        [Fact]
        public void CheckAccountStringTest()
        {
            var result = Bank.CheckAccount("abcdefghijklmnopqrstuvwxyz");
            Assert.False(result);
        }

        [Fact]
        public void CheckAccountNumbersTest()
        {
            var result = Bank.CheckAccount("123456789");
            Assert.True(result);
        }
    }
}