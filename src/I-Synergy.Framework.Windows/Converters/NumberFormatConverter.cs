using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class DecimalNumberFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalNumber)
            {
                return decimalNumber.ToString("N", CultureInfo.CurrentCulture);
            }

            return 0m.ToString("N", CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }

            return 0m;
        }
    }

    public class IntegerNumberFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intNumber)
            {
                return intNumber.ToString();
            }

            return 0.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrEmpty(value.ToString()) && int.TryParse(value.ToString(), out int result))
            {
                return result;
            }

            return 0;
        }
    }
}
