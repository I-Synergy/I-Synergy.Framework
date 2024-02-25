using ISynergy.Framework.Core.Enumerations;
using System.Drawing;

namespace ISynergy.Framework.UI.Extensions;

public static class ThemeColorsExtensions
{
    public static string ToHtmlColor(this ThemeColors self)
    {
        var color = Color.FromArgb((int)self);
        return ColorTranslator.ToHtml(color);
    }
}
