using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var currencySymbol = "$";

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out var amount))
                {
                    return SimpleIoc.Default.GetInstance<IConverterService>().ConvertDecimalToCurrency(amount);
                }
            }

            var info = CultureInfo.CurrentCulture.NumberFormat;
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NegativeCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var currencySymbol = "$";

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out var amount))
                {
                    return SimpleIoc.Default.GetInstance<IConverterService>().ConvertDecimalToCurrency(amount * -1);
                }
            }

            var info = CultureInfo.CurrentCulture.NumberFormat;
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
