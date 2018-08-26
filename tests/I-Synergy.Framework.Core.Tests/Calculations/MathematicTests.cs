using ISynergy.Library;
using Xunit;

namespace ISynergy.Common.Calculations.Tests
{
    public class MathematicTests
    {
        [Fact]
        [Trait(nameof(Mathematics.IsEven), Test.Unit)]
        public void IsEvenTest()
        {
            bool result = Mathematics.IsEven(28);
            Assert.True(result);
        }

        [Fact]
        [Trait(nameof(Mathematics.IsEven), Test.Unit)]
        public void IsOnEvenTest()
        {
            bool result = Mathematics.IsEven(15);
            Assert.False(result);
        }
    }
}