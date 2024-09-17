using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Financial.Tests;

/// <summary>
/// Class VATTests.
/// </summary>
[TestClass]
public class VATTests
{
    /// <summary>
    /// Defines the test method CalcPriceExclVATTest.
    /// </summary>
    [DataTestMethod]
    [DataRow(100, 21, 121)]
    [DataRow(0, 21, 0)]
    public void CalculateAmountFromAmountExcludingVATTest(object amount, object percentage, object result)
    {
        decimal assert = VAT.CalculateAmountFromAmountExcludingVAT(Convert.ToDecimal(percentage), Convert.ToDecimal(amount));
        Assert.AreEqual(Convert.ToDecimal(result), assert);
    }

    /// <summary>
    /// Defines the test method CalcPriceInclVATTest.
    /// </summary>
    [DataTestMethod]
    [DataRow(121, 21, 100)]
    [DataRow(0, 21, 0)]
    public void CalculateAmountFromAmountIncludingVATTest(object amount, object percentage, object result)
    {
        decimal assert = VAT.CalculateAmountFromAmountIncludingVAT(Convert.ToDecimal(percentage), Convert.ToDecimal(amount));
        Assert.AreEqual(Convert.ToDecimal(result), assert);
    }

    /// <summary>
    /// Defines the test method CalcVATExclVATTest.
    /// </summary>
    [DataTestMethod]
    [DataRow(100, 21, 21)]
    [DataRow(0, 21, 0)]
    public void CalculateVATFromAmountExcludingVATTest(object amount, object percentage, object result)
    {
        decimal assert = VAT.CalculateVATFromAmountExcludingVAT(Convert.ToDecimal(percentage), Convert.ToDecimal(amount));
        Assert.AreEqual(Convert.ToDecimal(result), assert);
    }

    /// <summary>
    /// Defines the test method CalcVATInclVATTest.
    /// </summary>
    [DataTestMethod]
    [DataRow(121, 21, 21)]
    [DataRow(0, 21, 0)]
    public void CalculateVATFromAmountIncludingVATTest(object amount, object percentage, object result)
    {
        decimal assert = VAT.CalculateVATFromAmountIncludingVAT(Convert.ToDecimal(percentage), Convert.ToDecimal(amount));
        Assert.AreEqual(Convert.ToDecimal(result), assert);
    }
}
