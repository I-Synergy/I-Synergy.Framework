using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using UIKit;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// iOS-specific half of <see cref="ThemeService"/>: the tint colour UIKit applies to controls.
/// </summary>
public partial class ThemeService
{
    /// <inheritdoc />
    partial void ApplyPlatformColors(Application application)
    {
        Logger.LogTrace("Updating iOS-specific colors");

        var accent = Application.AccentColor;

        if (accent is null)
            return;

        UIView.Appearance.TintColor = new UIColor(
            (nfloat)accent.Red,
            (nfloat)accent.Green,
            (nfloat)accent.Blue,
            1.0f);
    }
}
