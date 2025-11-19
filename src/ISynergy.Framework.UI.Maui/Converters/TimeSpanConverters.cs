using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System.Globalization;
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
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            var result = new StringBuilder();

            if (timeSpan.Days > 0)
            {
                result.Append($"{timeSpan.Days} {ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString("Day_s")}, ");
            }

            result.Append(
                $"{timeSpan.Hours} {ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString("Hour_s")} " +
                $"{ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString("And")} " +
                $"{timeSpan.Minutes} {ServiceLocator.Default.GetRequiredService<ILanguageService>().GetString("Minute_s")}");

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
    /// <param name="culture">The culture.</param>
    /// <returns>System.Object.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
