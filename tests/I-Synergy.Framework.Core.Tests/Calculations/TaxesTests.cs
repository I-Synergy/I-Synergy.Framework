using ISynergy.Framework.Tests.Base;
using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class TaxesTests : UnitTest
    {
        [Fact]
        public void CalcPriceExclVATTest()
        {
            var result = Tax.CalcPriceExclVAT(21, 121);
            Assert.Equal(100, result);
        }

        [Fact]
        public void CalcPriceInclVATTest()
        {
            var result = Tax.CalcPriceInclVAT(21, 100);
            Assert.Equal(121, result);
        }

        [Fact]
        public void CalcVATExclVATTest()
        {
            var result = Tax.CalcVATExclVAT(21, 100);
            Assert.Equal(21, result);
        }

        [Fact]
        public void CalcVATInclVATTest()
        {
            var result = Tax.CalcVATInclVAT(21, 121);
            Assert.Equal(21, result);
        }
    }
}