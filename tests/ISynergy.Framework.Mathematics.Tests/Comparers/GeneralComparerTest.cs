using ISynergy.Framework.Mathematics.Comparers;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Comparers;
[TestClass]
public class GeneralComparerTest
{
    [TestMethod]
    public void GeneralComparerConstructorTest()
    {
        double[] actual, expected;

        actual = [0, -1, 2, double.PositiveInfinity, double.NegativeInfinity];
        expected = [double.NegativeInfinity, -1, 0, 2, double.PositiveInfinity];
        Array.Sort(actual, new GeneralComparer(ComparerDirection.Ascending, useAbsoluteValues: false));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));

        actual = [0, -1, 2, double.PositiveInfinity, double.NegativeInfinity];
        expected = [double.PositiveInfinity, 2, 0, -1, double.NegativeInfinity];
        Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, false));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));

        actual = [0, -1, 2, double.PositiveInfinity, double.NegativeInfinity];
        expected = [double.PositiveInfinity, double.NegativeInfinity, 2, -1, 0];
        Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, true));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));

        actual = [0, -1, 2, double.PositiveInfinity, double.NegativeInfinity];
        expected = [double.PositiveInfinity, double.NegativeInfinity, 2, -1, 0];
        Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, Math.Abs));
        Assert.IsTrue(Matrix.IsEqual(actual, expected));
    }
}
