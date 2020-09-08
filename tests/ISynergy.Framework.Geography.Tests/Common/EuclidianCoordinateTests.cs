using System;
using Xunit;

namespace ISynergy.Framework.Geography.Common.Tests
{
    public class EuclidianCoordinateTests
    {
        private readonly GlobalMercatorProjection projection = new SphericalMercatorProjection();

        [Fact]
        public void TestConstructor1()
        {
            var e = new EuclidianCoordinate(projection, -1, -2);
            Assert.Equal(e.X, -1);
            Assert.Equal(e.Y, -2);
        }

        [Fact]
        public void TestConstructor2()
        {
            var e = new EuclidianCoordinate(projection, new double[] { -3, -4 });
            Assert.Equal(e.X, -3);
            Assert.Equal(e.Y, -4);
        }

        [Fact]
        public void TestConstructor3()
        {
            Assert.Throws<IndexOutOfRangeException>(() => new EuclidianCoordinate(projection, new double[] { -3, -4, -5 }));
        }

        [Fact]
        public void TestEquals1()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = new EuclidianCoordinate(projection, -3, -4);
            Assert.Equal(e1, e2);
        }

        [Fact]
        public void TestEquals2()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = "123";
            Assert.NotSame(e1, e2);
        }

        [Fact]
        public void TestEquals3()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = new EuclidianCoordinate(new EllipticalMercatorProjection(), -3, -4);
            Assert.NotEqual(e1, e2);
        }

        [Fact]
        public void TestEquals4()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = new EuclidianCoordinate(projection, -3 + 1e-13, -4);
            Assert.Equal(e1, e2);
        }

        [Fact]
        public void TestEquals5()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = new EuclidianCoordinate(projection, -3, -4 + 1e-13);
            Assert.Equal(e1, e2);
        }

        [Fact]
        public void TestHash()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = new EuclidianCoordinate(projection, -3, -4 + 1e-13);
            Assert.NotEqual(e1.GetHashCode(), e2.GetHashCode());
        }

        [Fact]
        public void TestNotSameProjection()
        {
            var e1 = new EuclidianCoordinate(projection, -3, -4);
            var e2 = new EuclidianCoordinate(new EllipticalMercatorProjection(), -3, -4);
            Assert.Throws<ArgumentException>(() => e1.DistanceTo(e2));
        }
    }
}
