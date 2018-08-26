using ISynergy.Library;
using Xunit;

namespace ISynergy.Common.Calculations.Tests
{
    public class BankingTests
    {
        [Fact]
        [Trait(nameof(Bank), Test.Unit)]
        public void CheckAccountBankTest()
        {
            bool result = Bank.CheckAccount("150483341");
            Assert.True(result);
        }

        [Fact]
        [Trait(nameof(Bank), Test.Unit)]
        public void CheckAccountGiroTest()
        {
            bool result = Bank.CheckAccount("8318140");
            Assert.False(result);
        }

        [Fact]
        [Trait(nameof(Bank), Test.Unit)]
        public void CheckSofinummerTest()
        {
            bool result = Bank.CheckAccount("169649167");
            Assert.False(result);
        }

        [Fact]
        [Trait(nameof(Bank), Test.Unit)]
        public void CheckAccountStringTest()
        {
            bool result = Bank.CheckAccount("abcdefghijklmnopqrstuvwxyz");
            Assert.False(result);
        }

        [Fact]
        [Trait(nameof(Bank), Test.Unit)]
        public void CheckAccountNumbersTest()
        {
            bool result = Bank.CheckAccount("123456789");
            Assert.True(result);
        }
    }
}