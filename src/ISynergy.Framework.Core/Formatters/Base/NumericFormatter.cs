using System.Globalization;

namespace ISynergy.Framework.Core.Formatters.Base;

public abstract class NumericFormatter
{
    protected readonly CultureInfo _culture;

    public NumericFormatter(CultureInfo culture)
    {
        _culture = culture ?? throw new ArgumentNullException(nameof(culture));
    }

    public abstract string FormatValue(decimal value);

    public virtual string CleanInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var decimalSeparator = _culture.NumberFormat.NumberDecimalSeparator;
        if (input.Contains(decimalSeparator))
        {
            var parts = input.Split(decimalSeparator);
            return parts[0];
        }

        return input;
    }
}
