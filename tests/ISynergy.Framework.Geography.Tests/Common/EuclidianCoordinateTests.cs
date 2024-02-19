using ISynergy.Framework.Geography.Global;
using ISynergy.Framework.Geography.Projection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Common.Tests;

/// <summary>
/// Class EuclidianCoordinateTests.
/// </summary>
[TestClass]
public class EuclidianCoordinateTests
{
    /// <summary>
    /// The projection
    /// </summary>
    private readonly GlobalMercatorProjection projection = new SphericalMercatorProjection();

    /// <summary>
    /// Defines the test method TestConstructor1.
    /// </summary>
    [TestMethod]
    public void TestConstructor1()
    {
        EuclidianCoordinate e = new(projection, -1, -2);
        Assert.AreEqual(e.X, -1);
        Assert.AreEqual(e.Y, -2);
    }

    /// <summary>
    /// Defines the test method TestConstructor2.
    /// </summary>
    [TestMethod]
    public void TestConstructor2()
    {
        EuclidianCoordinate e = new(projection, new double[] { -3, -4 });
        Assert.AreEqual(e.X, -3);
        Assert.AreEqual(e.Y, -4);
    }

    /// <summary>
    /// Defines the test method TestConstructor3.
    /// </summary>
    [TestMethod]
    public void TestConstructor3()
    {
        Assert.ThrowsException<IndexOutOfRangeException>(() => new EuclidianCoordinate(projection, new double[] { -3, -4, -5 }));
    }

    /// <summary>
    /// Defines the test method TestEquals1.
    /// </summary>
    [TestMethod]
    public void TestEquals1()
    {
        EuclidianCoordinate e1 = new(projection, -3, -4);
        EuclidianCoordinate e2 = new(projection, -3, -4);
        Assert.AreEqual(e1, e2);
    }

    ///// <summary>
    ///// Defines the test method TestEquals2.
    ///// </summary>
    ///// [TestMethod]
    //public void TestEquals2()
    //{
    //    var e1 = new EuclidianCoordinate(projection, -3, -4);
    //    var e2 = "123";
    //    Assert.AreNotSame(e1, e2);
    //}

    /// <summary>
    /// Defines the test method TestEquals3.
    /// </summary>
    [TestMethod]
    public void TestEquals3()
    {
        EuclidianCoordinate e1 = new(projection, -3, -4);
        EuclidianCoordinate e2 = new(new EllipticalMercatorProjection(), -3, -4);
        Assert.AreNotEqual(e1, e2);
    }

    /// <summary>
    /// Defines the test method TestEquals4.
    /// </summary>
    [TestMethod]
    public void TestEquals4()
    {
        EuclidianCoordinate e1 = new(projection, -3, -4);
        EuclidianCoordinate e2 = new(projection, -3 + 1e-13, -4);
        Assert.AreEqual(e1, e2);
    }

    /// <summary>
    /// Defines the test method TestEquals5.
    /// </summary>
    [TestMethod]
    public void TestEquals5()
    {
        EuclidianCoordinate e1 = new(projection, -3, -4);
        EuclidianCoordinate e2 = new(projection, -3, -4 + 1e-13);
        Assert.AreEqual(e1, e2);
    }

    /// <summary>
    /// Defines the test method TestHash.
    /// </summary>
    [TestMethod]
    public void TestHash()
    {
        EuclidianCoordinate e1 = new(projection, -3, -4);
        EuclidianCoordinate e2 = new(projection, -3, -4 + 1e-13);
        Assert.AreNotEqual(e1.GetHashCode(), e2.GetHashCode());
    }

    /// <summary>
    /// Defines the test method TestNotSameProjection.
    /// </summary>
    [TestMethod]
    public void TestNotSameProjection()
    {
        EuclidianCoordinate e1 = new(projection, -3, -4);
        EuclidianCoordinate e2 = new(new EllipticalMercatorProjection(), -3, -4);
        Assert.ThrowsException<ArgumentException>(() => e1.DistanceTo(e2));
    }
}
