namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Common;
    using ISynergy.Framework.Mathematics.Formats;
    using ISynergy.Framework.Mathematics.Wavelets;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HaarTest
    {

        [TestMethod]
        public void FWT2DTest()
        {
            double[,] original =
            {
                { 5, 6, 1, 2 },
                { 4, 2, 5, 5 },
                { 3, 1, 7, 1 },
                { 6, 3, 5, 1 }
            };

            double[,] data = (double[,])original.Clone();

            Haar.FWT(data, 1);

            double[,] expected = 
            {
                {  4.25,  3.25,  0.25, -0.25 },
                {  3.25,  3.5,   1.25,  2.5  },
                {  1.25, -1.75, -0.75, -0.25 },
                { -1.25,  0.5,  -0.25,  0.5  } 
            };

            // string dataStr = data.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            Assert.IsTrue(Matrix.IsEqual(expected, data, 1e-5));

            Haar.IWT(data, 1);

            Assert.IsTrue(Matrix.IsEqual(data, original, 0.0001));
        }

        [TestMethod]
        public void FWT2DTest2()
        {
            int levels = 2;

            double[,] original =
            {
                { 5, 6, 1, 2 },
                { 4, 2, 5, 5 },
                { 3, 1, 7, 1 },
                { 6, 3, 5, 1 }
            };

            double[,] data = (double[,])original.Clone();

            Haar.FWT(data, levels);

            double[,] expected = 
            {
                {  3.5625, 0.1875, 0.25, -0.25 },
                {  0.1875, 0.3125, 1.25,  2.5  },
                {  1.25,  -1.75,  -0.75, -0.25 },
                { -1.25,   0.5,   -0.25,  0.5  } 
            };

            string dataStr = data.ToString(CSharpMatrixFormatProvider.InvariantCulture);

            Assert.IsTrue(Matrix.IsEqual(expected, data, 1e-5));

            Haar.IWT(data, levels);

            Assert.IsTrue(Matrix.IsEqual(data, original, 0.0001));
        }

        [TestMethod]
        public void IWTTest()
        {
            double[] original = { 1, 2, 3, 4 };
            double[] data = { 1, 2, 3, 4 };
            double[] expected = { 2.1213, 4.9497, -0.7071, -0.7071 };

            Haar.FWT(data);

            var d = data.Multiply(Constants.Sqrt2);

            Assert.IsTrue(Matrix.IsEqual(expected, d, 0.001));

            Haar.IWT(data);
            Assert.IsTrue(Matrix.IsEqual(original, data, 0.001));
        }

    }
}
