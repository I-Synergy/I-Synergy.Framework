﻿using ISynergy.Framework.Mathematics.Comparers;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Comparers;
[TestClass]
public class CustomComparerTest
{
    [TestMethod]
    public void CustomComparerConstructorTest()
    {
        double[] actual, expected;

        actual = [0, -1, 2, double.PositiveInfinity, double.NegativeInfinity];
        expected = [double.NegativeInfinity, -1, 0, 2, double.PositiveInfinity];
        Array.Sort(actual, new CustomComparer<double>((a, b) => a.CompareTo(b)));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));

        actual = [0, -1, 2, double.PositiveInfinity, double.NegativeInfinity];
        expected = [double.PositiveInfinity, 2, 0, -1, double.NegativeInfinity];
        Array.Sort(actual, new CustomComparer<double>((a, b) => -a.CompareTo(b)));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));

        actual = [0, 5, 3, 1, 8];
        expected = [8, 5, 3, 1, 0];
        Array.Sort(actual, new CustomComparer<double>((a, b) => -a.CompareTo(b)));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));
    }
}
