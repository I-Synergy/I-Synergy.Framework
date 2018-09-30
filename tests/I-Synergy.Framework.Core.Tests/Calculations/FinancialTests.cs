using System;
using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class FinancialTests
    {
        [Theory]
        [Trait(nameof(Financial), Test.Unit)]
        [InlineData(1, 50, 49)]
        [InlineData(100, 120, 0.2)]
        [InlineData(0, 0, 0)]
        [InlineData(0, 100, 1)]
        [InlineData(12510.41, 14696.23, 0.1747)]
        public void CalcPercAmountOfAmountTest(decimal amount, decimal mainamount, decimal result)
        {
            decimal assert = Math.Round(Financial.CalcPercAmountOfAmount(amount, mainamount), 4);
            Assert.Equal(result, assert);
        }

        [Fact]
        [Trait(nameof(Financial), Test.Unit)]
        public void CalcPercAmountOfAmountDevideByZeroTest()
        {
            decimal result = Financial.CalcPercAmountOfAmount(0m, 0m);
            Assert.Equal(0, result);
        }

        [Fact]
        [Trait(nameof(Financial), Test.Unit)]
        public void CalcAmountOfPercentageTest()
        {
            decimal result = Financial.CalcAmountOfPercentage(121, 21);
            Assert.Equal(25.41m, result);
        }

        [Theory]
        [Trait(nameof(Financial), Test.Unit)]
        [InlineData(1, 50, 49)]
        [InlineData(10, 100, 9)]
        [InlineData(10, 20, 1)]
        [InlineData(0, 100, 1)]
        public void CalcMarginPercentageTest(decimal purchasePrice, decimal salesPrice, decimal result)
        {
            decimal assert = Financial.CalcMarginPercentage(salesPrice, purchasePrice);
            Assert.Equal(result, assert);
        }

        [Fact]
        [Trait(nameof(Financial), Test.Unit)]
        public void CalcMarginPercentageDevideByZeroTest()
        {
            decimal result = Financial.CalcMarginPercentage(100, 0);
            Assert.Equal(1m, result);
        }
    }
}