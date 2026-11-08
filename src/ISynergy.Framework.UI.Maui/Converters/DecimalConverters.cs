using ISynergy.Framework.UI.Converters.Shared;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class DecimalToVisibilityConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalToVisibilityConverter : IValueConverter
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
        if (value is decimal decimalValue && decimalValue != 0)
        {
            return true;
        }

        return false;
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
/// Class ZeroDecimalToVisibilityConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ZeroDecimalToVisibilityConverter : IValueConverter
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
        if (value is decimal decimalValue && decimalValue == 0)
        {
            return true;
        }

        return false;
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
/// Class DecimalEqualsConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalEqualsConverter : IValueConverter
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
        if (decimal.TryParse(parameter?.ToString(), out var limit) && decimal.TryParse(value?.ToString(), out var test) && test == limit)
            return true;

        return false;
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
/// Class DecimalLesserThanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalLesserThanConverter : IValueConverter
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
        if (decimal.TryParse(parameter?.ToString(), out var limit) && decimal.TryParse(value?.ToString(), out var test) && test < limit)
            return true;

        return false;
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
/// Class DecimalGreaterThanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalGreaterThanConverter : IValueConverter
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
        if (decimal.TryParse(parameter?.ToString(), out var limit) && decimal.TryParse(value?.ToString(), out var test) && test > limit)
            return true;

        return false;
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
/// Class DecimalEqualsOrLesserThanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalEqualsOrLesserThanConverter : IValueConverter
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
        if (decimal.TryParse(parameter?.ToString(), out var limit) && decimal.TryParse(value?.ToString(), out var test) && test <= limit)
            return true;

        return false;
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
/// Class DecimalEqualsOrGreaterThanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalEqualsOrGreaterThanConverter : IValueConverter
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
        if (decimal.TryParse(parameter?.ToString(), out var limit) && decimal.TryParse(value?.ToString(), out var test) && test >= limit)
            return true;

        return false;
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
/// Class DecimalToStringConverter - Two-way converter for decimal values with Entry.Text binding.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalToStringConverter : IValueConverter
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
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            // Parse without strict formatting - accept any valid decimal input
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
/// Class DecimalToDoubleConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class DecimalToDoubleConverter : IValueConverter
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
        return DecimalToDoubleConverterBase.Convert(value);
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DecimalToDoubleConverterBase.ConvertBack(value);
    }
}
