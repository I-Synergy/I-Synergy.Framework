using ISynergy.Framework.Geography.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Utm.Tests;

/// <summary>
/// Class GlobalPositionTests.
/// </summary>
[TestClass]
public class GlobalPositionTests
{
    /// <summary>
    /// The c1
    /// </summary>
    GlobalCoordinates c1 = new(45, 9);
    /// <summary>
    /// The c2
    /// </summary>
    GlobalCoordinates c2 = new(45, 10);
    /// <summary>
    /// The c3
    /// </summary>
    GlobalCoordinates c3 = new(46, 9);
    /// <summary>
    /// The c4
    /// </summary>
    GlobalCoordinates c4 = new(44, 9);

    /// <summary>
    /// Defines the test method TestConstructor1.
    /// </summary>
    [TestMethod]
    public void TestConstructor1()
    {
        GlobalPosition a = new();
        Assert.AreEqual(0, a.Latitude);
        Assert.AreEqual(0, a.Longitude);
        Assert.AreEqual(0, a.Elevation);
    }

    /// <summary>
    /// Defines the test method TestConstructor2.
    /// </summary>
    [TestMethod]
    public void TestConstructor2()
    {
        GlobalPosition a = new(c1, 100);
        Assert.AreEqual(a.Coordinates, c1);
        Assert.AreEqual(100, a.Elevation);
    }

    /// <summary>
    /// Defines the test method TestConstructor3.
    /// </summary>
    [TestMethod]
    public void TestConstructor3()
    {
        GlobalPosition a = new(c1);
        Assert.AreEqual(a.Coordinates, c1);
        Assert.AreEqual(0, a.Elevation);
    }

    /// <summary>
    /// Defines the test method TestCoordSetter.
    /// </summary>
    [TestMethod]
    public void TestCoordSetter()
    {
        GlobalPosition a = new(c1);
        Assert.AreEqual(a.Coordinates, c1);
        Assert.AreEqual(0, a.Elevation);
        a.Coordinates = c2;
        Assert.AreEqual(a.Coordinates, c2);
    }

    /// <summary>
    /// Defines the test method TestLatSetter.
    /// </summary>
    [TestMethod]
    public void TestLatSetter()
    {
        GlobalPosition a = new(c1);
        Assert.AreEqual(a.Coordinates, c1);
        Assert.AreEqual(0, a.Elevation);
        a.Latitude = 46;
        Assert.AreEqual(a.Coordinates, c3);
    }

    /// <summary>
    /// Defines the test method TestLongSetter.
    /// </summary>
    [TestMethod]
    public void TestLongSetter()
    {
        GlobalPosition a = new(c1);
        Assert.AreEqual(a.Coordinates, c1);
        Assert.AreEqual(0, a.Elevation);
        a.Longitude = 10;
        Assert.AreEqual(a.Coordinates, c2);
    }

    /// <summary>
    /// Defines the test method TestElevSetter.
    /// </summary>
    [TestMethod]
    public void TestElevSetter()
    {
        GlobalPosition a = new(c1);
        Assert.AreEqual(a.Coordinates, c1);
        Assert.AreEqual(0, a.Elevation);
        a.Elevation = -100;
        Assert.AreEqual(a.Elevation, -100);
    }

    /// <summary>
    /// Defines the test method TestCompareTo1.
    /// </summary>
    [TestMethod]
    public void TestCompareTo1()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c1);
        Assert.AreEqual(0, a.CompareTo(b));
        b.Elevation += 1e-13;
        Assert.AreEqual(0, a.CompareTo(b));
        b.Elevation = 100;
        Assert.AreEqual(a.CompareTo(b), -1);
        b.Elevation = -100;
        Assert.AreEqual(1, a.CompareTo(b));
    }

    /// <summary>
    /// Defines the test method TestEquals.
    /// </summary>
    [TestMethod]
    public void TestEquals()
    {
        GlobalPosition a = new(c1);
        Assert.IsFalse(a.Equals(null));
        object s = "x";
        Assert.IsFalse(a.Equals(s));
        GlobalPosition b = new(c1);
        Assert.IsTrue(a.Equals(b));
        b.Elevation += 1;
        Assert.IsFalse(a.Equals(b));
    }

    /// <summary>
    /// Defines the test method TestToString.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        GlobalPosition a = new(c1, 200);
        Assert.AreEqual("45N;9E;200m", a.ToString());
    }

    /// <summary>
    /// Defines the test method TestGetHash.
    /// </summary>
    [TestMethod]
    public void TestGetHash()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c2);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    /// <summary>
    /// Defines the test method TestGetHash2.
    /// </summary>
    [TestMethod]
    public void TestGetHash2()
    {
        GlobalPosition a = new(c1, 100);
        GlobalPosition b = new(c2, -100);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    /// <summary>
    /// Defines the test method TestEquality.
    /// </summary>
    [TestMethod]
    public void TestEquality()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c1);
        Assert.IsTrue(a == b);
        b.Elevation += 1e-13;
        Assert.IsTrue(a == b);
        b.Elevation += 0.00001;
        Assert.IsFalse(a == b);
    }

    /// <summary>
    /// Defines the test method TestInEquality.
    /// </summary>
    [TestMethod]
    public void TestInEquality()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c1);
        Assert.IsFalse(a != b);
        b.Elevation += 1e-13;
        Assert.IsFalse(a != b);
        b.Elevation += 0.00001;
        Assert.IsTrue(a != b);
    }

    /// <summary>
    /// Defines the test method TestGreater.
    /// </summary>
    [TestMethod]
    public void TestGreater()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c4);
        Assert.IsTrue(a > b);
        b.Latitude = a.Latitude + 1e-13;
        Assert.IsFalse(a > b);
        a.Elevation = 100;
        Assert.IsTrue(a > b);
        b.Latitude = a.Latitude + 0.00001;
        Assert.IsFalse(a > b);
    }

    /// <summary>
    /// Defines the test method TestGreaterEqual.
    /// </summary>
    [TestMethod]
    public void TestGreaterEqual()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c4);
        Assert.IsTrue(a >= b);
        b.Latitude = a.Latitude + 1e-13;
        Assert.IsTrue(a >= b);
        a.Elevation = 100;
        Assert.IsTrue(a >= b);
        b.Latitude = a.Latitude - 0.00001;
        Assert.IsTrue(a >= b);
    }

    /// <summary>
    /// Defines the test method TestLess.
    /// </summary>
    [TestMethod]
    public void TestLess()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c4);
        Assert.IsTrue(b < a);
        b.Latitude = a.Latitude + 1e-13;
        Assert.IsFalse(b < a);
        a.Elevation = 100;
        Assert.IsTrue(b < a);
        b.Latitude = a.Latitude + 0.00001;
        Assert.IsFalse(b < a);
    }

    /// <summary>
    /// Defines the test method TestLessEqual.
    /// </summary>
    [TestMethod]
    public void TestLessEqual()
    {
        GlobalPosition a = new(c1);
        GlobalPosition b = new(c4);
        Assert.IsTrue(b <= a);
        b.Latitude = a.Latitude + 1e-13;
        Assert.IsTrue(b <= a);
        a.Elevation = 100;
        Assert.IsTrue(b <= a);
        b.Latitude = a.Latitude - 0.00001;
        Assert.IsTrue(b <= a);
    }
}
