using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Helpers;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class SolidColorBrushToHexStringConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class SolidColorBrushToHexStringConverter : IValueConverter
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
            Argument.IsNotNull(nameof(value), value);
            return ((SolidColorBrush)value).Color.ToString();
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
    /// Class IntegerToBrushConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class IntegerToBrushConverter : IValueConverter
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
            if(value is int color && color != 0)
            {
                return new SolidColorBrush(ImageHelper.ConvertInteger2Color(color));
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

    /// <summary>
    /// Class IntegerToColorConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class IntegerToColorConverter : IValueConverter
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
            Argument.IsNotNull(nameof(value), value);
            return ImageHelper.ConvertInteger2Color((int)value);
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
            Argument.IsNotNull(nameof(value), value);
            return ImageHelper.ConvertColor2Integer((Color)value);
        }
    }
}
