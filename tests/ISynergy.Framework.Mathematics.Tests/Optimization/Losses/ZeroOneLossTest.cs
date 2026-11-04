using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Optimization.Losses;
[TestClass]
public class ZeroOneLossTest
{
    [TestMethod]
    public void TestThatLossIsZeroForCorrectClassification()
    {
        int[] expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        double loss = new ZeroOneLoss(expected).Loss(expected);
        Assert.AreEqual(0, loss);
    }

    [TestMethod]
    public void TestThatLossIsOneForTotalMissClassification()
    {
        int[] expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        int[] actual = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        double loss = new ZeroOneLoss(expected).Loss(actual);
        Assert.AreEqual(1.0, loss, 1e-10);
    }

    [TestMethod]
    public void TestThatZeroOneLossReturnsNumberOfMissClassifications()
    {
        int[] expected = [1, 2, 3, 4, 5];
        int[] actual = [0, 0, 0, 0, 0];
        double loss = new ZeroOneLoss(expected)
        {
            Mean = false
        }.Loss(actual);
        Assert.AreEqual(5.0, loss, 1e-10);
    }

    [TestMethod]
    public void TestThatZeroOneLossEncodesDoubles()
    {
        double[] expected = [0, 1.01, 0.99, 0];
        int[] actual = [0, 1, 1, 0];
        double loss = new ZeroOneLoss(expected).Loss(actual);
        Assert.AreEqual(0, loss);
    }

    [TestMethod]
    public void TestThatZeroOneLossEncodesDoubleMatrices()
    {
        double[][] expected =
        [
            [1d, 0d, 1d, 0d],
            [0d, 1d, 0d, 1d]
        ];
        int[] actual = [0, 1, 0, 1];
        double loss = new ZeroOneLoss(expected).Loss(actual);
        Assert.AreEqual(0, loss);
    }

    [TestMethod]
    public void TestThatZeroOneLossNormalizesInputForBinaryClassification()
    {
        int[] expected = [-1, -1, 1, -1];
        int[] actual = [0, 0, 1, 0];
        double loss = new ZeroOneLoss(expected).Loss(actual);
        Assert.AreEqual(0, loss);
    }

    private static readonly int[] expected = new[] { 0, 1 };
    private static readonly int[] actual = new[] { 0, 2 };

    [TestMethod]
    public void gh_824()
    {
        double loss = new ZeroOneLoss(expected).Loss(actual);
        Assert.AreEqual(0.5, loss, 1e-10);
    }
}
