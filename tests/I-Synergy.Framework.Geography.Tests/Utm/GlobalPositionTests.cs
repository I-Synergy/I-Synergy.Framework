using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    public class GlobalPositionTests
    {
        GlobalCoordinates c1 = new GlobalCoordinates(45, 9);
        GlobalCoordinates c2 = new GlobalCoordinates(45, 10);
        GlobalCoordinates c3 = new GlobalCoordinates(46, 9);
        GlobalCoordinates c4 = new GlobalCoordinates(44, 9);

        [Fact]
        public void TestConstructor1()
        {
            var a = new GlobalPosition();
            Assert.Equal(0, a.Latitude);
            Assert.Equal(0, a.Longitude);
            Assert.Equal(0, a.Elevation);
        }

        [Fact]
        public void TestConstructor2()
        {
            var a = new GlobalPosition(c1, 100);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(100, a.Elevation);
        }

        [Fact]
        public void TestConstructor3()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
        }

        [Fact]
        public void TestCoordSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Coordinates = c2;
            Assert.Equal(a.Coordinates, c2);
        }

        [Fact]
        public void TestLatSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Latitude = 46;
            Assert.Equal(a.Coordinates, c3);
        }

        [Fact]
        public void TestLongSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Longitude = 10;
            Assert.Equal(a.Coordinates, c2);
        }

        [Fact]
        public void TestElevSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.Equal(a.Coordinates, c1);
            Assert.Equal(0, a.Elevation);
            a.Elevation = -100;
            Assert.Equal(a.Elevation, -100);
        }

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

        [Fact]
        public void TestToString()
        {
            var a = new GlobalPosition(c1, 200);
            Assert.Equal("45N;9E;200m", a.ToString());
        }

        [Fact]
        public void TestGetHash()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c2);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void TestGetHash2()
        {
            var a = new GlobalPosition(c1, 100);
            var b = new GlobalPosition(c2, -100);
            Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
        }

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
