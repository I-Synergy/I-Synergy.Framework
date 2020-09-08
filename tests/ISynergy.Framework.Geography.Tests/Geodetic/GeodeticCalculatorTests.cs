using System;
using ISynergy.Framework.Geography.Tests;
using Xunit;

namespace ISynergy.Framework.Geography.Geodetic.Tests
{
    public class GeodeticCalculatorTests
    {
        private readonly GeodeticCalculator calc = new GeodeticCalculator(Ellipsoid.WGS84);

        [Fact]
        public void TestCurve()
        {
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            // Reference values computed with 
            // http://williams.best.vwh.net/gccalc.htm
            Assert.Equal(43232.317, Math.Round(1000 * curve.EllipsoidalDistance) / 1000);
            Assert.Equal(342.302315, Math.Round(1000000 * curve.Azimuth.Degrees) / 1000000);
            Assert.Equal(curve.Calculator, calc);
        }

        [Fact]
        public void TestCurve2()
        {
            var target = new GlobalCoordinates(Constants.MyHome.Latitude, Constants.MyHome.Longitude - 1.0);
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, target);
            // Reference values computed with 
            // http://williams.best.vwh.net/gccalc.htm
            Assert.Equal(270.382160, Math.Round(100000 * curve.Azimuth.Degrees) / 100000);
            Assert.Equal(curve.Calculator, calc);
        }

        [Fact]
        public void TestMeasurement()
        {
            var start = new GlobalPosition(Constants.MyHome, 200);
            var end = new GlobalPosition(Constants.MyOffice, 240);
            var m = calc.CalculateGeodeticMeasurement(start, end);
            var c = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            Assert.True(m.EllipsoidalDistance > c.EllipsoidalDistance);
        }

        [Fact]
        public void TestNearAntipodicCurve()
        {
            var loc = new GlobalCoordinates(0, 10); // on the equator
            var aloc = loc.Antipode;
            aloc.Latitude *= 0.99999998;
            var curve = calc.CalculateGeodeticCurve(loc, aloc);
            Assert.True(double.IsNaN(curve.Azimuth.Degrees));
            Assert.Equal(curve.Calculator, calc);
        }

        [Fact]
        public void TestEnding()
        {
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            var final = calc.CalculateEndingGlobalCoordinates(
                Constants.MyHome,
                curve.Azimuth,
                curve.EllipsoidalDistance);
            Assert.Equal(final, Constants.MyOffice);
            Assert.Equal(curve.Calculator, calc);
        }

        [Fact]
        public void TestEndingNegDist()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => calc.CalculateEndingGlobalCoordinates(
                Constants.MyHome,
                0.0,
                -1.0));
        }

        [Fact]
        public void TestEndingZeroDist()
        {
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            var final = calc.CalculateEndingGlobalCoordinates(
                Constants.MyHome,
                curve.Azimuth,
                0.0);
            Assert.Equal(final, Constants.MyHome);
            Assert.Equal(curve.Calculator, calc);
        }

        [Fact]
        public void TestPath1()
        {
            var path = calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyOffice, 2);
            Assert.Equal(path[0], Constants.MyHome);
            Assert.Equal(path[1], Constants.MyOffice);
            Assert.Equal(calc.ReferenceGlobe, Ellipsoid.WGS84);
        }

        [Fact]
        public void TestPath2()
        {
            var path = calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyOffice);
            Assert.Equal(path[0], Constants.MyHome);
            Assert.Equal(path[path.Length - 1], Constants.MyOffice);
            Assert.Equal(calc.ReferenceGlobe, Ellipsoid.WGS84);
        }

        [Fact]
        public void TestPath3()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyOffice, 1));
        }

        [Fact]
        public void TestPath4()
        {
            var path = calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyHome, 10);
            Assert.Equal(2, path.Length);
            Assert.Equal(path[0], Constants.MyHome);
            Assert.Equal(path[1], Constants.MyHome);
        }
    }
}
