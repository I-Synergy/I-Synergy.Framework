namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Comparers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class CustomComparerTest
    {
        [TestMethod]
        public void CustomComparerConstructorTest()
        {
            double[] actual, expected;

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.NegativeInfinity, -1, 0, 2, Double.PositiveInfinity };
            Array.Sort(actual, new CustomComparer<double>((a, b) => a.CompareTo(b)));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, -1, 2, Double.PositiveInfinity, Double.NegativeInfinity };
            expected = new double[] { Double.PositiveInfinity, 2, 0, -1, Double.NegativeInfinity };
            Array.Sort(actual, new CustomComparer<double>((a, b) => -a.CompareTo(b)));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));

            actual = new double[] { 0, 5, 3, 1, 8 };
            expected = new double[] { 8, 5, 3, 1, 0 };
            Array.Sort(actual, new CustomComparer<double>((a, b) => -a.CompareTo(b)));
            Assert.IsTrue(Matrix.IsEqual(actual, expected));
        }
    }
}
