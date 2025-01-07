using System.Globalization;
using Windows.Globalization.NumberFormatting;

namespace ISynergy.Framework.UI.Formatters;

public class DecimalFormatter : INumberFormatter2, INumberParser
{
    public int Decimals { get; set; } = 2;

    public DecimalFormatter()
    {
    }

    public DecimalFormatter(int decimals)
        : this()
    {
        Decimals = decimals;
    }

    public string FormatDouble(double value) =>
        value.ToString($"N{Decimals}", CultureInfo.CurrentCulture.NumberFormat);

    public string FormatInt(long value) =>
        value.ToString($"N{Decimals}", CultureInfo.CurrentCulture.NumberFormat);

    public string FormatUInt(ulong value) =>
        value.ToString($"N{Decimals}", CultureInfo.CurrentCulture.NumberFormat);

    public double? ParseDouble(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0d;

        try
        {
            var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            var decimalSeparator = numberFormat.NumberDecimalSeparator;
            var groupSeparator = numberFormat.NumberGroupSeparator;

            if (text.Contains(groupSeparator))
                text = text.Replace(groupSeparator, decimalSeparator);

            var charposition = text.LastIndexOf(decimalSeparator);

            if (charposition != -1)
            {
                var originalPosition = text.Length - charposition;
                text = text.Replace(decimalSeparator, "");
                text = text.Insert(text.Length - (originalPosition - 1), decimalSeparator);

                if (text.StartsWith(decimalSeparator))
                    text = text.Insert(0, "0");
            }

            if (double.TryParse(text, numberFormat, out double result))
                return result;
        }
        catch
        {
            // If any parsing error occurs, return default value
        }

        return 0d;
    }

    public long? ParseInt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0L;

        try
        {
            var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            var decimalSeparator = numberFormat.NumberDecimalSeparator;
            var groupSeparator = numberFormat.NumberGroupSeparator;

            // Remove group separators first
            if (text.Contains(groupSeparator))
                text = text.Replace(groupSeparator, "");

            // Handle decimal part if present
            var charposition = text.LastIndexOf(decimalSeparator);
            if (charposition != -1)
            {
                // Convert to double and round
                if (double.TryParse(text, numberFormat, out double doubleValue))
                {
                    return (long)Math.Round(doubleValue, 0, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                // Direct integer parse if no decimal
                if (long.TryParse(text, numberFormat, out long result))
                    return result;
            }
        }
        catch
        {
            // If any parsing error occurs, return default value
        }

        return 0L;
    }

    public ulong? ParseUInt(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0UL;

        try
        {
            var numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            var decimalSeparator = numberFormat.NumberDecimalSeparator;
            var groupSeparator = numberFormat.NumberGroupSeparator;

            // Remove group separators first
            if (text.Contains(groupSeparator))
                text = text.Replace(groupSeparator, "");

            // Handle decimal part if present
            var charposition = text.LastIndexOf(decimalSeparator);
            if (charposition != -1)
            {
                // Convert to double and round
                if (double.TryParse(text, numberFormat, out double doubleValue))
                {
                    if (doubleValue < 0)
                        return 0UL;

                    return (ulong)Math.Round(doubleValue, 0, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                // Direct unsigned integer parse if no decimal
                if (ulong.TryParse(text, numberFormat, out ulong result))
                    return result;
            }
        }
        catch
        {
            // If any parsing error occurs, return default value
        }

        return 0UL;
    }
}
