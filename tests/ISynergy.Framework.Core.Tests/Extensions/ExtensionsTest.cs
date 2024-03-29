﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests;

[TestClass]
public class ExtensionsTest
{
    [TestMethod]
    public void CompareTest()
    {
        Assert.IsTrue(10.IsLessThan(11));
        Assert.IsTrue((-1).IsLessThanOrEqual(-1));

        Assert.IsTrue(10.IsGreaterThan(9));
        Assert.IsTrue((-1).IsGreaterThanOrEqual(-2));
    }

    [TestMethod]
    public void MakeArrayType_test()
    {
        System.Type t = typeof(double);
        System.Type r = t.MakeArrayType(rank: 3, jagged: true);
        Assert.AreEqual(typeof(double[][][]), r);

        r = t.MakeArrayType(rank: 1, jagged: true);
        Assert.AreEqual(typeof(double[]), r);

        r = t.MakeArrayType(rank: 0, jagged: true);
        Assert.AreEqual(typeof(double), r);
    }
}
