using ISynergy.Framework.Core.Utilities;
using System;
using System.Globalization;

#if NET5_0 && WINDOWS
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace ISynergy.Framework.UI.Converters
{
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
        public object Convert(object value, Type targetType, object parameter, string culture)
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
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
