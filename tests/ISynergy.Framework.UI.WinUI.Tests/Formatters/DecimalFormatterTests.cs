using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace ISynergy.Framework.UI.Formatters.Tests;

[TestClass]
public class DecimalFormatterTests
{
    private CultureInfo _originalCulture;

    public DecimalFormatterTests()
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
    public void Constructor_WithDefaultParameters_SetsDefaultDecimals()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        // Act
        var formatter = new DecimalFormatter();

        // Assert
        Assert.AreEqual(2, formatter.Decimals);
    }

    [TestMethod]
    public void Constructor_WithCustomDecimals_SetsSpecifiedDecimals()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        // Act
        var formatter = new DecimalFormatter(4);

        // Assert
        Assert.AreEqual(4, formatter.Decimals);
    }

    [TestMethod]
    public void Constructor_WithNullServices_UsesSystemDefaults()
    {
        // Arrange
        // Act
        var formatter = new DecimalFormatter();

        // Assert
        Assert.AreEqual(2, formatter.Decimals);
    }

    [TestMethod]
    public void FormatDouble_WithInvariantCulture_FormatsCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter(2);
        var value = 1234.5678;

        // Act
        var result = formatter.FormatDouble(value);

        // Assert
        Assert.AreEqual("1,234.57", result);
    }

    [TestMethod]
    public void FormatDouble_WithCustomCulture_FormatsCorrectly()
    {
        // Arrange
        var currentCulture = new CultureInfo("nl-NL");
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter(2);
        var value = 1234.5678;

        // Act
        var result = formatter.FormatDouble(value);

        // Assert
        Assert.AreEqual("1.234,57", result);
    }

    [TestMethod]
    public void FormatInt_WithInvariantCulture_FormatsCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter(2);
        long value = 1234567;

        // Act
        var result = formatter.FormatInt(value);

        // Assert
        Assert.AreEqual("1,234,567.00", result);
    }

    [TestMethod]
    public void FormatUInt_WithInvariantCulture_FormatsCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter(2);
        ulong value = 1234567;

        // Act
        var result = formatter.FormatUInt(value);

        // Assert
        Assert.AreEqual("1,234,567.00", result);
    }

    [TestMethod]
    public void ParseDouble_WithInvariantCulture_ParsesCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();
        var text = "1,234.56";

        // Act
        var result = formatter.ParseDouble(text);

        // Assert
        Assert.AreEqual(1234.56, result);
    }

    [TestMethod]
    public void ParseDouble_WithCustomCulture_ParsesCorrectly()
    {
        // Arrange
        var currentCulture = new CultureInfo("nl-NL");
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();
        var text = "1.234,56";

        // Act
        var result = formatter.ParseDouble(text);

        // Assert
        Assert.AreEqual(1234.56, result);
    }

    [TestMethod]
    public void ParseDouble_WithEmptyString_ReturnsZero()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();

        // Act
        var result = formatter.ParseDouble("");

        // Assert
        Assert.AreEqual(0d, result);
    }

    [TestMethod]
    public void ParseDouble_WithInvalidFormat_ReturnsZero()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();

        // Act
        var result = formatter.ParseDouble("invalid");

        // Assert
        Assert.AreEqual(0d, result);
    }

    [TestMethod]
    public void ParseInt_WithInvariantCulture_ParsesCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();
        var text = "1,234";

        // Act
        var result = formatter.ParseInt(text);

        // Assert
        Assert.AreEqual(1234L, result);
    }

    [TestMethod]
    public void ParseUInt_WithInvariantCulture_ParsesCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();
        var text = "1,234";

        // Act
        var result = formatter.ParseUInt(text);

        // Assert
        Assert.AreEqual(1234UL, result);
    }

    [TestMethod]
    public void ParseDouble_WithLeadingDecimalSeparator_ParsesCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();
        var text = ".56";

        // Act
        var result = formatter.ParseDouble(text);

        // Assert
        Assert.AreEqual(0.56, result);
    }

    [TestMethod]
    public void ParseInt_WithDecimalValue_RoundsCorrectly()
    {
        // Arrange
        var currentCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        CultureInfo.CurrentCulture = currentCulture;

        var formatter = new DecimalFormatter();

        // Act & Assert
        Assert.AreEqual(1235L, formatter.ParseInt("1234.6")); // Round up
        Assert.AreEqual(1234L, formatter.ParseInt("1234.4")); // Round down
        Assert.AreEqual(1234L, formatter.ParseInt("1234.0")); // Exact integer
    }
}