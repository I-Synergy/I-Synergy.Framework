using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class IntegerToIsVisibleConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class IntegerToIsVisibleConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((int)value == 0 || value is null)
        {
            return false;
        }
        else
        {
            return true;
        }
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
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Class ZeroIntegerToIsVisibleConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ZeroIntegerToIsVisibleConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue && intValue == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
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
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Class IntegerToStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class IntegerToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null)
        {
            return value.ToString();
        }
        else
        {
            return "0";
        }
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null && int.TryParse(value.ToString(), out int result))
            return result;

        return 0;
    }
}

/// <summary>
/// Class IntegerToDoubleConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class IntegerToDoubleConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return System.Convert.ToDouble(intValue);
        }

        return 0d;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            return System.Convert.ToInt32(doubleValue);
        }

        return 0;
    }
}
