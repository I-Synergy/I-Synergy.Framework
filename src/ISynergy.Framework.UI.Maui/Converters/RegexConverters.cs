using ISynergy.Framework.Core.Utilities;
using System.Globalization;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Mask to Regex converter.
/// </summary>
public class MaskToRegexConverter : IValueConverter
{
    /// <summary>
    /// Converts Mask string to Regex.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var valueString = value?.ToString();
        if (!string.IsNullOrEmpty(valueString))
            return RegexUtility.MaskToRegexConverter(valueString);

        return null;
    }

    /// <summary>
    /// Converts regex back to Mask string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
