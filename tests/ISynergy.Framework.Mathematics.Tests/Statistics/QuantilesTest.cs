namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ISynergy.Framework.Mathematics.Statistics;

    [TestClass]
    public class QuantilesTest
    {
        private static readonly double[] EVEN_DATA = new double[] { 8.2, 6.2, 10.2, 20.2, 16.2, 15.2, 13.2, 3.2, 7.2, 8.2 };
        private static readonly double[] ODD_DATA = new double[] { 8.2, 6.2, 10.2, 9.2, 20.2, 16.2, 15.2, 13.2, 3.2, 7.2, 8.2 };
        private static readonly double[] TEST_PROBABILITIES = new double[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

        [TestMethod]
        public void Quantile1_Even()
        {
            var expected = new double[] { 3.2, 7.2, 8.2, 15.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)1, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile1_Odd()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)1, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile2_Even()
        {
            var expected = new double[] { 3.2, 7.7, 9.2, 15.7, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)2, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile2_Odd()
        {
            var expected = new double[] { 3.2, 7.7, 9.7, 15.7, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)2, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile3_Even()
        {
            var expected = new double[] { 3.2, 6.2, 8.2, 15.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)3, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile3_Odd()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 13.2, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)3, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile4_Even()
        {
            var expected = new double[] { 3.2, 6.7, 8.2, 14.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)4, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile4_Odd()
        {
            var expected = new double[] { 3.2, 6.95, 8.7, 13.7, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)4, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile5_Even()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)5, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile5_Odd()
        {
            var expected = new double[] { 3.2, 7.45, 9.20, 14.7, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)5, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile6_Even()
        {
            var expected = new double[] { 3.2, 6.95, 9.2, 15.45, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)6, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile6_Odd()
        {
            var expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)6, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile7_Even()
        {
            var expected = new double[] { 3.2, 7.45, 9.2, 14.7, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)7, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile7_Odd()
        {
            var expected = new double[] { 3.2, 7.7, 9.2, 14.2, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)7, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile8_Even()
        {
            var expected = new double[] { 3.2, 7.11666667, 9.2, 15.28333333, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)8, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile8_Odd()
        {
            var expected = new double[] { 3.2, 7.36666667, 9.2, 14.86666667, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)8, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile9_Even()
        {
            var expected = new double[] { 3.2, 7.1375, 9.2, 15.2625, 20.2 };
            var actual = EVEN_DATA.Quantiles(type: (QuantileMethod)9, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        [TestMethod]
        public void Quantile9_Odd()
        {
            var expected = new double[] { 3.2, 7.3875, 9.2, 14.8250, 20.2 };
            var actual = ODD_DATA.Quantiles(type: (QuantileMethod)9, probabilities: TEST_PROBABILITIES);
            Assert_AreEqual(expected, actual);
        }

        private static void Assert_AreEqual(double[] expected, double[] actual, double rtol = 1e-8)
        {
            Assert.IsTrue(expected.IsEqual(actual, rtol: rtol));
        }
    }
}
