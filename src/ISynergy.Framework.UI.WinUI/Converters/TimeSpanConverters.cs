using ISynergy.Framework.Core.Services;
using Microsoft.UI.Xaml.Data;
using System.Text;

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
    /// <param name="language">The language.</param>
    /// <returns>System.Object.</returns>
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            var result = new StringBuilder();

            if (timeSpan.Days > 0)
            {
                result.Append($"{timeSpan.Days} {LanguageService.Default.GetString("Day_s")}, ");
            }

            result.Append(
                $"{timeSpan.Hours} {LanguageService.Default.GetString("Hour_s")} " +
                $"{LanguageService.Default.GetString("And")} " +
                $"{timeSpan.Minutes} {LanguageService.Default.GetString("Minute_s")}");

            return result.ToString();
        }

        return null;
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="language">The language.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
