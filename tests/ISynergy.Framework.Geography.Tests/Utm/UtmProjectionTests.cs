using ISynergy.Framework.Geography.Tests;
using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    public class UtmProjectionTests
    {
        private readonly UtmProjection utm = new UtmProjection();

        [Fact]
        public void TestMyHome()
        {
            var e = (UtmCoordinate)utm.ToEuclidian(Constants.MyHome);
            // Reference Computation from http://www.earthpoint.us/Convert.aspx
            Assert.Equal("32U 485577 5521521", e.ToString());
        }

        [Fact]
        public void TestInversion()
        {
            var e = (UtmCoordinate)utm.ToEuclidian(Constants.MyHome);
            var c = utm.FromEuclidian(e);
            Assert.True(c.IsApproximatelyEqual(Constants.MyHome, 0.000000001));
        }

        [Fact]
        public void TestEquals1()
        {
            var s = "123";
            Assert.False(utm.Equals(s));
        }

        [Fact]
        public void TestEquals2()
        {
            var utm2 = new UtmProjection();
        }
    }
}
