using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters.Tests;

[TestClass]
public class CurrencyConverterTests
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
    public void CurrencyConverter_Convert_ReturnsFormattedCurrency()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        currentCulture.NumberFormat.CurrencySymbol = "$";
        currentCulture.NumberFormat.CurrencyGroupSeparator = ",";
        currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
        CultureInfo.CurrentCulture = currentCulture;

        var converter = new CurrencyConverter();
        decimal value = 1234.56m;

        // Act
        var result = converter.Convert(value, typeof(string), null, null) as string;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.StartsWith("$"));
        Assert.IsTrue(result.EndsWith("1,234.56"));
    }

    [TestMethod]
    public void CurrencyConverter_Convert_HandlesNegativeValue()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        currentCulture.NumberFormat.CurrencySymbol = "$";
        currentCulture.NumberFormat.CurrencyGroupSeparator = ",";
        currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
        currentCulture.NumberFormat.CurrencyNegativePattern = 1;
        CultureInfo.CurrentCulture = currentCulture;

        var converter = new CurrencyConverter();
        decimal value = -1234.56m;

        // Act
        var result = converter.Convert(value, typeof(string), null, null) as string;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.StartsWith("-"));
        Assert.IsTrue(result.EndsWith("1,234.56"));
    }

    [TestMethod]
    public void CurrencyConverter_Convert_HandlesZeroValue()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        currentCulture.NumberFormat.CurrencySymbol = "$";
        currentCulture.NumberFormat.CurrencyGroupSeparator = ",";
        currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
        CultureInfo.CurrentCulture = currentCulture;

        var converter = new CurrencyConverter();
        decimal value = 0m;

        // Act
        var result = converter.Convert(value, typeof(string), null, null) as string;

        // Assert
        Assert.IsNotNull(result);

        var expected = 0m.ToString("C", CultureInfo.CurrentCulture);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public void CurrencyConverter_ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new CurrencyConverter();

        // Act
        converter.ConvertBack("$ 1,234.56", typeof(decimal), null, null);

        // Assert is handled by ExpectedException
    }
}

