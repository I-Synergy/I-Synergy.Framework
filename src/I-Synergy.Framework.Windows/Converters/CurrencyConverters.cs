using GalaSoft.MvvmLight.Ioc;
using ISynergy.Services;
using System;
using System.Globalization;
using System.Threading;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string currencySymbol = "$";

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out decimal amount))
                {
                    return SimpleIoc.Default.GetInstance<IConverterService>().ConvertDecimalToCurrency(amount);
                }
            }

            NumberFormatInfo info = Thread.CurrentThread.CurrentCulture.NumberFormat;
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NegativeCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string currencySymbol = "$";

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out decimal amount))
                {
                    return SimpleIoc.Default.GetInstance<IConverterService>().ConvertDecimalToCurrency(amount * -1);
                }
            }

            NumberFormatInfo info = Thread.CurrentThread.CurrentCulture.NumberFormat;
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
