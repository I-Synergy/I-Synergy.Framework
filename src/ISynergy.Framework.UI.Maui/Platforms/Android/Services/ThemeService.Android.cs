using ISynergy.Framework.UI.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Android-specific half of <see cref="ThemeService"/>: the platform colour resources the native
/// theme reads.
/// </summary>
public partial class ThemeService
{
    /// <inheritdoc />
    partial void ApplyPlatformColors(Application application)
    {
        Logger.LogTrace("Updating Android-specific colors");

        var accent = Application.AccentColor;

        if (accent is null)
            return;

        UpdateOrAddResource(application, "colorPrimary", accent);
        UpdateOrAddResource(application, "colorAccent", accent);
        UpdateOrAddResource(application, "colorPrimaryDark", accent.AddLuminosity(-0.25f));
    }
}
