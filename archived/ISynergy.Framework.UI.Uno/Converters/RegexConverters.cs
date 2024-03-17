using ISynergy.Framework.Core.Utilities;
using Microsoft.UI.Xaml.Data;

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
    /// <param name="language"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not null && !string.IsNullOrEmpty(value.ToString()))
            return RegexUtility.MaskToRegexConverter(value.ToString());

        return null;
    }

    /// <summary>
    /// Converts regex back to Mask string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
