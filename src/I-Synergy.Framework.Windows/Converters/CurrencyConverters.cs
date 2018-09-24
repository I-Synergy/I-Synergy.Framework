using ISynergy.Services;
using System;
using System.Globalization;
using System.Threading;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace ISynergy.Converters
{
    public class CurrencyConverter : MarkupExtension, IValueConverter
    {
        public IConverterService ConverterService { get; }

        public CurrencyConverter(IConverterService converterService)
        {
            ConverterService = converterService;
        }

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string currencySymbol = "$";

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out decimal amount))
                {
                    return ConverterService.ConvertDecimalToCurrency(amount);
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

    public class NegativeCurrencyConverter : MarkupExtension, IValueConverter
    {
        public IConverterService ConverterService { get; }

        public NegativeCurrencyConverter(IConverterService converterService)
        {
            ConverterService = converterService;
        }

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string currencySymbol = "$";

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (decimal.TryParse(value.ToString(), out decimal amount))
                {
                    return ConverterService.ConvertDecimalToCurrency(amount * -1);
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
