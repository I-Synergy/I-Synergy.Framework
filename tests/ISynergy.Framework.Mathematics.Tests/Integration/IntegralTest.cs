using ISynergy.Framework.Mathematics.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Integration;
[TestClass]
public class IntegralTest
{

    private static double function1(double x)
    {
        return 2 + Math.Cos(2 * Math.Sqrt(x));
    }

    [TestMethod]
    public void TrapezoidalTest()
    {
        double actual;

        actual = TrapezoidalRule.Integrate(function1, 0, 2, 1);
        Assert.AreEqual(4.0486368718741526, actual, 1e-15);
        Assert.IsFalse(double.IsNaN(actual));

        actual = TrapezoidalRule.Integrate(function1, 0, 2, 2);
        Assert.AreEqual(3.6081715993899337, actual, 1e-15);
        Assert.IsFalse(double.IsNaN(actual));

        actual = TrapezoidalRule.Integrate(function1, 0, 2, 4);
        Assert.AreEqual(3.4971047822027077, actual, 1e-15);
        Assert.IsFalse(double.IsNaN(actual));

        actual = TrapezoidalRule.Integrate(function1, 0, 2, 8);
        Assert.AreEqual(3.4692784302833672, actual, 1e-15);
        Assert.IsFalse(double.IsNaN(actual));

        actual = TrapezoidalRule.Integrate(function1, 0, 2, 16);
        Assert.AreEqual(3.4623181105467689, actual, 1e-15);
        Assert.IsFalse(double.IsNaN(actual));
    }

    [TestMethod]
    public void MultipleTests_Examples()
    {
        // Let's say we would like to compute the definite
        // integral of the function f(x) = cos(x) from -1 to 1

        Func<double, double> f = Math.Cos;

        double a = -1;
        double b = +1;

        double trapez = TrapezoidalRule.Integrate(f, a, b, steps: 1000); // 1.6829414
        double romberg = RombergMethod.Integrate(f, a, b); // 1.6829419
        double nagk = NonAdaptiveGaussKronrod.Integrate(f, a, b); // 1.6829419

        // Now let's say we would like to compute an improper integral
        // from -infinite to +infinite, such as the integral of a Gaussian
        // PDF (which should evaluate to 1):

        Func<double, double> g = (x) => 1 / Math.Sqrt(2 * Math.PI) * Math.Exp(-(x * x) / 2);

        double iagk = InfiniteAdaptiveGaussKronrod.Integrate(g,
            double.NegativeInfinity, double.PositiveInfinity); // Output should be 0.99999...


        Assert.AreEqual(1.6829414086350976, trapez); // 1.6829414086350976
        Assert.AreEqual(1.682941969615797, romberg); // 1.682941969615797
        Assert.AreEqual(1.6829419595739716, nagk); // 1.6829419595739716
        Assert.AreEqual(0.99999999999999978, iagk); // 0.99999999999999978
    }

    private static double function2(double x)
    {
        return 1 / x;
    }

    [TestMethod]
    public void RombergTest()
    {
        // Example from http://www.mathstat.dal.ca/~tkolokol/classes/1500/romberg.pdf

        double actual;

        actual = RombergMethod.Integrate(function2, 1, 2, 1);
        Assert.AreEqual(0.75, actual);

        actual = RombergMethod.Integrate(function2, 1, 2, 2);
        Assert.AreEqual(0.69444444444444431, actual);

        actual = RombergMethod.Integrate(function2, 1, 2, 4);
        Assert.AreEqual(0.6931474776448322, actual);

        actual = RombergMethod.Integrate(function2, 1, 2, 8);
        Assert.AreEqual(0.6931471805599444, actual);

        actual = RombergMethod.Integrate(function2, 1, 2, 16);
        Assert.AreEqual(0.69314718055994151, actual);
    }


    public static double function3(double x)
    {
        return x * Math.Exp(-(x * x));
    }

    [TestMethod]
    public void GaussKronrodTest()
    {
        double expected = Math.Sin(2);
        double actual = NonAdaptiveGaussKronrod.Integrate(Math.Cos, 0, 2);

        Assert.AreEqual(expected, actual, 1e-6);
    }

}
