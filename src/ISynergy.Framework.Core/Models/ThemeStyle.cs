using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Enumerations;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Represents a combined application theme and accent color.
/// </summary>
public class ThemeStyle : ObservableClass
{
    /// <summary>
    /// Gets or sets the application theme (Light, Dark, or Default).
    /// </summary>
    public Themes Theme
    {
        get => GetValue<Themes>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the accent color hex string (lowercase, e.g., "#ffb900").
    /// </summary>
    public string Color
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public override string ToString() => $"{Theme}|{Color}";
}
