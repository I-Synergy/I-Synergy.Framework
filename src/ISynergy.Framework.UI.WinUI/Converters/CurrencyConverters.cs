using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.UI.Xaml.Data;
using System.Globalization;
using Windows.ApplicationModel;

namespace ISynergy.Framework.UI.Converters;

/// <summary>
/// Class CurrencyConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class CurrencyConverter : IValueConverter
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
        var currencySymbol = "$";

        if (!DesignMode.DesignModeEnabled)
        {
            if (decimal.TryParse(value.ToString(), out var amount))
            {
                return ServiceLocator.Default.GetService<IConverterService>().ConvertDecimalToCurrency(amount);
            }
        }

        var info = CultureInfo.CurrentCulture.NumberFormat;
        info.CurrencySymbol = $"{currencySymbol} ";
        info.CurrencyNegativePattern = 1;

        return string.Format(info, "{0:C2}", 0);
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
/// Class NegativeCurrencyConverter.
/// Implements the <see cref="IValueConverter" />
/// </summary>
/// <seealso cref="IValueConverter" />
public class NegativeCurrencyConverter : IValueConverter
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
        var currencySymbol = "$";

        if (!DesignMode.DesignModeEnabled)
        {
            if (decimal.TryParse(value.ToString(), out var amount))
            {
                return ServiceLocator.Default.GetService<IConverterService>().ConvertDecimalToCurrency(amount * -1);
            }
        }

        var info = CultureInfo.CurrentCulture.NumberFormat;
        info.CurrencySymbol = $"{currencySymbol} ";
        info.CurrencyNegativePattern = 1;

        return string.Format(info, "{0:C2}", 0);
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
