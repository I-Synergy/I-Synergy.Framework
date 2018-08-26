using ISynergy.Library;
using Xunit;

namespace ISynergy.Common.Handlers.Tests
{
    public class GenericStringTests
    {
        [Fact]
        [Trait(nameof(General.IncreaseString2Long), Test.Unit)]
        public void IncreaseStringNumericSummand1()
        {
            string result = General.IncreaseString2Long("10", 1);
            Assert.Equal("11", result);
        }

        [Fact]
        [Trait(nameof(General.IncreaseString2Long), Test.Unit)]
        public void IncreaseStringNumericSummand3()
        {
            string result = General.IncreaseString2Long("6281085010557", 3);
            Assert.Equal("6281085010560", result);
        }

        [Fact]
        [Trait(nameof(General.IncreaseString2Long), Test.Unit)]
        public void IncreaseStringAlphaNumericSummand1()
        {
            string result = General.IncreaseString2Long("A19", 1);
            Assert.Equal("A20", result);
        }

        [Fact]
        [Trait(nameof(General.IncreaseString2Long), Test.Unit)]
        public void IncreaseStringAlphaNumericSummand8()
        {
            string result = General.IncreaseString2Long("AZURE02", 8);
            Assert.Equal("AZURE10", result);
        }

        [Fact]
        [Trait(nameof(General.IncreaseString2Long), Test.Unit)]
        public void IncreaseStringAlphaNumericComplex()
        {
            string result = General.IncreaseString2Long("2016AZURE10STAGE001", 99);
            Assert.Equal("2016AZURE10STAGE100", result);
        }
    }
}