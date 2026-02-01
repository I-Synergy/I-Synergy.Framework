using System.Globalization;
using System.Windows.Data;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class TimeSpanToStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class TimeSpanToStringConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The language.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
            return timeSpan.ToString();

        return TimeSpan.FromMinutes(5).ToString();
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The language.</param>
    /// <returns>System.Object.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var valueString = value?.ToString();
        if (!string.IsNullOrEmpty(valueString) && TimeSpan.TryParse(valueString, out var result))
            return result;

        return TimeSpan.FromMinutes(5);
    }
}
