using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Base.Tests
{
    /// <summary>
    /// Class MetricTests.
    /// </summary>
    [TestClass]
    public class MetricTests
    {
        /// <summary>
        /// Defines the test method Surface100Test.
        /// </summary>
        [TestMethod]
        public void Surface100Test()
        {
            var result = Metric.Surface(10, 10);
            Assert.AreEqual(100, result);
        }

        /// <summary>
        /// Defines the test method SurfaceDecimalTest.
        /// </summary>
        [TestMethod]
        public void SurfaceDecimalTest()
        {
            var result = Metric.Surface(2.5m, 7.3m);
            Assert.AreEqual(18.25m, result);
        }

        /// <summary>
        /// Defines the test method Volume1000Test.
        /// </summary>
        [TestMethod]
        public void Volume1000Test()
        {
            var result = Metric.Volume(10, 10, 10);
            Assert.AreEqual(1000, result);
        }

        /// <summary>
        /// Defines the test method VolumeDecimalTest.
        /// </summary>
        [TestMethod]
        public void VolumeDecimalTest()
        {
            var result = Metric.Volume(2.5m, 7.3m, 5.7m);
            Assert.AreEqual(104.025m, result);
        }

        /// <summary>
        /// Defines the test method DensityTest.
        /// </summary>
        [TestMethod]
        public void DensityTest()
        {
            var result = Metric.Density(1000m, 100m);
            Assert.AreEqual(0.1m, result);
        }
    }
}
