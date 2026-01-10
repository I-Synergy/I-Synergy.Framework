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
    /// Converts decimal to string for display.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">Optional: Pass "formatted" to return currency-formatted value.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>String representation for entry binding.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is decimal decimalValue)
        {
            if (decimalValue == 0m)
                return string.Empty;

            // Return raw value for editing - the behavior will format with currency symbol on blur
            // The behavior handles the formatting lifecycle (raw during edit, formatted on blur)
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
            // Try to parse as currency first (handles formatted values with currency symbols)
            if (decimal.TryParse(stringValue, NumberStyles.Currency, culture, out var currencyResult))
            {
                return currencyResult;
            }
            
            // Fallback: Remove currency symbols and other formatting - accept any valid input
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
