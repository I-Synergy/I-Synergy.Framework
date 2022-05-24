using ISynergy.Framework.Core.Abstractions;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Windows.Globalization.NumberFormatting;

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class DecimalFormatConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class DecimalFormatConverter : IValueConverter
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
            var decimalFormatter = new DecimalFormatter();

            if (value is IContext context && context.NumberFormat is not null)
            {
                decimalFormatter.FractionDigits = context.NumberFormat.NumberDecimalDigits;
            }
            else
            {
                decimalFormatter.FractionDigits = 2;
            }

            decimalFormatter.IntegerDigits = 1;

            return decimalFormatter;
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
