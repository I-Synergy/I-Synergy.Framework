﻿using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.UI.Extensions;
using Microsoft.UI.Xaml.Data;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class BytesToImageSourceConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class BytesToImageSourceConverter : IValueConverter
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
        if (value is byte[] bytes)
        {
            return bytes.ToImageSource();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">Type of the target.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="language">The language.</param>
    /// <returns>System.Object.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return new NotImplementedException();
    }
}

/// <summary>
/// Class ByteArrayToStreamConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class ByteArrayToStreamConverter : IValueConverter
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
        Stream result = null;

        if (value is byte[] bytes)
        {
            result = bytes.ToMemoryStream();
        }

        return result;
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
