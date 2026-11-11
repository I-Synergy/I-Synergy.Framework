using ISynergy.Framework.Geography.Common;
using ISynergy.Framework.Geography.Global;

namespace ISynergy.Framework.Geography.Geodetic;

/// <summary>
/// Tests for the branch of <see cref="GeodeticCalculator.CalculateGeodeticCurve"/> that handles
/// curves which do not converge (points on the same meridian), for the azimuth normalisation of the
/// converged branch, and for the equality members of the calculator.
/// </summary>
[TestClass]
public class GeodeticCalculatorBranchTests
{
    private readonly GeodeticCalculator _calculator = new(Ellipsoid.WGS84);

    #region Non-converged (same meridians)

    [TestMethod]
    public void CalculateGeodeticCurve_OnSameMeridianHeadedSouth_Returns180DegreeBearing()
    {
        // Arrange - the iterative solution cannot converge when both points share a meridian
        GlobalCoordinates start = new(10, 0);
        GlobalCoordinates end = new(-10, 0);

        // Act
        var curve = _calculator.CalculateGeodeticCurve(start, end);

        // Assert - going south means the initial bearing is 180 degrees
        Assert.AreEqual(180.0, curve.Azimuth.Degrees, 1e-9);
        Assert.AreEqual(180.0, Angle.Angle180.Degrees, 1e-9);
    }

    [TestMethod]
    public void CalculateGeodeticCurve_OnSameMeridianHeadedNorth_ReturnsZeroBearing()
    {
        // Arrange
        GlobalCoordinates start = new(-10, 0);
        GlobalCoordinates end = new(10, 0);

        // Act
        var curve = _calculator.CalculateGeodeticCurve(start, end);

        // Assert - going north means the initial bearing is 0 degrees
        Assert.AreEqual(Angle.Zero.Degrees, curve.Azimuth.Degrees, 1e-9);
    }

    [TestMethod]
    public void CalculateGeodeticCurve_OnSameMeridianAtSameLatitude_ReturnsNaN()
    {
        // Arrange
        GlobalCoordinates start = new(0, 0);
        GlobalCoordinates end = new(0, 0);

        // Act
        var curve = _calculator.CalculateGeodeticCurve(start, end);

        // Assert - neither north nor south: the bearing is undefined
        Assert.IsTrue(double.IsNaN(curve.Azimuth.Degrees));
    }

    #endregion

    #region Converged azimuths

    [TestMethod]
    [DataRow(0.0, 1.0, 90.0)]
    [DataRow(0.0, -1.0, 270.0)]
    [DataRow(1.0, 0.0, 0.0)]
    [DataRow(-1.0, 0.0, 180.0)]
    public void CalculateGeodeticCurve_ForConvergingPoints_ReturnsNormalisedBearing(
        double latitudeOffset,
        double longitudeOffset,
        double expectedBearing)
    {
        // Arrange
        GlobalCoordinates start = new(45, 0);
        GlobalCoordinates end = new(45 + latitudeOffset, longitudeOffset);

        // Act
        var curve = _calculator.CalculateGeodeticCurve(start, end);

        // Assert - the bearing is normalised into [0, 360)
        Assert.IsGreaterThanOrEqualTo(0.0, curve.Azimuth.Degrees);
        Assert.IsLessThan(360.0, curve.Azimuth.Degrees);
        Assert.AreEqual(expectedBearing, curve.Azimuth.Degrees, 1.0);
    }

    [TestMethod]
    public void CalculateGeodeticCurve_ReturnsPositiveDistance()
    {
        // Arrange
        GlobalCoordinates start = new(52.0, 4.0);
        GlobalCoordinates end = new(52.0, 4.5);

        // Act
        var curve = _calculator.CalculateGeodeticCurve(start, end);

        // Assert
        Assert.IsGreaterThan(0.0, curve.EllipsoidalDistance);
        Assert.AreEqual(_calculator.ReferenceGlobe, curve.Calculator.ReferenceGlobe);
    }

    #endregion

    #region Equality members

    [TestMethod]
    public void Equals_SameReferenceGlobe_IsTrue()
    {
        // Arrange
        var other = new GeodeticCalculator(Ellipsoid.WGS84);

        // Act & Assert
        Assert.IsTrue(_calculator.Equals(other));
        Assert.IsTrue(_calculator.Equals((object)other));
    }

    [TestMethod]
    public void Equals_DifferentReferenceGlobe_IsFalse()
    {
        // Arrange
        var other = new GeodeticCalculator(Ellipsoid.GRS80);

        // Act & Assert
        Assert.IsFalse(_calculator.Equals(other));
        Assert.IsFalse(_calculator.Equals((object)other));
    }

    [TestMethod]
    public void Equals_WithNonCalculatorObject_IsFalse()
    {
        Assert.IsFalse(_calculator.Equals("not a calculator"));
        Assert.IsFalse(_calculator.Equals(null));
    }

    [TestMethod]
    public void EqualityOperators_CompareReferenceGlobe()
    {
        // Arrange
        var same = new GeodeticCalculator(Ellipsoid.WGS84);
        var different = new GeodeticCalculator(Ellipsoid.GRS80);

        // Act & Assert
        Assert.IsTrue(_calculator == same);
        Assert.IsFalse(_calculator != same);
        Assert.IsFalse(_calculator == different);
        Assert.IsTrue(_calculator != different);
    }

    #endregion
}
