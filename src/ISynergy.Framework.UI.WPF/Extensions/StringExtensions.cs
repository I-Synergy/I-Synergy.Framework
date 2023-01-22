using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ISynergy.Framework.UI.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the Geometry object.
        /// </summary>
        /// <param name="path">Path Data string</param>
        /// <returns>System.String.</returns>
        public static Geometry ToGeometry(this string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                return Geometry.Parse(path);

            return null;
        }

        /// <summary>
        /// Gets the Geometry object.
        /// </summary>
        /// <param name="path">Path Data string</param>
        /// <returns>System.String.</returns>
        public static Path ToPath(this string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                return new Path
                {
                    Data = path.ToGeometry(),
                    Fill = new SolidColorBrush(Colors.Black),
                    Height = 16,
                    Width = 16,
                    Stretch = Stretch.Fill,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

            return null;
        }
    }
}
