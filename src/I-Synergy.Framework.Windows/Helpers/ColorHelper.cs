using System;
using System.Globalization;
using Windows.UI.Xaml.Media;

namespace ISynergy.Helpers
{
    public static class ColorConverters
    {
        public static SolidColorBrush GetSolidColorBrushFromHexString(string hex)
        {
            hex = hex.Replace("#", string.Empty);

            if(hex.Length == 6)
            {
                byte a = byte.Parse("1");
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

                return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
            else if(hex.Length == 8)
            {
                byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

                return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
            else
            {
                throw new InvalidCastException("string is not an hex color code.");
            }
        }

        public static Windows.UI.Color GetColorFromHexString(string hex)
        {
            hex = hex.Replace("#", string.Empty);

            if (hex.Length == 6)
            {
                byte a = byte.Parse("1");
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

                return Windows.UI.Color.FromArgb(a, r, g, b);
            }
            else if (hex.Length == 8)
            {
                byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

                return Windows.UI.Color.FromArgb(a, r, g, b);
            }
            else
            {
                throw new InvalidCastException("string is not an hex color code.");
            }
        }
    }
}
