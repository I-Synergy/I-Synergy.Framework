using System.Globalization;
using ISynergy.Framework.Core.Formatters;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class CurrencyConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class CurrencyConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (decimal.TryParse(value?.ToString(), out var amount))
            return amount.ToString("C2");

        return 0m.ToString("C2");
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Class CurrencyToStringConverter - Two-way converter for currency values with Entry.Text binding.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class CurrencyToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts decimal to string without formatting (raw value).
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">Optional: Not used - behavior handles formatting.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Raw string representation or empty string for zero values.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            if (decimalValue == 0m)
                return string.Empty;

            // Return raw value without formatting - behavior will format on blur
            return decimalValue.ToString(culture);
        }
        return string.Empty;
    }

    /// <summary>
    /// Converts string back to decimal, accepting currency formatting.
    /// </summary>
    /// <param name="value">The currency formatted or raw string.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Decimal value or 0 if parsing fails.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            // Remove currency symbols and other formatting - accept any valid input
            var cleaned = new string(stringValue.Where(c => 
                char.IsDigit(c) || 
                c.ToString() == culture.NumberFormat.NumberDecimalSeparator || 
                c == '-' ||
                c == '.' ||
                c == ',').ToArray());
            
            // Normalize decimal separator
            cleaned = cleaned.Replace(",", culture.NumberFormat.NumberDecimalSeparator);
            cleaned = cleaned.Replace(".", culture.NumberFormat.NumberDecimalSeparator);
            
            if (decimal.TryParse(cleaned, NumberStyles.Number, culture, out var result))
            {
                return result;
            }
        }

        return 0m;
    }
}

/// <summary>
/// Class NegativeCurrencyConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class NegativeCurrencyConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (decimal.TryParse(value?.ToString(), out var amount))
            return (amount * -1).ToString("C2");

        return 0m.ToString("C2");
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
