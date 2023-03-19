namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics.Transforms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Numerics;

    [TestClass]
    public class HilbertTransformTest
    {

        [TestMethod]
        public void FHTTest()
        {
            Complex[] original = { (Complex)1, (Complex)2, (Complex)3, (Complex)4 };
            Complex[] actual = (Complex[])original.Clone();

            HilbertTransform.FHT(actual, FourierTransform.Direction.Forward);

            Assert.AreEqual(actual[0].Real, 1);
            Assert.AreEqual(actual[1].Real, 2);
            Assert.AreEqual(actual[2].Real, 3);
            Assert.AreEqual(actual[3].Real, 4);

            Assert.AreEqual(actual[0].Imaginary, +1, 0.000000001);
            Assert.AreEqual(actual[1].Imaginary, -1, 0.000000001);
            Assert.AreEqual(actual[2].Imaginary, -1, 0.000000001);
            Assert.AreEqual(actual[3].Imaginary, +1, 0.000000001);

            HilbertTransform.FHT(actual, FourierTransform.Direction.Backward);

            Assert.AreEqual(actual[0], original[0]);
            Assert.AreEqual(actual[1], original[1]);
            Assert.AreEqual(actual[2], original[2]);
            Assert.AreEqual(actual[3], original[3]);
        }

        [TestMethod]
        public void FHTTest2()
        {
            double[] original = { -1.0, -0.8, -0.2, -0.1, 0.1, 0.2, 0.8, 1.0 };

            double[] actual = (double[])original.Clone();

            HilbertTransform.FHT(actual, FourierTransform.Direction.Forward);

            HilbertTransform.FHT(actual, FourierTransform.Direction.Backward);

            Assert.AreEqual(actual[0], original[0], 0.08);
            Assert.AreEqual(actual[1], original[1], 0.08);
            Assert.AreEqual(actual[2], original[2], 0.08);
            Assert.AreEqual(actual[3], original[3], 0.08);
        }
    }
}
