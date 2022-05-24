namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using ISynergy.Framework.Mathematics.Comparers;

    [TestClass]
    public class GeneralComparerTest
    {
        [TestMethod]
        public void GeneralComparerConstructorTest()
        {
            double[] actual, expected;

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.NegativeInfinity, -1, 0, 2, Double.PositiveInfinity };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Ascending, useAbsoluteValues: false));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, 2, 0, -1, Double.NegativeInfinity };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, false));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, Double.NegativeInfinity, 2, -1, 0 };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, true));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, Double.NegativeInfinity, 2, -1, 0 };
            Array.Sort(actual, new GeneralComparer(ComparerDirection.Descending, Math.Abs));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));
        }
    }
}
