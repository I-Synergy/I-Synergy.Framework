using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.UI.Extensions.Tests;

[TestClass]
public class DecimalExtensionsTests
{
    private CultureInfo _originalCulture;

    [TestInitialize]
    public void Setup()
    {
        _originalCulture = CultureInfo.CurrentCulture;
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Restore original culture
        CultureInfo.CurrentCulture = _originalCulture;
    }

    [TestMethod]
    public void ToCurrency_WithCustomSymbol_ReturnsCorrectFormat()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        currentCulture.NumberFormat.CurrencySymbol = "€";
        currentCulture.NumberFormat.CurrencyGroupSeparator = ".";
        currentCulture.NumberFormat.CurrencyDecimalSeparator = ",";
        CultureInfo.CurrentCulture = currentCulture;

        decimal value = 1234.56m;

        // Act
        string result = value.ToCurrency();

        // Assert
        Assert.IsTrue(result.StartsWith("€"));
        Assert.IsTrue(result.Contains("1.234,56"));
    }

    [TestMethod]
    public void ToCurrency_WithNegativeValue_ReturnsCorrectFormat()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        currentCulture.NumberFormat.CurrencySymbol = "$";
        currentCulture.NumberFormat.CurrencyGroupSeparator = ",";
        currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
        CultureInfo.CurrentCulture = currentCulture;

        decimal value = -1234.56m;

        // Act
        string result = value.ToCurrency();

        // Assert
        Assert.IsTrue(result.StartsWith("("));
        Assert.IsTrue(result.Contains("1,234.56"));
        Assert.IsTrue(result.EndsWith(")"));
    }

    [TestMethod]
    public void ToCurrency_WithZeroValue_ReturnsCorrectFormat()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        currentCulture.NumberFormat.CurrencySymbol = "$";
        currentCulture.NumberFormat.CurrencyGroupSeparator = ",";
        currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";

        CultureInfo.CurrentCulture = currentCulture;

        decimal value = 0m;

        // Act
        string result = value.ToCurrency();

        // Assert
        Assert.IsTrue(result.Contains("$ 0.00"));
    }
}
