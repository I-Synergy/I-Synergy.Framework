﻿using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Common;
[TestClass]
public class ReducedRowEchelonFormTest
{


    private TestContext testContextInstance;

    public TestContext TestContext
    {
        get
        {
            return testContextInstance;
        }
        set
        {
            testContextInstance = value;
        }
    }



    [TestMethod]
    public void ReducedRowEchelonFormConstructorTest()
    {
        double[,] matrix =
        {
            { 1, 2, -3 },
            { 3, 5,  9 },
            { 5, 9,  3 },
        };

        ReducedRowEchelonForm target = new(matrix);

        double[,] actual = target.Result;
        double[,] expected =
        {
            { 1, 0,  33 },
            { 0, 1, -18 },
            { 0, 0,   0 },
        };


        Assert.IsTrue(expected.IsEqual(actual));
    }

    [TestMethod]
    public void ReducedRowEchelonFormConstructorTest2()
    {
        double[,] matrix =
        {
            {3,2,2,3,1},
            {6,4,4,6,2},
            {9,6,6,9,1},
        };

        ReducedRowEchelonForm target = new(matrix);

        double[,] actual = target.Result;

        double[,] expected =
        {
            { 1, 2/3.0,  2/3.0,   1,   0   },
            { 0,     0,      0,   0,   1   },
            { 0,     0,      0,   0,   0   },
        };


        Assert.IsTrue(expected.IsEqual(actual));
    }

}
