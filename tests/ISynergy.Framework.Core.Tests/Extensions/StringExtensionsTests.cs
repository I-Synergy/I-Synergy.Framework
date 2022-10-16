using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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

        [TestMethod()]
        public void ToCapitalizedTest()
        {
            var source = "hallo ik ben ismail. ik ben software developer.";
            var result = "Hallo ik ben ismail. Ik ben software developer.";

            Assert.AreEqual(result, source.ToCapitalized());
        }

        [TestMethod()]
        public void ToCapitalizedFirstLetterTest()
        {
            var source = "hallo ik ben ISMAIL. ik ben SOFTWARE DEVELOPER.";
            var result = "Hallo Ik Ben Ismail. Ik Ben Software Developer.";

            Assert.AreEqual(result, source.ToCapitalizedFirstLetter());
        }

        [TestMethod()]
        public void SplitAndKeepTest()
        {
            var text = "[a link|http://www.google.com]";
            var result = text.SplitAndKeep('[', '|', ']');
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("[", result[0]);
            Assert.AreEqual("a link", result[1]);
            Assert.AreEqual("|", result[2]);
            Assert.AreEqual("http://www.google.com", result[3]);
            Assert.AreEqual("]", result[4]);
        }
    }
}
