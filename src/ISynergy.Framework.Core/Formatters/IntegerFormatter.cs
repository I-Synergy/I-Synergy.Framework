using ISynergy.Framework.Core.Formatters.Base;
using System.Globalization;

namespace ISynergy.Framework.Core.Formatters;

/// <summary>
/// Helper class for integer formatting logic.
/// </summary>
public sealed class IntegerFormatter : NumericFormatter
{
    public IntegerFormatter(CultureInfo culture)
        : base(culture)
    {
    }

    public override string FormatValue(decimal value)
    {
        return value.ToString("N0", _culture);
    }

    public bool IsInteger(decimal value)
    {
        return value == Math.Truncate(value);
    }
}