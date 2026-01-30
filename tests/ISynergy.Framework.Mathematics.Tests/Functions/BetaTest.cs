using ISynergy.Framework.Mathematics.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Functions;
[TestClass]
public class BetaTest
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
    public void IncompleteTest()
    {
        double a = 5;
        double b = 4;
        double x = 0.5;

        double actual = Beta.Incomplete(a, b, x);
        double expected = 0.36328125;

        Assert.AreEqual(expected, actual, 1e-6);
    }

    [TestMethod]
    public void FunctionTest()
    {
        double actual = Beta.Function(4, 0.42);
        Assert.AreEqual(1.2155480852832423, actual);
    }

    [TestMethod]
    public void LogTest()
    {
        double actual = Beta.Log(4, 15.2);
        Assert.AreEqual(-9.46087817876467, actual);
    }

    [TestMethod]
    public void Incbcf()
    {
        double actual = Beta.Incbcf(4, 2, 4.2);
        Assert.AreEqual(-0.23046874999999992, actual);
    }

    [TestMethod]
    public void Incbd()
    {
        double actual = Beta.Incbd(4, 2, 4.2);
        Assert.AreEqual(0.7375, actual);
    }

    [TestMethod]
    public void IncompleteInverse()
    {
        double actual = Beta.IncompleteInverse(0.5, 0.6, 0.1);
        Assert.AreEqual(0.019145979066925722, actual);
    }

    [TestMethod]
    public void PowerSeries()
    {
        double actual = Beta.PowerSeries(4, 2, 4.2);
        Assert.AreEqual(-3671.801280000001, actual);
    }

    [TestMethod]
    public void Multinomial()
    {
        double actual = Beta.Multinomial(0.42, 0.5, 5.2);
        Assert.AreEqual(0.82641912952987062, actual);
    }

    [TestMethod]
    public void IbetaTest()
    {
        double xx = 0.42;
        double aa = 2;
        double bb = 4;
        double expected = 0.696717907200000;

        double actual = Beta.Incomplete(aa, bb, xx);
        Assert.AreEqual(expected, actual, 0.0000001);
    }


    [TestMethod]
    public void BetaTest2()
    {
        double a = 4.2;
        double b = 3.0;
        double expected = 0.014770176060499;
        double actual = Beta.Function(a, b);
        Assert.AreEqual(expected, actual, 1e-6);
    }
}
