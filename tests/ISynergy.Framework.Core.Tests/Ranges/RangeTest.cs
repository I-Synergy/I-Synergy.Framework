using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

namespace ISynergy.Framework.Core.Ranges;

[TestClass]
public class RangeTest
{
    private static readonly double[] EnumeratedValues = [0.0, 1.0, 2.0];

    [TestMethod]
    [DataRow(0, 1, 1, 2, true)]
    [DataRow(0, 1, 2, 3, false)]
    [DataRow(0, 10, 2, 4, true)]
    [DataRow(0, 10, 5, 15, true)]
    [DataRow(0, 10, -5, 5, true)]
    [DataRow(2, 4, 0, 10, true)]
    [DataRow(5, 15, 0, 10, true)]
    [DataRow(-5, 5, 0, 10, true)]
    public void IsOverlappingTest(float min1, float max1, float min2, float max2, bool expectedResult)
    {
        NumericRange range1 = new(min1, max1);
        NumericRange range2 = new(min2, max2);

        Assert.AreEqual(expectedResult, range1.IsOverlapping(range2));
    }

    [TestMethod]
    [DataRow(0.4f, 7.3f, 1, 7, true)]
    [DataRow(-6.6f, -0.1f, -6, -1, true)]
    [DataRow(0.4f, 7.3f, 0, 8, false)]
    [DataRow(-6.6f, -0.1f, -7, 0, false)]
    public void ToRangeTest(float fMin, float fMax, int iMin, int iMax, bool innerRange)
    {
        NumericRange range = new(fMin, fMax);
        NumericRange iRange = range.ToRange(innerRange);

        Assert.AreEqual(iMin, iRange.Min);
        Assert.AreEqual(iMax, iRange.Max);
    }

    [TestMethod]
    [DataRow(1.1f, 2.2f, 1.1f, 2.2f, true)]
    [DataRow(-2.2f, -1.1f, -2.2f, -1.1f, true)]
    [DataRow(1.1f, 2.2f, 2.2f, 3.3f, false)]
    [DataRow(1.1f, 2.2f, 1.1f, 4.4f, false)]
    [DataRow(1.1f, 2.2f, 3.3f, 4.4f, false)]
    public void EqualityOperatorTest(float min1, float max1, float min2, float max2, bool areEqual)
    {
        NumericRange range1 = new(min1, max1);
        NumericRange range2 = new(min2, max2);

        Assert.AreEqual(range1.Equals(range2), areEqual);
        Assert.AreEqual(range1 == range2, areEqual);
        Assert.AreEqual(range1 != range2, !areEqual);
    }

    // --- Constructors (long / decimal / byte) ---

    [TestMethod]
    public void LongConstructor_NormalizesLimits()
    {
        var range = new NumericRange(1L, 5L);

        Assert.AreEqual(1d, range.Min);
        Assert.AreEqual(5d, range.Max);
    }

    [TestMethod]
    public void DecimalConstructor_NormalizesLimits()
    {
        var range = new NumericRange(1.5m, 5.5m);

        Assert.AreEqual(1.5d, range.Min);
        Assert.AreEqual(5.5d, range.Max);
    }

    [TestMethod]
    public void ByteConstructor_NormalizesLimits()
    {
        var range = new NumericRange((byte)2, (byte)8);

        Assert.AreEqual(2d, range.Min);
        Assert.AreEqual(8d, range.Max);
    }

    // --- Length ---

    [TestMethod]
    public void Length_ComputesDifference()
    {
        var range = new NumericRange(1.0, 4.5);

        Assert.AreEqual(3.5, range.Length);
    }

    // --- IsInside(range) ---

    [TestMethod]
    public void IsInsideRange_ChecksBothLimits()
    {
        var outer = new NumericRange(0.0, 10.0);
        var inner = new NumericRange(2.0, 8.0);

        Assert.IsTrue(outer.IsInside(inner));
        Assert.IsFalse(inner.IsInside(outer));
    }

    // --- Intersection ---

    [TestMethod]
    public void Intersection_ReturnsOverlap()
    {
        var a = new NumericRange(0.0, 10.0);
        var b = new NumericRange(5.0, 15.0);

        var intersection = a.Intersection(b);

        Assert.AreEqual(5.0, intersection.Min);
        Assert.AreEqual(10.0, intersection.Max);
    }

    // --- Equals(object) / GetHashCode ---

    [TestMethod]
    public void EqualsObject_And_GetHashCode_Work()
    {
        var a = new NumericRange(1.0, 2.0);
        var b = new NumericRange(1.0, 2.0);
        var c = new NumericRange(1.0, 3.0);

        Assert.IsTrue(a.Equals((object)b));
        Assert.IsFalse(a.Equals((object)c));
        Assert.IsFalse(a.Equals("not a range"));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
    }

    // --- ToString ---

    [TestMethod]
    public void ToString_And_FormattedToString()
    {
        var range = new NumericRange(1.0, 2.0);

        Assert.AreEqual("[1, 2]", range.ToString());
        Assert.AreEqual("[1.0, 2.0]", range.ToString("F1", CultureInfo.InvariantCulture));
    }

    // --- Enumeration (generic + non-generic) ---

    [TestMethod]
    public void Enumeration_YieldsValuesBelowMax()
    {
        var range = new NumericRange(0.0, 3.0);

        var generic = new List<double>();
        foreach (var value in range)
            generic.Add(value);
        Assert.AreSequenceEqual(EnumeratedValues, generic);

        var nonGeneric = new List<double>();
        System.Collections.IEnumerable enumerable = range;
        foreach (double value in enumerable)
            nonGeneric.Add(value);
        Assert.AreSequenceEqual(EnumeratedValues, nonGeneric);
    }
}
