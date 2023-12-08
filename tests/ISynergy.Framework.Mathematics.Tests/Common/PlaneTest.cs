namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

[TestClass]
public class PlaneTest
{

    [TestMethod]
    public void FromPointsTest()
    {
        Point3 point1 = new(0, 1, -7);
        Point3 point2 = new(3, 1, -9);
        Point3 point3 = new(0, -5, -8);

        Plane actual = Plane.FromPoints(point1, point2, point3);
        Vector3 expected = new(-12, 3, -18);

        Assert.AreEqual(expected, actual.Normal);
    }

    [TestMethod]
    public void FromPointsTest2()
    {
        Point3 point1 = new(1, 2, -2);
        Point3 point2 = new(3, -2, 1);
        Point3 point3 = new(5, 1, -4);

        Plane expected = new(11, 16, 14, -15);
        Plane actual = Plane.FromPoints(point1, point2, point3);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToStringTest()
    {
        Plane target = new(-12, 3, -18, 1);

        {
            string expected = "-12x +3y -18z +1 = 0";
            string actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        {
            string expected = "x = +0.25y -1.5z +0.083333336";
            string actual = target.ToString('x', CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, actual);
        }
    }
}
