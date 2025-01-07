using System.Globalization;

namespace ISynergy.Framework.UI.Extensions;
public static class DecimalExtensions
{
    /// <summary>
    /// Converts the decimal to currency.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.String.</returns>
    public static string ToCurrency(this decimal value) =>
        string.Format(CultureInfo.CurrentCulture.NumberFormat, "{0:C}", value);
}
