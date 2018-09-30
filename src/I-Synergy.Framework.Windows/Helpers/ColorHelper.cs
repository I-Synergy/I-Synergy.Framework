using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ISynergy.Helpers
{
    public static class ColorConverters
    {
        /// <summary>
        /// Convert a hex string to a .NET Color object.
        /// </summary>
        /// <param name="hexColor">a hex string: "FFFFFF", "#000000"</param>
        public static Color HexStringToColor(string hexColor)
        {
            string hc = hexColor.ExtractHexDigits();

            if (hc.Length != 6)
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return Colors.Transparent;
            }

            string r = hc.Substring(0, 2);
            string g = hc.Substring(2, 2);
            string b = hc.Substring(4, 2);

            Color color = Colors.Transparent;

            try
            {
                byte ri
                   = byte.Parse(r, System.Globalization.NumberStyles.HexNumber);
                byte gi
                   = byte.Parse(g, System.Globalization.NumberStyles.HexNumber);
                byte bi
                   = byte.Parse(b, System.Globalization.NumberStyles.HexNumber);

                color = Color.FromArgb(255, ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return Colors.Transparent;
            }
            return color;
        }

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
