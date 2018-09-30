using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class TaxesTests
    {
        [Fact]
        [Trait(nameof(Tax), Test.Unit)]
        public void CalcPriceExclVATTest()
        {
            decimal result = Tax.CalcPriceExclVAT(21, 121);
            Assert.Equal(100, result);
        }

        [Fact]
        [Trait(nameof(Tax), Test.Unit)]
        public void CalcPriceInclVATTest()
        {
            decimal result = Tax.CalcPriceInclVAT(21, 100);
            Assert.Equal(121, result);
        }

        [Fact]
        [Trait(nameof(Tax), Test.Unit)]
        public void CalcVATExclVATTest()
        {
            decimal result = Tax.CalcVATExclVAT(21, 100);
            Assert.Equal(21, result);
        }

        [Fact]
        [Trait(nameof(Tax), Test.Unit)]
        public void CalcVATInclVATTest()
        {
            decimal result = Tax.CalcVATInclVAT(21, 121);
            Assert.Equal(21, result);
        }
    }
}