using ISynergy.Framework.Core.Points;
using ISynergy.Framework.Core.Ranges;
using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Common;
[TestClass]
public class CommonToolsTest
{

    [TestMethod]
    public void ScaleTest1()
    {
        double fromMin = 0;
        double fromMax = 1;
        double toMin = 0;
        double toMax = 100;
        double x = 0.2;

        double actual = x.Scale(fromMin, fromMax, toMin, toMax);
        Assert.AreEqual(20.0, actual);

        float actualF = (float)x.Scale((float)fromMin, (float)fromMax, (float)toMin, (float)toMax);
        Assert.AreEqual(20f, actualF);
    }


    [TestMethod]
    public void ScaleTest()
    {
        NumericRange from = new(0, 100);
        NumericRange to = new(0, 50);
        int x = 50;
        int expected = 25;
        double actual = Vector.Scale(x, from, to);
        Assert.AreEqual(expected, actual);
    }


    [TestMethod]
    public void ScaleTest2()
    {
        NumericRange from = new(-100, 100);
        NumericRange to = new(0, 50);
        double x = 0;
        double expected = 25;
        double actual = Vector.Scale(x, from, to);
        Assert.AreEqual(expected, actual);
    }


    [TestMethod]
    public void ScaleTest3()
    {
        double toMin = 0;
        double toMax = 100;

        double[][] x =
        [
            [-1.0,  1.0],
            [-0.2,  0.0],
            [-0.6,  0.0],
            [0.0, -1.0]
        ];

        double[][] expected =
        [
            [0, 100],
            [80,  50],
            [40,  50],
            [100,   0]
        ];

        double[] min = Matrix.Min(x, 0);
        double[] max = Matrix.Max(x, 0);

        double[][] actual = Vector.Scale(min, max, toMin, toMax, x);

        Assert.IsTrue(Matrix.IsEqual(expected, actual));

    }

    [TestMethod]
    public void AtanhTest()
    {
        double d = 0.42;
        double expected = 0.447692023527421;
        double actual = Tools.Atanh(d);
        Assert.AreEqual(expected, actual, 1e-10);
    }


    [TestMethod]
    public void AsinhTest()
    {
        double d = 0.42;
        double expected = 0.408540207829808;
        double actual = Tools.Asinh(d);
        Assert.AreEqual(expected, actual, 1e-10);
    }


    [TestMethod]
    public void AcoshTest()
    {
        double x = 3.14;
        double expected = 1.810991348900196;
        double actual = Tools.Acosh(x);
        Assert.AreEqual(expected, actual, 1e-10);
    }


    [TestMethod]
    public void InvSqrtTest()
    {
        float f = 42f;

        float expected = 1f / (float)System.Math.Sqrt(f);
        float actual = Tools.InvSqrt(f);

        Assert.AreEqual(expected, actual, 0.001);
    }

    [TestMethod]
    public void DirectionTest()
    {
        Point center = new(0, 0);

        Point w = new(1, 0);
        Point nw = new(1, 1);
        Point n = new(0, 1);
        Point ne = new(-1, 1);
        Point e = new(-1, 0);
        Point se = new(-1, -1);
        Point s = new(0, -1);
        Point sw = new(1, -1);


        int actual;
        int expected;

        actual = Tools.Direction(center, w);
        expected = (int)System.Math.Floor(0 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, nw);
        expected = (int)System.Math.Floor(45 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, n);
        expected = (int)System.Math.Floor(90 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, ne);
        expected = (int)System.Math.Floor(135 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, e);
        expected = (int)System.Math.Floor(180 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, se);
        expected = (int)System.Math.Floor(225 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, s);
        expected = (int)System.Math.Floor(270 / 18.0);
        Assert.AreEqual(expected, actual);

        actual = Tools.Direction(center, sw);
        expected = (int)System.Math.Floor(315 / 18.0);
        Assert.AreEqual(expected, actual);
    }

}
