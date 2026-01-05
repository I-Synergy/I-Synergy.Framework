using ISynergy.Framework.Core.Formatters.Base;
using System.Globalization;

namespace ISynergy.Framework.Core.Formatters;

/// <summary>
/// Helper class for currency formatting and parsing logic.
/// Separated from behavior for testability.
/// </summary>
public sealed class CurrencyFormatter : NumericFormatter
{
    private readonly int _decimalPlaces;

    public CurrencyFormatter(CultureInfo culture, int decimalPlaces = 2)
        : base(culture)
    {
        _decimalPlaces = decimalPlaces;
    }

    /// <summary>
    /// Cleans input by removing currency symbols and group separators.
    /// </summary>
    public override string CleanInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var cleaned = input;

        // Remove currency symbols first
        cleaned = cleaned.Replace(_culture.NumberFormat.CurrencySymbol, string.Empty);
        cleaned = cleaned.Trim();

        // Remove group separators BEFORE normalizing decimal separators
        // This is crucial because group separators might be in the normalization list
        cleaned = cleaned.Replace(_culture.NumberFormat.CurrencyGroupSeparator, string.Empty);
        cleaned = cleaned.Replace(_culture.NumberFormat.NumberGroupSeparator, string.Empty);

        // Now normalize decimal separators
        cleaned = NormalizeDecimalSeparators(cleaned);

        return cleaned;
    }

    /// <summary>
    /// Formats a decimal value as currency.
    /// </summary>
    public override string FormatValue(decimal value)
    {
        return value.ToString($"C{_decimalPlaces}", _culture);
    }

    /// <summary>
    /// Attempts to parse cleaned input into a decimal value.
    /// </summary>
    public bool TryParse(string input, out decimal value)
    {
        value = 0;

        var cleanedInput = CleanInput(input);

        if (string.IsNullOrWhiteSpace(cleanedInput))
            return false;

        return decimal.TryParse(
            cleanedInput,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            _culture,
            out value);
    }

    /// <summary>
    /// Validates the number of decimal places in a value.
    /// </summary>
    public bool HasValidDecimalPlaces(decimal value)
    {
        var decimalPlaces = GetDecimalPlaces(value);
        return decimalPlaces <= _decimalPlaces;
    }

    private string NormalizeDecimalSeparators(string input)
    {
        var decimalSeparator = _culture.NumberFormat.NumberDecimalSeparator;

        // Build list of alternative separators, excluding the current culture's group separator
        var groupSeparator = _culture.NumberFormat.NumberGroupSeparator;
        var alternativeSeparators = new[] { ".", ",", "·", "٫" }
            .Where(s => s != decimalSeparator && s != groupSeparator)
            .ToArray();

        var normalized = input;
        foreach (var separator in alternativeSeparators)
        {
            normalized = normalized.Replace(separator, decimalSeparator);
        }

        return normalized;
    }

    private static int GetDecimalPlaces(decimal value)
    {
        return BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
    }
}