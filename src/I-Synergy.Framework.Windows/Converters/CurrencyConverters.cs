using System;
using System.Globalization;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.Windows.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var currencySymbol = "$";

            if (!DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out var amount))
                {
                    return ServiceLocator.Default.GetInstance<IConverterService>().ConvertDecimalToCurrency(amount);
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

            if (!DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out var amount))
                {
                    return ServiceLocator.Default.GetInstance<IConverterService>().ConvertDecimalToCurrency(amount * -1);
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
