using ISynergy.Framework.Geography.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    /// <summary>
    /// Class UtmProjectionTests.
    /// </summary>
    [TestClass]
    public class UtmProjectionTests
    {
        /// <summary>
        /// The utm
        /// </summary>
        private readonly UtmProjection utm = new();

        /// <summary>
        /// Defines the test method TestMyHome.
        /// </summary>
        [TestMethod]
        public void TestMyHome()
        {
            UtmCoordinate e = (UtmCoordinate)utm.ToEuclidian(Constants.MyHome);
            // Reference Computation from http://www.earthpoint.us/Convert.aspx
            Assert.AreEqual("32U 485577 5521521", e.ToString());
        }

        /// <summary>
        /// Defines the test method TestInversion.
        /// </summary>
        [TestMethod]
        public void TestInversion()
        {
            UtmCoordinate e = (UtmCoordinate)utm.ToEuclidian(Constants.MyHome);
            Global.GlobalCoordinates c = utm.FromEuclidian(e);
            Assert.IsTrue(c.IsApproximatelyEqual(Constants.MyHome, 0.000000001));
        }

        /// <summary>
        /// Defines the test method TestEquals1.
        /// </summary>
        [TestMethod]
        public void TestEquals1()
        {
            string s = "123";
            Assert.IsFalse(utm.Equals(s));
        }

        /// <summary>
        /// Defines the test method TestEquals2.
        /// </summary>
        [TestMethod]
        public void TestEquals2()
        {
            UtmProjection utm2 = new();
        }
    }
}
