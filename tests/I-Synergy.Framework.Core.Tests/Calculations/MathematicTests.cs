using ISynergy.Framework.Tests.Base;
using Xunit;

namespace ISynergy.Calculations.Tests
{
    public class MathematicTests : UnitTest
    {
        [Fact]
        public void IsEvenTest()
        {
            bool result = Mathematics.IsEven(28);
            Assert.True(result);
        }

        [Fact]
        public void IsOnEvenTest()
        {
            bool result = Mathematics.IsEven(15);
            Assert.False(result);
        }
    }
}