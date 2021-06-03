using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Base.Tests
{
    /// <summary>
    /// Class MathematicTests.
    /// </summary>
    [TestClass]
    public class MathematicTests
    {
        /// <summary>
        /// Defines the test method IsEvenTest.
        /// </summary>
        [TestMethod]
        public void IsEvenTest()
        {
            var result = Mathematics.IsEven(28);
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Defines the test method IsOnEvenTest.
        /// </summary>
        [TestMethod]
        public void IsOnEvenTest()
        {
            var result = Mathematics.IsEven(15);
            Assert.IsFalse(result);
        }
    }
}
