using Microsoft.Maui.Controls.Shapes;

namespace ISynergy.Framework.UI.Extensions;

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
            return (Geometry)new PathGeometryConverter().ConvertFromInvariantString(path);

        return null;
    }
}
