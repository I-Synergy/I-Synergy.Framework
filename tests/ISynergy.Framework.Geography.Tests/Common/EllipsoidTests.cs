using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Common.Tests;

/// <summary>
/// Class EllipsoidTests.
/// </summary>
[TestClass]
public class EllipsoidTests
{
    /// <summary>
    /// Defines the test method TestFactory1.
    /// </summary>
    [TestMethod]
    public void TestFactory1()
    {
        Ellipsoid e = Ellipsoid.FromAAndF(100000, 0.01);
        Assert.AreEqual(e.SemiMinorAxis, (1 - 0.01) * 100000);
        Assert.AreEqual(100, e.InverseFlattening);
        Assert.AreEqual(100000, e.SemiMajorAxis);
        Assert.AreEqual(e.Ratio, 1.0 - 0.01);
    }

    /// <summary>
    /// Defines the test method TestFactory2.
    /// </summary>
    [TestMethod]
    public void TestFactory2()
    {
        Ellipsoid e = Ellipsoid.FromAAndInverseF(100000, 100);
        Assert.AreEqual(e.SemiMinorAxis, (1 - 0.01) * 100000);
        Assert.AreEqual(0.01, e.Flattening);
        Assert.AreEqual(100000, e.SemiMajorAxis);
        Assert.AreEqual(e.Ratio, 1.0 - 0.01);
    }

    /// <summary>
    /// Defines the test method TestEquality.
    /// </summary>
    [TestMethod]
    public void TestEquality()
    {
        Ellipsoid e1 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e2 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e3 = Ellipsoid.FromAAndInverseF(100000, 101);
        Ellipsoid e4 = Ellipsoid.FromAAndInverseF(100000, 100 + 1e-13);
        Ellipsoid e5 = Ellipsoid.FromAAndInverseF(99000, 100);
        Assert.IsTrue(e1 == e2);
        Assert.IsFalse(e1 == e3);
        Assert.IsTrue(e1 == e4);
        Assert.IsFalse(e1 == e5);
    }

    /// <summary>
    /// Defines the test method TestInEquality.
    /// </summary>
    [TestMethod]
    public void TestInEquality()
    {
        Ellipsoid e1 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e2 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e3 = Ellipsoid.FromAAndInverseF(100000, 101);
        Ellipsoid e4 = Ellipsoid.FromAAndInverseF(100000, 100 + 1e-13);
        Assert.IsFalse(e1 != e2);
        Assert.IsTrue(e1 != e3);
        Assert.IsFalse(e1 != e4);
    }

    /// <summary>
    /// Defines the test method TestEquals.
    /// </summary>
    [TestMethod]
    public void TestEquals()
    {
        Ellipsoid e1 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e2 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e3 = Ellipsoid.FromAAndInverseF(100000, 101);
        Ellipsoid e4 = Ellipsoid.FromAAndInverseF(100000, 100 + 1e-13);
        object s = "123";
        Assert.IsTrue(e1.Equals(e2));
        Assert.IsFalse(e1.Equals(e3));
        Assert.IsTrue(e1.Equals(e4));
        Assert.IsFalse(e1.Equals(null));
        Assert.IsFalse(e1.Equals(s));
    }

    /// <summary>
    /// Defines the test method TestHashCode.
    /// </summary>
    [TestMethod]
    public void TestHashCode()
    {
        Ellipsoid e1 = Ellipsoid.FromAAndInverseF(100000, 100);
        Ellipsoid e2 = Ellipsoid.FromAAndInverseF(100000, 101);
        Assert.AreNotEqual(e1.GetHashCode(), e2.GetHashCode());
    }

    /// <summary>
    /// Defines the test method TestEccentricity.
    /// </summary>
    [TestMethod]
    public void TestEccentricity()
    {
        Ellipsoid e = Ellipsoid.WGS84;
        Assert.IsTrue(e.Eccentricity.IsApproximatelyEqual(0.081819190842621));
    }
}
