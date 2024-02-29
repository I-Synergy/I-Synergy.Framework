namespace ISynergy.Framework.Core.Enumerations;

/// <summary>
/// Theme colors.
/// </summary>
public class ThemeColors
{
    private readonly List<string> _colors;

    /// <summary>
    /// Get list of all available colors.
    /// </summary>
    public List<string> Colors { get => _colors; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ThemeColors()
    {
        _colors =
        [
            "#ffb900",
            "#ff8c00",
            "#f7630c",
            "#ca5010",
            "#da3b01",
            "#ef6950",
            "#d13438",
            "#ff4343",
            "#e74856",
            "#e81123",
            "#ea005e",
            "#c30052",
            "#e3008c",
            "#bf0077",
            "#c239b3",
            "#9a0089",
            "#0078d7",
            "#0063b1",
            "#8e8cd8",
            "#6b69d6",
            "#8764b8",
            "#744da9",
            "#b146c2",
            "#881798",
            "#0099bc",
            "#2d7d9a",
            "#00b7c3",
            "#038387",
            "#00b294",
            "#018574",
            "#00cc6a",
            "#10893e",
            "#7a7574",
            "#5d5a58",
            "#68768a",
            "#515c6b",
            "#567c73",
            "#486860",
            "#498205",
            "#107c10",
            "#767676",
            "#4c4a48",
            "#69797e",
            "#4a5459",
            "#647c64",
            "#525e54",
            "#847545",
            "#7e735f"
        ];
    }

    /// <summary>
    /// Gets or sets the Default property value.
    /// </summary>
    public static string Default
    {
        get => "#0078d7";
    }
}