using System;
using System.Globalization;
using ISynergy.Framework.Core.Extensions;
using Windows.UI;
using Windows.UI.Xaml.Media;

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
    public static Windows.UI.Color HexStringToColor(string hexColor)
    {
        var hc = hexColor.ExtractHexDigits();

        if (hc.Length != 6)
        {
            // you can choose whether to throw an exception
            //throw new ArgumentException("hexColor is not exactly 6 digits.");
            return Colors.Transparent;
        }

        var r = hc.Substring(0, 2);
        var g = hc.Substring(2, 2);
        var b = hc.Substring(4, 2);

        var color = Colors.Transparent;

        try
        {
            var ri
               = byte.Parse(r, NumberStyles.HexNumber);
            var gi
               = byte.Parse(g, NumberStyles.HexNumber);
            var bi
               = byte.Parse(b, NumberStyles.HexNumber);

            color = Windows.UI.Color.FromArgb(255, ri, gi, bi);
        }
        catch
        {
            // you can choose whether to throw an exception
            //throw new ArgumentException("Conversion failed.");
            return Colors.Transparent;
        }
        return color;
    }

    /// <summary>
    /// Gets the solid color brush from hexadecimal string.
    /// </summary>
    /// <param name="hex">The hexadecimal.</param>
    /// <returns>SolidColorBrush.</returns>
    /// <exception cref="InvalidCastException">string is not an hex color code.</exception>
    public static SolidColorBrush GetSolidColorBrushFromHexString(string hex)
    {
        hex = hex.Replace("#", string.Empty);

        if (hex.Length == 6)
        {
            var a = byte.Parse("1");
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }
        else if (hex.Length == 8)
        {
            var a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }
        else
        {
            throw new InvalidCastException("string is not an hex color code.");
        }
    }

    /// <summary>
    /// Gets the color from hexadecimal string.
    /// </summary>
    /// <param name="hex">The hexadecimal.</param>
    /// <returns>Color.</returns>
    /// <exception cref="InvalidCastException">string is not an hex color code.</exception>
    public static Windows.UI.Color GetColorFromHexString(string hex)
    {
        hex = hex.Replace("#", string.Empty);

        if (hex.Length == 6)
        {
            var a = byte.Parse("1");
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }
        else if (hex.Length == 8)
        {
            var a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }
        else
        {
            throw new InvalidCastException("string is not an hex color code.");
        }
    }
}
