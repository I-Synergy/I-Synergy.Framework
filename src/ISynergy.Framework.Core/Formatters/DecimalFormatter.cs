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

    public bool HasValidDecimalPlaces(decimal value)
    {
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
        return decimalPlaces <= _decimalPlaces;
    }
}
