namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Comparers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class ElementComparerTest
    {

        [TestMethod]
        public void ElementComparerConstructorTest()
        {
            double[][] values =
            {   //                 v
                new double[] {  0, 3, 0 },
                new double[] {  0, 4, 1 },
                new double[] { -1, 1, 1 },
                new double[] { -1, 5, 4 },
                new double[] { -2, 2, 6 },
            };

            // Sort the array considering only the second column
            Array.Sort(values, new ElementComparer() { Index = 1 });

            double[][] expected =
            {
                new double[] { -1, 1, 1 },
                new double[] { -2, 2, 6 },
                new double[] {  0, 3, 0 },
                new double[] {  0, 4, 1 },
                new double[] { -1, 5, 4 },
            };

            Assert.IsTrue(values.IsEqual(expected));
        }
    }
}
