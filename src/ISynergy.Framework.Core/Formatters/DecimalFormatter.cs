using ISynergy.Framework.Core.Formatters.Base;
using System.Globalization;

namespace ISynergy.Framework.Core.Formatters;

/// <summary>
/// Helper class for decimal formatting and parsing logic.
/// </summary>
public sealed class DecimalFormatter : NumericFormatter
{
    private readonly int _decimalPlaces;

    public DecimalFormatter(CultureInfo culture, int decimalPlaces = 2)
        : base(culture)
    {
        _decimalPlaces = decimalPlaces;
    }

    public override string FormatValue(decimal value)
    {
        return value.ToString($"N{_decimalPlaces}", _culture);
    }

    /// <summary>
    /// Cleans and normalizes decimal input, preserving the fractional part.
    /// </summary>
    public override string CleanInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var decimalSeparator = _culture.NumberFormat.NumberDecimalSeparator;
        var groupSeparator = _culture.NumberFormat.NumberGroupSeparator;
        
        // Remove group separators
        var cleaned = input.Replace(groupSeparator, string.Empty).Trim();
        
        // Normalize alternative decimal separators to culture-specific separator
        var alternativeSeparators = new[] { ".", ",", "·", "٫" }
            .Where(s => s != decimalSeparator && s != groupSeparator)
            .ToArray();

        foreach (var separator in alternativeSeparators)
        {
            cleaned = cleaned.Replace(separator, decimalSeparator);
        }

        // Split on the decimal separator
        if (cleaned.Contains(decimalSeparator))
        {
            var parts = cleaned.Split(new[] { decimalSeparator }, StringSplitOptions.None);
            
            if (parts.Length > 2)
            {
                // Multiple decimal separators - take first two parts only
                parts = new[] { parts[0], parts[1] };
            }

            // Preserve integer part and fractional part (do NOT truncate here)
            var integerPart = parts[0];
            var fractionalPart = parts.Length > 1 ? parts[1] : string.Empty;
            
            // Rejoin with culture-specific decimal separator
            if (!string.IsNullOrEmpty(fractionalPart))
            {
                return integerPart + decimalSeparator + fractionalPart;
            }
            
            return integerPart;
        }

        return cleaned;
    }

    public bool HasValidDecimalPlaces(decimal value)
    {
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
        return decimalPlaces <= _decimalPlaces;
    }
}
