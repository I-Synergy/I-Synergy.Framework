using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Converters
{
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
        /// <param name="culture"></param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not null && parameter is string)
            {
                return Geometry.Parse(Application.Current.Resources[parameter] as string);
            }
            else
            {
                return Geometry.Empty;
            }
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture"></param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
        /// <param name="culture"></param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not null && parameter is string)
            {
                return Application.Current.Resources[parameter] as string;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture"></param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
