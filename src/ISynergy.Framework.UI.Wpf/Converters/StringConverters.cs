using System;
using ISynergy.Framework.UI.Helpers;
using ISynergy.Framework.UI.Extensions;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xaml;
using System.Windows.Media.Imaging;

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class StringFormatConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringFormatConverter : IValueConverter
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
            if (value is null)
                return null;

            if (parameter is null)
                return value;

            return string.Format((string)parameter, value);
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
    /// Class StringToBooleanConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringToBooleanConverter : IValueConverter
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
            if (parameter.ToString().Equals(value.ToString()))
            {
                return true;
            }
            else
            {
                return false;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return parameter.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Class StringToVisibilityConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringToVisibilityConverter : IValueConverter
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
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
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
    /// Class StringToEnabledConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringToEnabledConverter : IValueConverter
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
            if (string.IsNullOrEmpty((string)value))
            {
                return false;
            }
            else
            {
                return true;
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
    /// Class StringToInvertEnabledConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringToInvertEnabledConverter : IValueConverter
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
            if (string.IsNullOrEmpty((string)value))
            {
                return true;
            }
            else
            {
                return false;
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
    /// Class StringToDecimalConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringToDecimalConverter : IValueConverter
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
            if (decimal.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            else
            {
                return 0;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Class StringToIntegerConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class StringToIntegerConverter : IValueConverter
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
            if (int.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            else
            {
                return 0;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// Class UriToImageSourceConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class UriToImageSourceConverter : IValueConverter
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
            if (string.IsNullOrEmpty(value?.ToString()))
                return null;

            return new BitmapImage
            {
                CreateOptions = BitmapCreateOptions.IgnoreImageCache,
                UriSource = new Uri(value as string)
            };
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
    /// Converts hex string to color.
    /// </summary>
    public class StringToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string color && string.IsNullOrEmpty(color))
                return ColorHelper.HexStringToColor(color);

            return null;
        }

        /// <summary>
        /// Convert back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts hex string to color.
    /// </summary>
    public class StringToColorBrushConverter : IValueConverter
    {
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string color && string.IsNullOrEmpty(color))
            {
                return new SolidColorBrush(ColorHelper.HexStringToColor(color));
            }

            return null;
        }

        /// <summary>
        /// Convert back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
