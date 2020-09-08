using Xunit;

namespace ISynergy.Framework.Financial.Tests
{
    public class VATTests
    {
        [Fact]
        public void CalcPriceExclVATTest()
        {
            var result = VAT.CalculateAmountFromAmountExcludingVAT(21, 121);
            Assert.Equal(100, result);
        }

        [Fact]
        public void CalcPriceInclVATTest()
        {
            var result = VAT.CalculateAmountFromAmountIncludingVAT(21, 100);
            Assert.Equal(121, result);
        }

        [Fact]
        public void CalcVATExclVATTest()
        {
            var result = VAT.CalculateVATFromAmountExcludingVAT(21, 100);
            Assert.Equal(21, result);
        }

        [Fact]
        public void CalcVATInclVATTest()
        {
            var result = VAT.CalculateVATFromAmountIncludingVAT(21, 121);
            Assert.Equal(21, result);
        }
    }
}
