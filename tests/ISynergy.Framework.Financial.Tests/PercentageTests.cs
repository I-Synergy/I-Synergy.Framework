using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Financial.Tests;

/// <summary>
/// Class PercentageTests.
/// </summary>
[TestClass]
public class PercentageTests
{
    /// <summary>
    /// Defines the test method CalcPercAmountOfAmountTest.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="mainamount">The mainamount.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    [DataRow(1, 50, 49)]
    [DataRow(100, 120, 0.2)]
    [DataRow(0, 0, 0)]
    [DataRow(0, 100, 1)]
    [DataRow(75, 15, -0.80)]
    [DataRow(573, 520, -0.0925)]
    [DataRow(12510.41, 14696.23, 0.1747)]
    public void CalcPercAmountOfAmountTest(object amount, object mainamount, object result)
    {
        decimal assert = Math.Round(
            Percentage.CalculatePercentageAmountOfAmount(
                Convert.ToDecimal(amount),
                Convert.ToDecimal(mainamount)), 4);
        Assert.AreEqual(Convert.ToDecimal(result), assert);
    }

    /// <summary>
    /// Defines the test method CalcPercAmountOfAmountDevideByZeroTest.
    /// </summary>
    [TestMethod]
    public void CalcPercAmountOfAmountDevideByZeroTest()
    {
        decimal result = Percentage.CalculatePercentageAmountOfAmount(0m, 0m);
        Assert.AreEqual(0m, result);
    }

    /// <summary>
    /// Defines the test method CalcAmountOfPercentageTest.
    /// </summary>
    [TestMethod]
    public void CalcAmountOfPercentageTest()
    {
        decimal result = Percentage.CalculateAmountOfPercentage(121m, 21m);
        Assert.AreEqual(25.41m, result);
    }

    /// <summary>
    /// Defines the test method CalcMarginPercentageTest.
    /// </summary>
    /// <param name="purchasePrice">The purchase price.</param>
    /// <param name="salesPrice">The sales price.</param>
    /// <param name="result">The result.</param>
    [DataTestMethod]
    [DataRow(1, 50, 49)]
    [DataRow(10, 100, 9)]
    [DataRow(10, 20, 1)]
    [DataRow(0, 100, 1)]
    [DataRow(1.25, 1.75, 0.4)]
    [DataRow(4.95, 2.5, -0.4949)]
    public void CalcMarginPercentageTest(object purchasePrice, object salesPrice, object result)
    {
        decimal assert = Math.Round(
            Percentage.CalculateMarginPercentage(
                Convert.ToDecimal(salesPrice),
                Convert.ToDecimal(purchasePrice)), 4);

        Assert.AreEqual(Convert.ToDecimal(result), assert);
    }

    /// <summary>
    /// Defines the test method CalcMarginPercentageDevideByZeroTest.
    /// </summary>
    [TestMethod]
    public void CalcMarginPercentageDevideByZeroTest()
    {
        decimal result = Percentage.CalculateMarginPercentage(100, 0);
        Assert.AreEqual(1, result);
    }
}
