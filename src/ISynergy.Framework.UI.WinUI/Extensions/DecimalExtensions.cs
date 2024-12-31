using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;
public static class DecimalExtensions
{
    /// <summary>
    /// Converts the decimal to currency.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string ToCurrency(this decimal value)
    {
        var scopedContextService = ServiceLocator.Default.GetService<IScopedContextService>();

        if (scopedContextService.GetService<IContext>() is IContext context)
        {
            var currencySymbol = context.CurrencySymbol ?? "$";
            var info = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            info.CurrencySymbol = $"{currencySymbol} ";
            info.CurrencyNegativePattern = 1;

            return string.Format(info, "{0:C2}", value).Replace("  ", " ");
        }

        var fallback = CultureInfo.CurrentCulture.NumberFormat;
        fallback.CurrencySymbol = "$ ";
        fallback.CurrencyNegativePattern = 1;

        return string.Format(fallback, "{0:C2}", 0);
    }
}
