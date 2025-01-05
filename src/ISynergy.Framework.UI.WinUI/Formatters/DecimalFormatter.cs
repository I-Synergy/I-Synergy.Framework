using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System.Globalization;
using Windows.Globalization.NumberFormatting;

namespace ISynergy.Framework.UI.Formatters;

public class DecimalFormatter : INumberFormatter2, INumberParser
{
    private readonly IContext _context;
    private readonly NumberFormatInfo _numberFormat;

    public int Decimals { get; set; }

    public DecimalFormatter()
    {
        try
        {
            var scopedContextService = ServiceLocator.Default.GetService<IScopedContextService>();
            _context = scopedContextService?.GetService<IContext>();
            _numberFormat = _context?.NumberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
            Decimals = _context?.NumberFormat?.CurrencyDecimalDigits ?? 2;
        }
        catch
        {
            // Fallback to system defaults if service resolution fails
            _numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            Decimals = 2;
        }
    }

    public DecimalFormatter(int decimals)
        : this()
    {
        Decimals = decimals;
    }

    public string FormatDouble(double value)
    {
        if (_numberFormat == null)
            return value.ToString($"N{Decimals}");

        return value.ToString($"N{Decimals}", _numberFormat);
    }

    public string FormatInt(long value)
    {
        if (_numberFormat == null)
            return value.ToString($"N{Decimals}");

        return value.ToString($"N{Decimals}", _numberFormat);
    }

    public string FormatUInt(ulong value)
    {
        if (_numberFormat == null)
            return value.ToString($"N{Decimals}");

        return value.ToString($"N{Decimals}", _numberFormat);
    }

    public double? ParseDouble(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0d;

        try
        {
            var numberFormat = _numberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
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
            var numberFormat = _numberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
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
            var numberFormat = _numberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
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
