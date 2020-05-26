using System;
using ISynergy.Framework.Core.Abstractions;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class DecimalFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var decimalFormatter = new DecimalFormatter();

            if (value is IContext context && context.NumberFormat != null)
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
