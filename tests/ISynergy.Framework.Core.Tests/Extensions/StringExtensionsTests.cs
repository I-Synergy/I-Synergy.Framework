using Xunit;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    /// <summary>
    /// Class StringExtensionsTests.
    /// </summary>
    public class StringExtensionsTests
    {
        /// <summary>
        /// Defines the test method IncreaseStringNumericSummand1.
        /// </summary>
        [Fact]
        public void IncreaseStringNumericSummand1()
        {
            var result = "10".IncreaseString2Long(1);
            Assert.Equal("11", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringNumericSummand3.
        /// </summary>
        [Fact]
        public void IncreaseStringNumericSummand3()
        {
            var result = "6281085010557".IncreaseString2Long(3);
            Assert.Equal("6281085010560", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringAlphaNumericSummand1.
        /// </summary>
        [Fact]
        public void IncreaseStringAlphaNumericSummand1()
        {
            var result = "A19".IncreaseString2Long(1);
            Assert.Equal("A20", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringAlphaNumericSummand8.
        /// </summary>
        [Fact]
        public void IncreaseStringAlphaNumericSummand8()
        {
            var result = "AZURE02".IncreaseString2Long(8);
            Assert.Equal("AZURE10", result);
        }

        /// <summary>
        /// Defines the test method IncreaseStringAlphaNumericComplex.
        /// </summary>
        [Fact]
        public void IncreaseStringAlphaNumericComplex()
        {
            var result = "2016AZURE10STAGE001".IncreaseString2Long(99);
            Assert.Equal("2016AZURE10STAGE100", result);
        }

        /// <summary>
        /// Defines the test method CovertString2NumericIntegerTest.
        /// </summary>
        [Fact]
        public void CovertString2NumericIntegerTest()
        {
            var result = "2016001".CovertString2Numeric();
            Assert.Equal(2016001, result);
        }

        /// <summary>
        /// Defines the test method CovertString2NumericNonIntegerTest.
        /// </summary>
        [Fact]
        public void CovertString2NumericNonIntegerTest()
        {
            var result = "9999992016001".CovertString2Numeric();
            Assert.Equal(0, result);
        }
    }
}
