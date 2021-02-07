using ISynergy.Framework.Geography.Tests;
using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    /// <summary>
    /// Class UtmProjectionTests.
    /// </summary>
    public class UtmProjectionTests
    {
        /// <summary>
        /// The utm
        /// </summary>
        private readonly UtmProjection utm = new UtmProjection();

        /// <summary>
        /// Defines the test method TestMyHome.
        /// </summary>
        [Fact]
        public void TestMyHome()
        {
            var e = (UtmCoordinate)utm.ToEuclidian(Constants.MyHome);
            // Reference Computation from http://www.earthpoint.us/Convert.aspx
            Assert.Equal("32U 485577 5521521", e.ToString());
        }

        /// <summary>
        /// Defines the test method TestInversion.
        /// </summary>
        [Fact]
        public void TestInversion()
        {
            var e = (UtmCoordinate)utm.ToEuclidian(Constants.MyHome);
            var c = utm.FromEuclidian(e);
            Assert.True(c.IsApproximatelyEqual(Constants.MyHome, 0.000000001));
        }

        /// <summary>
        /// Defines the test method TestEquals1.
        /// </summary>
        [Fact]
        public void TestEquals1()
        {
            var s = "123";
            Assert.False(utm.Equals(s));
        }

        /// <summary>
        /// Defines the test method TestEquals2.
        /// </summary>
        [Fact]
        public void TestEquals2()
        {
            var utm2 = new UtmProjection();
        }
    }
}
