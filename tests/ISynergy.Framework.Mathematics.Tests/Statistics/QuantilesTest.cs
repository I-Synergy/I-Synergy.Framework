using ISynergy.Framework.Mathematics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests;

/// <summary>
/// Quantiles Test.
/// </summary>
[TestClass]
public class QuantilesTest
{
    private static readonly double[] EVEN_DATA = new double[] { 8.2, 6.2, 10.2, 20.2, 16.2, 15.2, 13.2, 3.2, 7.2, 8.2 };
    private static readonly double[] ODD_DATA = new double[] { 8.2, 6.2, 10.2, 9.2, 20.2, 16.2, 15.2, 13.2, 3.2, 7.2, 8.2 };
    private static readonly double[] TEST_PROBABILITIES = new double[] { 0.0, 0.25, 0.5, 0.75, 1.0 };

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile1_Even()
    {
        double[] expected = new double[] { 3.2, 7.2, 8.2, 15.2, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)1, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile1_Odd()
    {
        double[] expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)1, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile2_Even()
    {
        double[] expected = new double[] { 3.2, 7.7, 9.2, 15.7, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)2, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile2_Odd()
    {
        double[] expected = new double[] { 3.2, 7.7, 9.7, 15.7, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)2, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile3_Even()
    {
        double[] expected = new double[] { 3.2, 6.2, 8.2, 15.2, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)3, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile3_Odd()
    {
        double[] expected = new double[] { 3.2, 7.2, 9.2, 13.2, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)3, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile4_Even()
    {
        double[] expected = new double[] { 3.2, 6.7, 8.2, 14.2, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)4, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile4_Odd()
    {
        double[] expected = new double[] { 3.2, 6.95, 8.7, 13.7, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)4, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile5_Even()
    {
        double[] expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)5, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile5_Odd()
    {
        double[] expected = new double[] { 3.2, 7.45, 9.20, 14.7, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)5, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile6_Even()
    {
        double[] expected = new double[] { 3.2, 6.95, 9.2, 15.45, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)6, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile6_Odd()
    {
        double[] expected = new double[] { 3.2, 7.2, 9.2, 15.2, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)6, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile7_Even()
    {
        double[] expected = new double[] { 3.2, 7.45, 9.2, 14.7, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)7, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile7_Odd()
    {
        double[] expected = new double[] { 3.2, 7.7, 9.2, 14.2, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)7, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile8_Even()
    {
        double[] expected = new double[] { 3.2, 7.11666667, 9.2, 15.28333333, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)8, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile8_Odd()
    {
        double[] expected = new double[] { 3.2, 7.36666667, 9.2, 14.86666667, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)8, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile even Test
    /// </summary>
    [TestMethod]
    public void Quantile9_Even()
    {
        double[] expected = new double[] { 3.2, 7.1375, 9.2, 15.2625, 20.2 };
        double[] actual = EVEN_DATA.Quantiles(type: (QuantileMethod)9, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Quantile odd Test
    /// </summary>
    [TestMethod]
    public void Quantile9_Odd()
    {
        double[] expected = new double[] { 3.2, 7.3875, 9.2, 14.8250, 20.2 };
        double[] actual = ODD_DATA.Quantiles(type: (QuantileMethod)9, probabilities: TEST_PROBABILITIES);
        Assert_AreEqual(expected, actual);
    }

    /// <summary>
    /// Assert equal Test.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="rtol"></param>
    private static void Assert_AreEqual(double[] expected, double[] actual, double rtol = 1e-8)
    {
        Assert.IsTrue(expected.IsEqual(actual, rtol: rtol));
    }
}
