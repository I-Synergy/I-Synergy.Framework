﻿namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public partial class VectorTest
    {

        [TestMethod]
        public void ExpandTest()
        {
            int[] v = Vector.Zeros<int>(5);
            double[] u = Vector.Create(5, 1.0);
            int[] w = Vector.Create(1, 2, 3);

            Assert.IsTrue(v.IsEqual(new[] { 0, 0, 0, 0, 0 }));
            Assert.IsTrue(u.IsEqual(new[] { 1, 1, 1, 1, 1 }));
            Assert.IsTrue(w.IsEqual(new[] { 1, 2, 3 }));
        }

        [TestMethod]
        public void GetIndicesTest()
        {
            double[] v = Vector.Ones(5);
            int[] idx = v.GetIndices();
            Assert.IsTrue(idx.IsEqual(new[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void sample_test()
        {
            var r = Vector.Range(10000);
            var s = Vector.Sample(r, 10);
            Assert.IsTrue(s.Any(x => x > 10));
        }

    }
}
