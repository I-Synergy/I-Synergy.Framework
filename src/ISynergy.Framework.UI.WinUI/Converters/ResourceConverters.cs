using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class ResourceNameToGeometryConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ResourceNameToGeometryConverter : IValueConverter
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
        if (parameter is string resourceKey)
        {
            if (Application.Current?.Resources.TryGetValue(resourceKey, out var geometryData) == true && geometryData is string geometryString)
            {
                return (Geometry)XamlReader.Load($"<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{geometryString}</Geometry>");
            }
            return (Geometry)XamlReader.Load("<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><EllipseGeometry/></Geometry>");
        }

        return (Geometry)XamlReader.Load("<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'></Geometry>");
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
/// Class ResourceNameToStringConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ResourceNameToStringConverter : IValueConverter
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
        if (parameter is string resourceKey)
        {
            return Application.Current?.Resources.TryGetValue(resourceKey, out var resourceValue) == true
                ? resourceValue as string
                : null;
        }

        return string.Empty;
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
