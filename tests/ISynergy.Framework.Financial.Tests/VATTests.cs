using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Financial.Tests
{
    /// <summary>
    /// Class VATTests.
    /// </summary>
    [TestClass]
    public class VATTests
    {
        /// <summary>
        /// Defines the test method CalcPriceExclVATTest.
        /// </summary>
        [TestMethod]
        public void CalculateAmountFromAmountExcludingVATTest()
        {
            decimal result = VAT.CalculateAmountFromAmountExcludingVAT(21, 100);
            Assert.AreEqual(121, result);
        }

        /// <summary>
        /// Defines the test method CalcPriceInclVATTest.
        /// </summary>
        [TestMethod]
        public void CalculateAmountFromAmountIncludingVATTest()
        {
            decimal result = VAT.CalculateAmountFromAmountIncludingVAT(21, 121);
            Assert.AreEqual(100, result);
        }

        /// <summary>
        /// Defines the test method CalcVATExclVATTest.
        /// </summary>
        [TestMethod]
        public void CalculateVATFromAmountExcludingVATTest()
        {
            decimal result = VAT.CalculateVATFromAmountExcludingVAT(21, 100);
            Assert.AreEqual(21, result);
        }

        /// <summary>
        /// Defines the test method CalcVATInclVATTest.
        /// </summary>
        [TestMethod]
        public void CalculateVATFromAmountIncludingVATTest()
        {
            decimal result = VAT.CalculateVATFromAmountIncludingVAT(21, 121);
            Assert.AreEqual(21, result);
        }
    }
}
