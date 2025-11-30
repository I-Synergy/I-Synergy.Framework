using ISynergy.Framework.UI.Extensions;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class StringFormatConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (parameter is null)
            return value;

        return string.Format((string)parameter, value);
    }

    /// <summary>
    /// Converts the back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class StringToBooleanConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToBooleanConverter : IValueConverter
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
        var parameterString = parameter?.ToString();
        var valueString = value?.ToString();

        if (parameterString is not null && valueString is not null && parameterString.Equals(valueString))
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
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Only update the bound value when this is checked/true
        if (value is bool isChecked && isChecked)
            return parameter?.ToString() ?? string.Empty;

        // Return DoNothing to prevent unchecked controls from updating the binding
        // Note: Binding.DoNothing is of type object and is a valid return value
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class StringToIsVisibleConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToIsVisibleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts the back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class StringToEnabledConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToEnabledConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value as string))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Converts the back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class StringToInvertEnabledConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToInvertEnabledConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value as string))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts the back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class StringToDecimalConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToDecimalConverter : IValueConverter
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
        if (decimal.TryParse(value?.ToString(), out var result))
        {
            return result;
        }

        return 0m;
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
        if (decimal.TryParse(value?.ToString(), out var result))
            return result;
        return 0m;
    }
}

/// <summary>
/// Class StringToDecimalConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToDoubleConverter : IValueConverter
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
        if (double.TryParse(value?.ToString(), out var result))
        {
            return result;
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
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (double.TryParse(value?.ToString(), out var result))
            return result;
        return 0d;
    }
}

/// <summary>
/// Class StringToIntegerConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToIntegerConverter : IValueConverter
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
        if (int.TryParse(value?.ToString(), out var result))
        {
            return result;
        }

        return 0;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString();
    }
}

/// <summary>
/// Converts hex string to color.
/// </summary>
public class StringToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string color && !string.IsNullOrEmpty(color))
            return Color.FromArgb(color);

        return null;
    }

    /// <summary>
    /// Convert back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

public class StringToUriConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (Uri.TryCreate(value?.ToString(), UriKind.RelativeOrAbsolute, out Uri? uri))
            return uri;

        return null;
    }

    /// <summary>
    /// Convert back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class UriToImageSourceConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class UriToImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            return null;

        return ImageSource.FromUri(new Uri(value as string ?? string.Empty));
    }

    /// <summary>
    /// Converts the back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Converts hex string to color brush.
/// </summary>
public class StringToColorBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty(value?.ToString()))
        {
            try
            {
                var color = Color.FromArgb(value.ToString()!);
                return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(Color.FromArgb("#FF0078D7"));
            }
        }

        return new SolidColorBrush(Color.FromArgb("#FF0078D7"));
    }

    /// <summary>
    /// Convert back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

/// <summary>
/// Class StringToGeometryConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StringToGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.ToString().ToGeometry();

    /// <summary>
    /// Converts the back. This converter is read-only and does not support backwards conversion.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
