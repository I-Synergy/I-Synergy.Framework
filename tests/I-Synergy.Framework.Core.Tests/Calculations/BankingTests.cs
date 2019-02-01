using ISynergy.Framework.Tests.Base;
using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class BankingTests : UnitTest
    {
        [Fact]
        public void CheckAccountBankTest()
        {
            bool result = Bank.CheckAccount("150483341");
            Assert.True(result);
        }

        [Fact]
        public void CheckAccountGiroTest()
        {
            bool result = Bank.CheckAccount("8318140");
            Assert.False(result);
        }

        [Fact]
        public void CheckSofinummerTest()
        {
            bool result = Bank.CheckAccount("169649167");
            Assert.False(result);
        }

        [Fact]
        public void CheckAccountStringTest()
        {
            bool result = Bank.CheckAccount("abcdefghijklmnopqrstuvwxyz");
            Assert.False(result);
        }

        [Fact]
        public void CheckAccountNumbersTest()
        {
            bool result = Bank.CheckAccount("123456789");
            Assert.True(result);
        }
    }
}