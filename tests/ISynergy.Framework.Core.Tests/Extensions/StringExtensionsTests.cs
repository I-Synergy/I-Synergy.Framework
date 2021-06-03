using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class StringExtensionsTests.
    /// </summary>
    [TestClass]
    public class StringExtensionsTests
    {
        /// <summary>
        /// Defines the test method IncreaseStringNumericSummand1.
        /// </summary>
        [TestMethod]
        public void IncreaseStringNumericSummand1()
        {
            var result = "10".IncreaseString2Long(1);
            Assert.AreEqual("11", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringNumericSummand3.
        /// </summary>
        [TestMethod]
        public void IncreaseStringNumericSummand3()
        {
            var result = "6281085010557".IncreaseString2Long(3);
            Assert.AreEqual("6281085010560", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringAlphaNumericSummand1.
        /// </summary>
        [TestMethod]
        public void IncreaseStringAlphaNumericSummand1()
        {
            var result = "A19".IncreaseString2Long(1);
            Assert.AreEqual("A20", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringAlphaNumericSummand8.
        /// </summary>
        [TestMethod]
        public void IncreaseStringAlphaNumericSummand8()
        {
            var result = "AZURE02".IncreaseString2Long(8);
            Assert.AreEqual("AZURE10", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringAlphaNumericComplex.
        /// </summary>
        [TestMethod]
        public void IncreaseStringAlphaNumericComplex()
        {
            var result = "2016AZURE10STAGE001".IncreaseString2Long(99);
            Assert.AreEqual("2016AZURE10STAGE100", result);
        }

        /// <summary>
        /// Defines the test method CovertString2NumericIntegerTest.
        /// </summary>
        [TestMethod]
        public void CovertString2NumericIntegerTest()
        {
            var result = "2016001".CovertString2Numeric();
            Assert.AreEqual(2016001, result);
        }

        /// <summary>
        /// Defines the test method CovertString2NumericNonIntegerTest.
        /// </summary>
        [TestMethod]
        public void CovertString2NumericNonIntegerTest()
        {
            var result = "9999992016001".CovertString2Numeric();
            Assert.AreEqual(0, result);
        }
    }
}
