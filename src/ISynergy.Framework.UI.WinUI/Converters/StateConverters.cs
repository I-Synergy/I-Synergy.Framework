using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class StateToBrushConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StateToBrushConverter : IValueConverter
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
        if (!(value is bool))
        {
            return Application.Current?.Resources.TryGetValue("TertiaryDarkBrush", out var fallbackBrush) == true && fallbackBrush is SolidColorBrush solidFallbackBrush
                ? solidFallbackBrush
                : new SolidColorBrush(Microsoft.UI.Colors.Gray);
        }

        if ((bool)value)
        {
            return Application.Current?.Resources.TryGetValue("PrimaryMediumBrush", out var activeBrush) == true && activeBrush is SolidColorBrush solidActiveBrush
                ? solidActiveBrush
                : new SolidColorBrush(Microsoft.UI.Colors.Gray);
        }

        return Application.Current?.Resources.TryGetValue("TertiaryDarkBrush", out var fallbackBrush2) == true && fallbackBrush2 is SolidColorBrush solidFallbackBrush2
            ? solidFallbackBrush2
            : new SolidColorBrush(Microsoft.UI.Colors.Gray);
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

/// <summary>
/// Class StateToOpacityConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class StateToOpacityConverter : IValueConverter
{
    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="language">The language.</param>
    /// <returns>System.Object.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (!(value is bool))
        {
            return 0.2;
        }

        if ((bool)value)
        {
            return 1;
        }

        return 0.2;
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
