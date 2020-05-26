using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ISynergy.Framework.Financial.Tests
{
    public class PercentageTests
    {
        [Theory]
        [InlineData(1, 50, 49)]
        [InlineData(100, 120, 0.2)]
        [InlineData(0, 0, 0)]
        [InlineData(0, 100, 1)]
        [InlineData(75, 15, -0.80)]
        [InlineData(573, 520, -0.0925)]
        [InlineData(12510.41, 14696.23, 0.1747)]
        public void CalcPercAmountOfAmountTest(decimal amount, decimal mainamount, decimal result)
        {
            var assert = Math.Round(Percentage.CalculatePercentageAmountOfAmount(amount, mainamount), 4);
            Assert.Equal(result, assert);
        }

        [Fact]
        public void CalcPercAmountOfAmountDevideByZeroTest()
        {
            var result = Percentage.CalculatePercentageAmountOfAmount(0m, 0m);
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalcAmountOfPercentageTest()
        {
            var result = Percentage.CalculateAmountOfPercentage(121, 21);
            Assert.Equal(25.41m, result);
        }

        [Theory]
        [InlineData(1, 50, 49)]
        [InlineData(10, 100, 9)]
        [InlineData(10, 20, 1)]
        [InlineData(0, 100, 1)]
        [InlineData(1.25, 1.75, 0.4)]
        [InlineData(4.95, 2.5, -0.4949)]
        public void CalcMarginPercentageTest(decimal purchasePrice, decimal salesPrice, decimal result)
        {
            var assert = Math.Round(Percentage.CalculateMarginPercentage(salesPrice, purchasePrice), 4);
            Assert.Equal(result, assert);
        }

        [Fact]
        public void CalcMarginPercentageDevideByZeroTest()
        {
            var result = Percentage.CalculateMarginPercentage(100, 0);
            Assert.Equal(1m, result);
        }
    }
}
