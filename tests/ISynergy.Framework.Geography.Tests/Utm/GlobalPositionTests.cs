using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    /// <summary>
    /// Class GlobalPositionTests.
    /// </summary>
    public class GlobalPositionTests
    {
        /// <summary>
        /// The c1
        /// </summary>
        GlobalCoordinates c1 = new GlobalCoordinates(45, 9);
        /// <summary>
        /// The c2
        /// </summary>
        GlobalCoordinates c2 = new GlobalCoordinates(45, 10);
        /// <summary>
        /// The c3
        /// </summary>
        GlobalCoordinates c3 = new GlobalCoordinates(46, 9);
        /// <summary>
        /// The c4
        /// </summary>
        GlobalCoordinates c4 = new GlobalCoordinates(44, 9);

        /// <summary>
        /// Defines the test method TestConstructor1.
        /// </summary>
        [Fact]
        public void TestConstructor1()
        {
            var a = new GlobalPosition();
            Assert.Equal(0, a.Latitude);
            Assert.Equal(0, a.Longitude);
            Assert.Equal(0, a.Elevation);
        }

        /// <summary>
        /// Defines the test method TestConstructor2.
        /// </summary>
        [Fact]
        public void TestConstructor2()
        {
            var a = new GlobalPosition(c1, 100);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(100, a.Elevation);
        }

        /// <summary>
        /// Defines the test method TestConstructor3.
        /// </summary>
        [Fact]
        public void TestConstructor3()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
        }

        /// <summary>
        /// Defines the test method TestCoordSetter.
        /// </summary>
        [Fact]
        public void TestCoordSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Coordinates = c2;
            Assert.Equal(a.Coordinates, c2);
        }

        /// <summary>
        /// Defines the test method TestLatSetter.
        /// </summary>
        [Fact]
        public void TestLatSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Latitude = 46;
            Assert.Equal(a.Coordinates, c3);
        }

        /// <summary>
        /// Defines the test method TestLongSetter.
        /// </summary>
        [Fact]
        public void TestLongSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Longitude = 10;
            Assert.Equal(a.Coordinates, c2);
        }

        /// <summary>
        /// Defines the test method TestElevSetter.
        /// </summary>
        [Fact]
        public void TestElevSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Elevation = -100;
            Assert.Equal(a.Elevation, -100);
        }

        /// <summary>
        /// Defines the test method TestCompareTo1.
        /// </summary>
        [Fact]
        public void TestCompareTo1()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c1);
            Assert.Equal(0, a.CompareTo(b));
            b.Elevation += 1e-13;
            Assert.Equal(0, a.CompareTo(b));
            b.Elevation = 100;
            Assert.Equal(a.CompareTo(b), -1);
            b.Elevation = -100;
            Assert.Equal(1, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestEquals.
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var a = new GlobalPosition(c1);
            Assert.False(a.Equals(null));
            object s = "x";
            Assert.False(a.Equals(s));
            var b = new GlobalPosition(c1);
            Assert.True(a.Equals(b));
            b.Elevation += 1;
            Assert.False(a.Equals(b));
        }

        /// <summary>
        /// Defines the test method TestToString.
        /// </summary>
        [Fact]
        public void TestToString()
        {
            var a = new GlobalPosition(c1, 200);
            Assert.Equal("45N;9E;200m", a.ToString());
        }

        /// <summary>
        /// Defines the test method TestGetHash.
        /// </summary>
        [Fact]
        public void TestGetHash()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c2);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestGetHash2.
        /// </summary>
        [Fact]
        public void TestGetHash2()
        {
            var a = new GlobalPosition(c1, 100);
            var b = new GlobalPosition(c2, -100);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestEquality.
        /// </summary>
        [Fact]
        public void TestEquality()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c1);
            Assert.True(a == b);
            b.Elevation += 1e-13;
            Assert.True(a == b);
            b.Elevation += 0.00001;
            Assert.False(a == b);
        }

        /// <summary>
        /// Defines the test method TestInEquality.
        /// </summary>
        [Fact]
        public void TestInEquality()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c1);
            Assert.False(a != b);
            b.Elevation += 1e-13;
            Assert.False(a != b);
            b.Elevation += 0.00001;
            Assert.True(a != b);
        }

        /// <summary>
        /// Defines the test method TestGreater.
        /// </summary>
        [Fact]
        public void TestGreater()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.True(a > b);
            b.Latitude = a.Latitude + 1e-13;
            Assert.False(a > b);
            a.Elevation = 100;
            Assert.True(a > b);
            b.Latitude = a.Latitude + 0.00001;
            Assert.False(a > b);
        }

        /// <summary>
        /// Defines the test method TestGreaterEqual.
        /// </summary>
        [Fact]
        public void TestGreaterEqual()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.True(a >= b);
            b.Latitude = a.Latitude + 1e-13;
            Assert.True(a >= b);
            a.Elevation = 100;
            Assert.True(a >= b);
            b.Latitude = a.Latitude - 0.00001;
            Assert.True(a >= b);
        }

        /// <summary>
        /// Defines the test method TestLess.
        /// </summary>
        [Fact]
        public void TestLess()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.True(b < a);
            b.Latitude = a.Latitude + 1e-13;
            Assert.False(b < a);
            a.Elevation = 100;
            Assert.True(b < a);
            b.Latitude = a.Latitude + 0.00001;
            Assert.False(b < a);
        }

        /// <summary>
        /// Defines the test method TestLessEqual.
        /// </summary>
        [Fact]
        public void TestLessEqual()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.True(b <= a);
            b.Latitude = a.Latitude + 1e-13;
            Assert.True(b <= a);
            a.Elevation = 100;
            Assert.True(b <= a);
            b.Latitude = a.Latitude - 0.00001;
            Assert.True(b <= a);
        }
    }
}
