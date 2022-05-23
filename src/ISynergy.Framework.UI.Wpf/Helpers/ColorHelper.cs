using ISynergy.Framework.Core.Extensions;
using System.Globalization;
using System.Windows.Media;

namespace ISynergy.Framework.UI.Helpers
{
    public class ColorHelper
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
    }
}
