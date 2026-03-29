using System.Globalization;
using System.Text;

namespace ISynergy.Framework.Core.Utilities;

/// <summary>
/// Class StringUtility.
/// </summary>
public static class StringUtility
{
    /// <summary>
    /// Converts the string to decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="input">The input.</param>
    /// <param name="seperatoradded">if set to <c>true</c> [seperatoradded].</param>
    /// <returns>System.Decimal.</returns>
    public static decimal ConvertStringToDecimal(decimal value, string input, bool seperatoradded)
    {
        string placeholder;

        if (seperatoradded)
        {
            var culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
            placeholder = value.ToString() + culture.NumberFormat.CurrencyDecimalSeparator + input;
        }
        else
        {
            placeholder = value.ToString() + input;
        }

        if (placeholder.StartsWith("0") && placeholder.Length > 1)
        {
            placeholder = placeholder.Remove(0, 1);
        }

        if (decimal.TryParse(placeholder, out var result))
        {
            return result;
        }

        return 0;
    }

    /// <summary>
    /// Extension where encoded string is converted to a byte array.
    /// </summary>
    /// <typeparam name="TEncoding">The type of the t encoding.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>System.Byte[].</returns>
    public static byte[] GetBytes<TEncoding>(string value) where TEncoding : Encoding, new() =>
        new TEncoding().GetBytes(value);
}
