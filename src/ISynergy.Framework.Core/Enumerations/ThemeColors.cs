using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Enumerations;

/// <summary>
/// Theme colors.
/// </summary>
[Bindable(BindableSupport.Yes)]
public class ThemeColors
{
    /// <summary>
    /// 42 accent colors arranged as 6 hue families × 7 shades (light → dark).
    /// Families: Red → Orange → Yellow → Green → Blue → Violet.
    /// Shades correspond to Tailwind CSS 300–900 — the mid-range that works as
    /// accent colors (excludes the too-light 50/100/200 and near-black 950).
    /// </summary>
    public static readonly ReadOnlyCollection<string> Colors = new List<string>
    {
        // Red
        "#fca5a5", "#f87171", "#ef4444", "#dc2626", "#b91c1c", "#991b1b", "#7f1d1d",
        // Orange
        "#fdba74", "#fb923c", "#f97316", "#ea580c", "#c2410c", "#9a3412", "#7c2d12",
        // Yellow
        "#fde047", "#facc15", "#eab308", "#ca8a04", "#a16207", "#854d0e", "#713f12",
        // Green
        "#86efac", "#4ade80", "#22c55e", "#16a34a", "#15803d", "#166534", "#14532d",
        // Blue
        "#93c5fd", "#60a5fa", "#3b82f6", "#2563eb", "#1d4ed8", "#1e40af", "#1e3a8a",
        // Violet
        "#c4b5fd", "#a78bfa", "#8b5cf6", "#7c3aed", "#6d28d9", "#5b21b6", "#4c1d95",
    }.AsReadOnly();

    /// <summary>
    /// Gets or sets the Default property value.
    /// </summary>
    public static string Default => "#2563eb";
}
