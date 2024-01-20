using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Common;
[TestClass]
public class NormTest
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
    public void EuclideanTest()
    {
        double[,] a =
        {
            { 15.4457, 0.4187, 15.6093 },
            {  0.0000, 2.5708,  0.6534 }
        };


        double[] expected =
        {
            21.9634, 2.6525
        };

        double[] actual = a.Euclidean(1);
        Assert.IsTrue(expected.IsEqual(actual, 0.001));


        double[] expected2 =
        {
            15.4457, 2.6047, 15.6229
        };

        double[] actual2 = a.Euclidean(0);
        Assert.IsTrue(expected2.IsEqual(actual2, 0.001));

        double actual3 = a.GetRow(0).Euclidean();
        Assert.AreEqual(21.9634, actual3, 0.001);
    }

    [TestMethod]
    public void Norm2Test()
    {
        double[,] a =
        {
            { 2,     1,     5 },
            { 2,     2,     2 },
            { 1,     6,     4 }
        };

        double expected = 9.071111071571606;
        double actual = a.Norm2();
        Assert.AreEqual(expected, actual, 1e-12);
    }

    [TestMethod]
    public void Norm1Test()
    {
        double[,] a =
        {
            { 2,     1,     5 },
            { 2,     2,     2 },
            { 1,     6,     4 }
        };

        double expected = 11;
        double actual = a.Norm1();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void FrobeniusTest()
    {
        double[,] a = Matrix.Magic(5);

        double expected = 74.330343736592520;
        double actual = a.Frobenius();

        Assert.AreEqual(expected, actual, 1e-12);

    }
}
