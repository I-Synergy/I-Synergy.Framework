using ISynergy.Framework.Mathematics.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Functions;
[TestClass]
public class BesselTest
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
    public void BesselY0Test()
    {
        double actual;

        actual = Bessel.Y0(64);
        Assert.AreEqual(0.037067103232088, actual, 0.000001);
    }

    [TestMethod]
    public void BesselJTest()
    {
        double actual;

        actual = Bessel.J(0, 1);
        Assert.AreEqual(0.765197686557967, actual, 0.000001);

        actual = Bessel.J(0, 5);
        Assert.AreEqual(-0.177596771314338, actual, 0.000001);

        actual = Bessel.J(2, 17.3);
        Assert.AreEqual(0.117351128521774, actual, 0.000001);
    }

    [TestMethod]
    public void BesselYTest()
    {
        double actual;

        actual = Bessel.Y(2, 4);
        Assert.AreEqual(0.215903594603615, actual, 0.000001);

        actual = Bessel.Y(0, 64);
        Assert.AreEqual(0.037067103232088, actual, 0.000001);
    }

    [TestMethod]
    public void BesselJ0Test()
    {
        double actual;

        actual = Bessel.J0(1);
        Assert.AreEqual(0.765197686557967, actual, 0.000001);

        actual = Bessel.J0(5);
        Assert.AreEqual(-0.177596771314338, actual, 0.000001);
    }

}
