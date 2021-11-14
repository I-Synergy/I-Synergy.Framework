using ISynergy.Framework.Mathematics.Environments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests
{
    [TestClass]
    public class REnvironmentTest : REnvironment
    {
        [TestMethod]
        public void eyeTest()
        {
            var x = c(1, 2, 3, 4);

            var m = matrix(x, 2, 2);

            double[,] actual = m;
            Assert.AreEqual(1, actual[0, 0]);
            Assert.AreEqual(3, actual[0, 1]);
            Assert.AreEqual(2, actual[1, 0]);
            Assert.AreEqual(4, actual[1, 1]);
        }
    }
}