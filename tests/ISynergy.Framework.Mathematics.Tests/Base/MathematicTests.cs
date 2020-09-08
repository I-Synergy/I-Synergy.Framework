using Xunit;

namespace ISynergy.Framework.Mathematics.Base.Tests
{
    /// <summary>
    /// Class MathematicTests.
    /// </summary>
    public class MathematicTests
    {
        /// <summary>
        /// Defines the test method IsEvenTest.
        /// </summary>
        [Fact]
        public void IsEvenTest()
        {
            var result = Mathematics.IsEven(28);
            Assert.True(result);
        }

        /// <summary>
        /// Defines the test method IsOnEvenTest.
        /// </summary>
        [Fact]
        public void IsOnEvenTest()
        {
            var result = Mathematics.IsEven(15);
            Assert.False(result);
        }
    }
}
