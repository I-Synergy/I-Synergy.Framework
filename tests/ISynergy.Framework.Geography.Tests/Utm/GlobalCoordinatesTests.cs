using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    /// <summary>
    /// Class GlobalCoordinatesTests.
    /// </summary>
    public class GlobalCoordinatesTests
    {
        /// <summary>
        /// Defines the test method TestConstructor1.
        /// </summary>
        [Fact]
        public void TestConstructor1()
        {
            var g = new GlobalCoordinates(45, 9);
            Assert.Equal(45, g.Latitude.Degrees);
            Assert.Equal(9, g.Longitude.Degrees);
        }

        /// <summary>
        /// Defines the test method TestConstructor2.
        /// </summary>
        [Fact]
        public void TestConstructor2()
        {
            var g = new GlobalCoordinates(-181, 9);
            Assert.Equal(1, g.Latitude.Degrees);
            Assert.Equal(g.Longitude.Degrees, -171);
        }

        /// <summary>
        /// Defines the test method TestConstructor3.
        /// </summary>
        [Fact]
        public void TestConstructor3()
        {
            var g = new GlobalCoordinates(-811, 0);
            Assert.Equal(g.Latitude.Degrees, -89);
            Assert.Equal(g.Longitude.Degrees, -180);
        }

        /// <summary>
        /// Defines the test method TestConstructor4.
        /// </summary>
        [Fact]
        public void TestConstructor4()
        {
            var g = new GlobalCoordinates(-0, -811);
            Assert.Equal(0, g.Latitude.Degrees);
            Assert.Equal(g.Longitude.Degrees, -91);
        }

        /// <summary>
        /// Defines the test method TestLatitudeSetter.
        /// </summary>
        [Fact]
        public void TestLatitudeSetter()
        {
            var a = new GlobalCoordinates(0.0, 45.0)
            {
                Latitude = 45.0
            };
            Assert.Equal(45, a.Latitude.Degrees);
        }

        /// <summary>
        /// Defines the test method TestLongitudeSetter.
        /// </summary>
        [Fact]
        public void TestLongitudeSetter()
        {
            var a = new GlobalCoordinates(0.0, 45.0)
            {
                Longitude = -10.0
            };
            Assert.Equal(a.Longitude.Degrees, -10);
        }

        /// <summary>
        /// Defines the test method TestCompareTo1.
        /// </summary>
        [Fact]
        public void TestCompareTo1()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.Equal(0, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestCompareTo2.
        /// </summary>
        [Fact]
        public void TestCompareTo2()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(46, 9);
            Assert.Equal(a.CompareTo(b), -1);
        }

        /// <summary>
        /// Defines the test method TestCompareTo3.
        /// </summary>
        [Fact]
        public void TestCompareTo3()
        {
            var a = new GlobalCoordinates(45, 10);
            var b = new GlobalCoordinates(45, 9);
            Assert.Equal(1, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestCompareTo4.
        /// </summary>
        [Fact]
        public void TestCompareTo4()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 10);
            Assert.Equal(a.CompareTo(b), -1);
        }

        /// <summary>
        /// Defines the test method TestCompareTo5.
        /// </summary>
        [Fact]
        public void TestCompareTo5()
        {
            var a = new GlobalCoordinates(44, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.Equal(a.CompareTo(b), -1);
        }

        /// <summary>
        /// Defines the test method TestCompareTo6.
        /// </summary>
        [Fact]
        public void TestCompareTo6()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(44, 9);
            Assert.Equal(1, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestEquals.
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var a = new GlobalCoordinates(45, 9);
            Assert.False(a.Equals(null));
            object s = "x";
            Assert.False(a.Equals(s));
            var b = new GlobalCoordinates(45, 9);
            Assert.True(a.Equals(b));
            b.Longitude += 1;
            Assert.False(a.Equals(b));
        }

        /// <summary>
        /// Defines the test method TestToString.
        /// </summary>
        [Fact]
        public void TestToString()
        {
            var a = new GlobalCoordinates(45, 9);
            Assert.Equal("45N;9E;", a.ToString());
            var b = new GlobalCoordinates(-45, -9);
            Assert.Equal("45S;9W;", b.ToString());
        }

        /// <summary>
        /// Defines the test method TestGetHash.
        /// </summary>
        [Fact]
        public void TestGetHash()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9.000000001);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestEquality.
        /// </summary>
        [Fact]
        public void TestEquality()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.True(a == b);
            b.Longitude += 1e-13;
            Assert.True(a == b);
            b.Longitude += 0.00001;
            Assert.False(a == b);
        }

        /// <summary>
        /// Defines the test method TestInEquality.
        /// </summary>
        [Fact]
        public void TestInEquality()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.False(a != b);
            b.Longitude += 1e-13;
            Assert.False(a != b);
            b.Longitude += 0.00001;
            Assert.True(a != b);
        }

        /// <summary>
        /// Defines the test method TestGreater.
        /// </summary>
        [Fact]
        public void TestGreater()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 8);
            Assert.True(a > b);
            b.Longitude = a.Longitude + 1e-13;
            Assert.False(a > b);
            b.Longitude = a.Longitude + 0.00001;
            Assert.False(a > b);
        }

        /// <summary>
        /// Defines the test method TestGreaterEqual.
        /// </summary>
        [Fact]
        public void TestGreaterEqual()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 8);
            Assert.True(a >= b);
            b.Longitude = a.Longitude + 1e-13;
            Assert.True(a >= b);
            b.Longitude = a.Longitude - 0.00001;
            Assert.True(a > b);
        }

        /// <summary>
        /// Defines the test method TestLess.
        /// </summary>
        [Fact]
        public void TestLess()
        {
            var a = new GlobalCoordinates(45, 8);
            var b = new GlobalCoordinates(45, 9);
            Assert.True(a < b);
            a.Longitude = b.Longitude + 1e-13;
            Assert.False(a < b);
            a.Longitude = b.Longitude + 0.00001;
            Assert.False(a < b);
        }

        /// <summary>
        /// Defines the test method TestLessEqual.
        /// </summary>
        [Fact]
        public void TestLessEqual()
        {
            var a = new GlobalCoordinates(45, 8);
            var b = new GlobalCoordinates(45, 9);
            Assert.True(a <= b);
            a.Longitude = b.Longitude + 1e-13;
            Assert.True(a <= b);
            a.Longitude = b.Longitude + 0.00001;
            Assert.False(a <= b);
        }

        /// <summary>
        /// Defines the test method TestAntipode.
        /// </summary>
        [Fact]
        public void TestAntipode()
        {
            var loc = new GlobalCoordinates(27.97, -82.53);
            var antiloc = new GlobalCoordinates(-27.97, 97.47);
            Assert.Equal(loc.Antipode, antiloc);
        }
    }
}
