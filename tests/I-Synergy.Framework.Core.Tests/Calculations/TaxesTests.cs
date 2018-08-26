using ISynergy.Library;
using Xunit;

namespace ISynergy.Common.Calculations.Tests
{
    public class TaxesTests
    {
        [Fact]
        [Trait(nameof(Calculations.Tax), Test.Unit)]
        public void CalcPriceExclVATTest()
        {
            decimal result = Calculations.Tax.CalcPriceExclVAT(21, 121);
            Assert.Equal(100, result);
        }

        [Fact]
        [Trait(nameof(Calculations.Tax), Test.Unit)]
        public void CalcPriceInclVATTest()
        {
            decimal result = Calculations.Tax.CalcPriceInclVAT(21, 100);
            Assert.Equal(121, result);
        }

        [Fact]
        [Trait(nameof(Calculations.Tax), Test.Unit)]
        public void CalcVATExclVATTest()
        {
            decimal result = Calculations.Tax.CalcVATExclVAT(21, 100);
            Assert.Equal(21, result);
        }

        [Fact]
        [Trait(nameof(Calculations.Tax), Test.Unit)]
        public void CalcVATInclVATTest()
        {
            decimal result = Calculations.Tax.CalcVATInclVAT(21, 121);
            Assert.Equal(21, result);
        }
    }
}