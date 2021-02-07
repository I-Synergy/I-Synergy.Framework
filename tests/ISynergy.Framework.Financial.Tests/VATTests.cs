using Xunit;

namespace ISynergy.Framework.Financial.Tests
{
    /// <summary>
    /// Class VATTests.
    /// </summary>
    public class VATTests
    {
        /// <summary>
        /// Defines the test method CalcPriceExclVATTest.
        /// </summary>
        [Fact]
        public void CalcPriceExclVATTest()
        {
            var result = VAT.CalculateAmountFromAmountExcludingVAT(21, 121);
            Assert.Equal(100, result);
        }

        /// <summary>
        /// Defines the test method CalcPriceInclVATTest.
        /// </summary>
        [Fact]
        public void CalcPriceInclVATTest()
        {
            var result = VAT.CalculateAmountFromAmountIncludingVAT(21, 100);
            Assert.Equal(121, result);
        }

        /// <summary>
        /// Defines the test method CalcVATExclVATTest.
        /// </summary>
        [Fact]
        public void CalcVATExclVATTest()
        {
            var result = VAT.CalculateVATFromAmountExcludingVAT(21, 100);
            Assert.Equal(21, result);
        }

        /// <summary>
        /// Defines the test method CalcVATInclVATTest.
        /// </summary>
        [Fact]
        public void CalcVATInclVATTest()
        {
            var result = VAT.CalculateVATFromAmountIncludingVAT(21, 121);
            Assert.Equal(21, result);
        }
    }
}
