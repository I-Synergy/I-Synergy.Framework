namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Statistics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MeasuresTest
    {
        private const double Tolerance = 1e-10;
        private double[][] series;

        [TestInitialize]
        public void Setup()
        {
            double[,] data =
            {
                { 1, 3, 5, 6, 4, 2, 7, 8, 9, 2, 3, 4, 5, 6, 7 },
                { 5, 3, 8, 6, 4, 4, 3, 8, 9, 0, 9, 9, 1, 9, 2 },
            };

            this.series = data.ToJagged(transpose: true);
        }

        [TestMethod]
        public void EwmaWindowTest()
        {
            #region doc_example1
            /*
            Suppose we have a time series of (possibly correlated) observations. Our 
            sample has 17 observations in 2 variables (x and y). We wish to compute 
            the mean of our series but would like to provide a heavier weighting to 
            the more recent observations. First, arrange the observations in rows 
            with the oldest data at the top and the newest data at the bottom. */
            double[,] rawData =
            {
                { 2, 2, 1, 3, 5, 6, 4, 2, 7, 8, 9, 2, 3, 4, 5, 6, 7 },
                { 1, 2, 5, 3, 8, 6, 4, 4, 3, 8, 9, 0, 9, 9, 1, 9, 2 }
            };

            // Transpose our raw data to get it into the required format.
            double[][] timeSeries = rawData.ToJagged(transpose: true);

            // The window size determines how many observations to include in the 
            // calculation. If no window is specified, the entire dataset is used. 
            int window = 15;

            // We set alpha to 20% meaning each previous observation's contribution 
            // carries 20% less weight (relative to its immediate successor).  
            double alpha = 0.2;

            double[] ewm = timeSeries.ExponentialWeightedMean(window, alpha); // (5.47, 5.20)
            #endregion

            Assert.AreEqual(5.3708532126850983, ewm[0], Tolerance);
            Assert.AreEqual(5.2012181301523794, ewm[1], Tolerance);
        }

        [TestMethod]
        public void EwmCovWindowTest()
        {
            #region doc_example2
            /*
            Suppose we have a time series of (possibly correlated) observations. Our 
            sample has 17 observations in 2 variables (x and y). We wish to compute 
            the covariance matrix for our series but would like to provide a heavier 
            weighting to the more recent observations. First, arrange the observations 
            in rows with the oldest data at the top and the newest data at the bottom. */
            double[,] rawData =
            {
                { 2, 2, 1, 3, 5, 6, 4, 2, 7, 8, 9, 2, 3, 4, 5, 6, 7 },
                { 1, 2, 5, 3, 8, 6, 4, 4, 3, 8, 9, 0, 9, 9, 1, 9, 2 }
            };

            // Transpose our raw data to get it into the required format.
            double[][] timeSeries = rawData.ToJagged(transpose: true);

            // The window size determines how many observations to include in the 
            // calculation. If no window is specified, the entire dataset is used. 
            int window = 15;

            // We set alpha to 20% meaning each previous observation's contribution 
            // carries 20% less weight (relative to its immediate successor).  
            double alpha = 0.2;

            // Now we calculate the covariance matrix. The result should be:
            //
            //  { 3.80, 0.55 }
            //  { 0.55, 13.0 }
            //
            double[,] ewmCov = timeSeries.ExponentialWeightedCovariance(window, alpha);
            #endregion

            Assert.AreEqual(3.796312193730190, ewmCov[0, 0], Tolerance);
            Assert.AreEqual(0.550757446419597, ewmCov[1, 0], Tolerance);
            Assert.AreEqual(0.550757446419597, ewmCov[0, 1], Tolerance);
            Assert.AreEqual(12.99750453306480, ewmCov[1, 1], Tolerance);
        }

        [TestMethod]
        public void EwmWindowTest()
        {
            #region doc_example3
            /*
            Suppose we have a time series of observations. Our sample has 17 
            observations. We wish to compute the mean for our series but would 
            like to provide a heavier weighting to the more recent observations. 
            First, create a vector with the oldest data at the start and the most
            recent data at the end of the vector. */
            double[] timeSeries =
            {
                2, 2, 1, 3, 5, 6, 4, 2, 7, 8, 9, 2, 3, 4, 5, 6, 7
            };

            // The window size determines how many observations to include in the 
            // calculation. If no window is specified, the entire dataset is used. 
            int window = 15;

            // We set alpha to 20% meaning each previous observation's contribution 
            // carries 20% less weight (relative to its immediate successor).  
            double alpha = 0.2;

            // Now we calculate the EW mean. The result should be 5.37
            double ewm = timeSeries.ExponentialWeightedMean(window, alpha);
            #endregion

            Assert.AreEqual(5.3708532126850983, ewm, Tolerance);
        }

        [TestMethod]
        public void EwVarianceWindowTest()
        {
            #region doc_example4
            /*
            Suppose we have a time series of observations. Our sample has 17 
            observations. We wish to compute the variance for our series but would 
            like to provide a heavier weighting to the more recent observations. 
            First, create a vector with the oldest data at the start and the most
            recent data at the end of the vector. */
            double[] timeSeries =
            {
                2, 2, 1, 3, 5, 6, 4, 2, 7, 8, 9, 2, 3, 4, 5, 6, 7
            };

            // The window size determines how many observations to include in the 
            // calculation. If no window is specified, the entire dataset is used. 
            int window = 15;

            // We set alpha to 20% meaning each previous observation's contribution 
            // carries 20% less weight (relative to its immediate successor).  
            double alpha = 0.2;

            // Now we calculate the EW variance. The result should be 3.80.
            double ewVar = timeSeries.ExponentialWeightedVariance(window, alpha);
            #endregion

            Assert.AreEqual(3.796312193730190, ewVar, Tolerance);
        }

        [DataTestMethod]
        [DataRow(0, 4.8, 16d / 3)]
        [DataRow(0.2, 5.3708532126850983, 5.2012181301523794)]
        [DataRow(0.5, 6.1216162602618489, 4.3349406414990694)]
        [DataRow(1, 7, 2)]
        public void EwmaTest(double alpha, double e1, double e2)
        {
            // Arrange
            double[] expected = { e1, e2 };

            // Act
            double[] ewmas = this.series.ExponentialWeightedMean(alpha);

            // Assert
            Assert.IsTrue(ewmas.IsEqual(expected, Tolerance), "EWMA does not agree with expected");
        }

        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(0.2)]
        [DataRow(0.5)]
        [DataRow(1.0)]
        public void EwmaVecTest(double alpha)
        {
            /*
             * We have verified the numbers in the case of the matrix and so
             * it suffices to check consistency with the matrix calculations
             * here.
             */

            // Arrange
            double[] vec1 = this.series.GetColumn(0);
            double[] vec2 = this.series.GetColumn(1);

            // Act
            double ewma1 = vec1.ExponentialWeightedMean(alpha);
            double ewma2 = vec2.ExponentialWeightedMean(alpha);
            double[] ewmas = this.series.ExponentialWeightedMean(alpha);

            // Assert
            Assert.AreEqual(ewmas[0], ewma1, Tolerance);
            Assert.AreEqual(ewmas[1], ewma2, Tolerance);
        }

        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(0.2)]
        [DataRow(0.5)]
        [DataRow(1.0)]
        public void EwmaWindowVecTest(double alpha)
        {
            /*
             * We have verified the numbers in the case of the matrix and so
             * it suffices to check consistency with the matrix calculations
             * here.
             */

            // Arrange
            int window = 5;
            double[] vec1 = this.series.GetColumn(0);
            double[] vec2 = this.series.GetColumn(1);

            // Act
            double ewma1 = vec1.ExponentialWeightedMean(window, alpha);
            double ewma2 = vec2.ExponentialWeightedMean(window, alpha);
            double[] ewmas = this.series.ExponentialWeightedMean(window, alpha);

            // Assert
            Assert.AreEqual(ewmas[0], ewma1, Tolerance);
            Assert.AreEqual(ewmas[1], ewma2, Tolerance);
        }

        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(0.2)]
        [DataRow(0.5)]
        [DataRow(1.0)]
        public void EwmBiasedVarianceTest(double alpha)
        {
            /*
             * We have verified the numbers in the case of the matrix and so
             * it suffices to check consistency with the matrix calculations
             * here.
             */

            // Arrange
            int window = 5;
            double[] vec1 = this.series.GetColumn(0);
            double[] vec2 = this.series.GetColumn(1);

            // Act
            double ewma1 = vec1.ExponentialWeightedVariance(window, alpha, unbiased: false);
            double ewma2 = vec2.ExponentialWeightedVariance(window, alpha, unbiased: false);
            double[,] ewmas = this.series.ExponentialWeightedCovariance(window, alpha, unbiased: false);

            // Assert
            Assert.AreEqual(ewmas[0, 0], ewma1, Tolerance);
            Assert.AreEqual(ewmas[1, 1], ewma2, Tolerance);
        }

        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(0.2)]
        [DataRow(0.5)]
        [DataRow(1.0)]
        public void EwmVarianceUnbiasedTest(double alpha)
        {
            /*
             * We have verified the numbers in the case of the matrix and so
             * it suffices to check consistency with the matrix calculations
             * here.
             */

            // Arrange
            double[] vec1 = this.series.GetColumn(0);
            double[] vec2 = this.series.GetColumn(1);

            // Act
            double ewma1 = vec1.ExponentialWeightedVariance(alpha, unbiased: true);
            double ewma2 = vec2.ExponentialWeightedVariance(alpha, unbiased: true);
            double[,] ewmas = this.series.ExponentialWeightedCovariance(alpha, unbiased: true);

            // Assert
            Assert.AreEqual(ewmas[0, 0], ewma1, Tolerance);
            Assert.AreEqual(ewmas[1, 1], ewma2, Tolerance);
        }

        [DataTestMethod]
        [DataRow(0, 5.6, 2.28571428571429, 10.0952380952381)]
        [DataRow(0.2, 4.31014623570092, 0.625302929087394, 14.7567276814633)]
        [DataRow(0.5, 2.2476817067779, -2.04530939646465, 18.1801422111128)]
        [DataRow(1, 0, 0, 0)]
        public void UnbiasedEwmCovTest(double alpha, double cov11, double cov12, double cov22)
        {
            // Arrange
            double[,] expected =
            {
                { cov11, cov12 },
                { cov12, cov22 },
            };

            // Act
            double[,] ewmCov = this.series.ExponentialWeightedCovariance(alpha, unbiased: true);

            // Assert
            Assert.IsTrue(ewmCov.IsEqual(expected, Tolerance), "EWM Covariance does not agree with expected");
        }

        [DataTestMethod]
        [DataRow(0.0, 5.22666666666667, 2.13333333333333, 9.422222222222222)]
        [DataRow(0.2, 3.79631219373019, 0.550757446419597, 12.9975045330648)]
        [DataRow(0.5, 1.49840874058829, -1.36349798444697, 12.1197249201803)]
        [DataRow(1.0, 0, 0, 0)]
        public void BiasedEwmCovTest(double alpha, double cov11, double cov12, double cov22)
        {
            // Arrange
            double[,] expected =
            {
                { cov11, cov12 },
                { cov12, cov22 },
            };

            // Act
            double[,] ewmCov = this.series.ExponentialWeightedCovariance(alpha, unbiased: false);

            // Assert
            Assert.IsTrue(ewmCov.IsEqual(expected, Tolerance), "EWM Covariance does not agree with expected");
        }
    }
}
