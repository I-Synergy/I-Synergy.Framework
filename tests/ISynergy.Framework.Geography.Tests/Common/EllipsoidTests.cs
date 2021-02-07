using ISynergy.Framework.Core.Extensions;
using Xunit;

namespace ISynergy.Framework.Geography.Common.Tests
{
    /// <summary>
    /// Class EllipsoidTests.
    /// </summary>
    public class EllipsoidTests
    {
        /// <summary>
        /// Defines the test method TestFactory1.
        /// </summary>
        [Fact]
        public void TestFactory1()
        {
            var e = Ellipsoid.FromAAndF(100000, 0.01);
            Assert.Equal(e.SemiMinorAxis, (1 - 0.01) * 100000);
            Assert.Equal(100, e.InverseFlattening);
            Assert.Equal(100000, e.SemiMajorAxis);
            Assert.Equal(e.Ratio, 1.0 - 0.01);
        }

        /// <summary>
        /// Defines the test method TestFactory2.
        /// </summary>
        [Fact]
        public void TestFactory2()
        {
            var e = Ellipsoid.FromAAndInverseF(100000, 100);
            Assert.Equal(e.SemiMinorAxis, (1 - 0.01) * 100000);
            Assert.Equal(0.01, e.Flattening);
            Assert.Equal(100000, e.SemiMajorAxis);
            Assert.Equal(e.Ratio, 1.0 - 0.01);
        }

        /// <summary>
        /// Defines the test method TestEquality.
        /// </summary>
        [Fact]
        public void TestEquality()
        {
            var e1 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e2 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e3 = Ellipsoid.FromAAndInverseF(100000, 101);
            var e4 = Ellipsoid.FromAAndInverseF(100000, 100 + 1e-13);
            var e5 = Ellipsoid.FromAAndInverseF(99000, 100);
            Assert.True(e1 == e2);
            Assert.False(e1 == e3);
            Assert.True(e1 == e4);
            Assert.False(e1 == e5);
        }

        /// <summary>
        /// Defines the test method TestInEquality.
        /// </summary>
        [Fact]
        public void TestInEquality()
        {
            var e1 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e2 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e3 = Ellipsoid.FromAAndInverseF(100000, 101);
            var e4 = Ellipsoid.FromAAndInverseF(100000, 100 + 1e-13);
            Assert.False(e1 != e2);
            Assert.True(e1 != e3);
            Assert.False(e1 != e4);
        }

        /// <summary>
        /// Defines the test method TestEquals.
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var e1 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e2 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e3 = Ellipsoid.FromAAndInverseF(100000, 101);
            var e4 = Ellipsoid.FromAAndInverseF(100000, 100 + 1e-13);
            object s = "123";
            Assert.True(e1.Equals(e2));
            Assert.False(e1.Equals(e3));
            Assert.True(e1.Equals(e4));
            Assert.False(e1.Equals(null));
            Assert.False(e1.Equals(s));
        }

        /// <summary>
        /// Defines the test method TestHashCode.
        /// </summary>
        [Fact]
        public void TestHashCode()
        {
            var e1 = Ellipsoid.FromAAndInverseF(100000, 100);
            var e2 = Ellipsoid.FromAAndInverseF(100000, 101);
            Assert.NotEqual(e1.GetHashCode(), e2.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestEccentricity.
        /// </summary>
        [Fact]
        public void TestEccentricity()
        {
            var e = Ellipsoid.WGS84;
            Assert.True(e.Eccentricity.IsApproximatelyEqual(0.081819190842621));
        }
    }
}
