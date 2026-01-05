using System.Globalization;

namespace ISynergy.Framework.Core.Formatters.Tests;

[TestClass]
public sealed class CurrencyFormatterTests
{
    [TestMethod]
    public void Constructor_NullCulture_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CurrencyFormatter(null!));
    }

    [TestMethod]
    public void CleanInput_RemovesDollarSign()
    {
        // Arrange
        var formatter = new CurrencyFormatter(new CultureInfo("en-US"));

        // Act
        var result = formatter.CleanInput("$123.45");

        // Assert
        Assert.IsFalse(result.Contains("$"));
        Assert.IsTrue(result.Contains("123.45"));
    }

    [TestMethod]
    public void CleanInput_RemovesGroupSeparator()
    {
        // Arrange
        var formatter = new CurrencyFormatter(new CultureInfo("en-US"));

        // Act
        var result = formatter.CleanInput("$1,234.56");

        // Assert
        Assert.IsFalse(result.Contains("$"));
        Assert.IsFalse(result.Contains(","));
        Assert.IsTrue(result.Contains("1234.56"));
    }

    [TestMethod]
    public void CleanInput_RemovesEuroSymbol()
    {
        // Arrange
        var formatter = new CurrencyFormatter(new CultureInfo("nl-NL"));

        // Act
        var result = formatter.CleanInput("€ 123,45");

        // Assert
        Assert.IsFalse(result.Contains("€"));
    }

    [TestMethod]
    public void FormatValue_USD_DisplaysDollarSign()
    {
        // Arrange
        var formatter = new CurrencyFormatter(new CultureInfo("en-US"), 2);

        // Act
        var result = formatter.FormatValue(123.45m);

        // Assert
        Assert.IsTrue(result.Contains("$"));
        Assert.IsTrue(result.Contains("123.45"));
    }

    [TestMethod]
    public void FormatValue_EUR_DisplaysEuroSign()
    {
        // Arrange
        var formatter = new CurrencyFormatter(new CultureInfo("nl-NL"), 2);

        // Act
        var result = formatter.FormatValue(1234.56m);

        // Assert
        Assert.IsTrue(result.Contains("€") || result.Contains("EUR"));
        Assert.IsTrue(result.Contains("1") && result.Contains("234"));
    }

    [TestMethod]
    public void FormatValue_RespectsDecimalPlaces()
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture, 3);

        // Act
        var result = formatter.FormatValue(123.4m);

        // Assert
        Assert.IsTrue(result.Contains("123.400"));
    }

    [TestMethod]
    [DataRow("123.45", 123.45)]
    [DataRow("0.99", 0.99)]
    [DataRow("1000.00", 1000.00)]
    public void TryParse_ValidInput_ReturnsTrue(string input, double expected)
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture);

        // Act
        var success = formatter.TryParse(input, out var value);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual((decimal)expected, value);
    }

    [TestMethod]
    public void TryParse_DutchFormat_ParsesCorrectly()
    {
        // Arrange
        var formatter = new CurrencyFormatter(new CultureInfo("nl-NL"));

        // Act
        var success = formatter.TryParse("€ 123,45", out var value);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual(123.45m, value);
    }

    [TestMethod]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture);

        // Act
        var success = formatter.TryParse("abc", out var value);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual(0m, value);
    }

    [TestMethod]
    public void TryParse_EmptyInput_ReturnsFalse()
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture);

        // Act
        var success = formatter.TryParse(string.Empty, out var value);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual(0m, value);
    }

    [TestMethod]
    public void HasValidDecimalPlaces_TooManyPlaces_ReturnsFalse()
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture, 2);

        // Act
        var result = formatter.HasValidDecimalPlaces(123.456m);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasValidDecimalPlaces_CorrectPlaces_ReturnsTrue()
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture, 2);

        // Act
        var result = formatter.HasValidDecimalPlaces(123.45m);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasValidDecimalPlaces_FewerPlaces_ReturnsTrue()
    {
        // Arrange
        var formatter = new CurrencyFormatter(CultureInfo.InvariantCulture, 3);

        // Act
        var result = formatter.HasValidDecimalPlaces(123.4m);

        // Assert
        Assert.IsTrue(result);
    }
}
