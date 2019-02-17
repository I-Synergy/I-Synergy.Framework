using Xunit;
using System;
using ISynergy.Framework.Tests.Base;

namespace ISynergy.Handlers.Tests
{
    public class GenericStringTests : UnitTest
    {
        [Fact]
        public void IncreaseStringNumericSummand1()
        {
            string result = ("10").IncreaseString2Long(1);
            Assert.Equal("11", result);
        }

        [Fact]
        public void IncreaseStringNumericSummand3()
        {
            string result = ("6281085010557").IncreaseString2Long(3);
            Assert.Equal("6281085010560", result);
        }

        [Fact]
        public void IncreaseStringAlphaNumericSummand1()
        {
            string result = ("A19").IncreaseString2Long(1);
            Assert.Equal("A20", result);
        }

        [Fact]
        public void IncreaseStringAlphaNumericSummand8()
        {
            string result = ("AZURE02").IncreaseString2Long(8);
            Assert.Equal("AZURE10", result);
        }

        [Fact]
        public void IncreaseStringAlphaNumericComplex()
        {
            string result = ("2016AZURE10STAGE001").IncreaseString2Long(99);
            Assert.Equal("2016AZURE10STAGE100", result);
        }
    }
}