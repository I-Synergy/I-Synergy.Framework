using System;
using Windows.UI;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Class ColorExtensions.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts the color of the integer2.
        /// </summary>
        /// <param name="_self">The color.</param>
        /// <returns>Color.</returns>
        public static Color ConvertInteger2Color(this int _self)
        {
            var bytes = BitConverter.GetBytes(_self);
            return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        /// <summary>
        /// Converts the color2 integer.
        /// </summary>
        /// <param name="_self">The color.</param>
        /// <returns>System.Int32.</returns>
        public static int ConvertColor2Integer(this Color _self)
        {
            var bytes = new byte[] { 255, _self.R, _self.G, _self.B };

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
