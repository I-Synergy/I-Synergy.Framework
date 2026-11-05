using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Globalization.Abstractions.Services;
using System.Globalization;
using System.Text;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class ChangeTrackingConverters.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ChangeTrackingConverters : IValueConverter
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
        var languageService = IPlatformApplication.Current!.Services.GetRequiredService<ILanguageService>();
        return ChangeTrackingConverter.ToDisplayText(value, languageService);
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
