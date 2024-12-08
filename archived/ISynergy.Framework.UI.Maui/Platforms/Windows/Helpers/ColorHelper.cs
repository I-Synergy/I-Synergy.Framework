using ISynergy.Framework.Core.Extensions;
using System.Globalization;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace ISynergy.Framework.UI.Helpers;

/// <summary>
/// Class ColorConverters.
/// </summary>
public static class ColorHelper
{
    /// <summary>
    /// Convert a hex string to a .NET Color object.
    /// </summary>
    /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
    /// <returns>Color.</returns>
    public static Color HexStringToColor(string hexColor)
    {
        var hc = hexColor.ExtractHexDigits();

        if (hc.Length != 6)
        {
            // you can choose whether to throw an exception
            return Colors.Transparent;
        }

        var r = hc.Substring(0, 2);
        var g = hc.Substring(2, 2);
        var b = hc.Substring(4, 2);

        try
        {
            var ri
               = byte.Parse(r, NumberStyles.HexNumber);
            var gi
               = byte.Parse(g, NumberStyles.HexNumber);
            var bi
               = byte.Parse(b, NumberStyles.HexNumber);

            return Color.FromArgb(255, ri, gi, bi);
        }
        catch
        {
            // you can choose whether to throw an exception
            return Colors.Transparent;
        }
    }

    /// <summary>
    /// Gets the solid color brush from hexadecimal string.
    /// </summary>
    /// <param name="hex">The hexadecimal.</param>
    /// <returns>SolidColorBrush.</returns>
    /// <exception cref="InvalidCastException">string is not an hex color code.</exception>
    public static SolidColorBrush GetSolidColorBrushFromHexString(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return new SolidColorBrush(Colors.Transparent);

        hex = hex.Replace("#", string.Empty);

        if (hex.Length == 6)
        {
            var a = byte.Parse("1");
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        if (hex.Length == 8)
        {
            var a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
        throw new InvalidCastException("string is not an hex color code.");
    }

    /// <summary>
    /// Gets the color from hexadecimal string.
    /// </summary>
    /// <param name="hex">The hexadecimal.</param>
    /// <returns>Color.</returns>
    /// <exception cref="InvalidCastException">string is not an hex color code.</exception>
    public static Color GetColorFromHexString(string hex)
    {
        hex = hex.Replace("#", string.Empty);

        if (hex.Length == 6)
        {
            var a = byte.Parse("1");
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        if (hex.Length == 8)
        {
            var a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }
        throw new InvalidCastException("string is not an hex color code.");
    }
}
