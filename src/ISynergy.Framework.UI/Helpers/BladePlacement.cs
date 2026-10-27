using ISynergy.Framework.Globalization.Constants;
using ISynergy.Framework.UI.Enumerations;

namespace ISynergy.Framework.UI.Helpers;

/// <summary>
/// Platform-agnostic helper that decides which physical screen edge a blade pane
/// is anchored to. It resolves the logical <see cref="BladePaneAnchor"/> against the
/// active flow direction so that WPF, MAUI and WinUI hosts share the same contract.
/// </summary>
public static class BladePlacement
{
    /// <summary>
    /// Resolves <see cref="BladePaneAnchor.Auto"/> to <see cref="BladePaneAnchor.End"/>,
    /// leaving an explicit anchor unchanged. This only concerns the logical edge and does
    /// not flip the result for right-to-left.
    /// </summary>
    /// <param name="anchor">The logical anchor to resolve.</param>
    /// <param name="isRightToLeft">Whether the active flow direction is right-to-left.</param>
    /// <returns>The resolved logical anchor.</returns>
    public static BladePaneAnchor Resolve(BladePaneAnchor anchor, bool isRightToLeft) =>
        anchor == BladePaneAnchor.Auto ? BladePaneAnchor.End : anchor;

    /// <summary>
    /// Determines whether the resolved anchor maps to the physical LEFT edge.
    /// </summary>
    /// <param name="anchor">The logical anchor.</param>
    /// <param name="isRightToLeft">Whether the active flow direction is right-to-left.</param>
    /// <returns><c>true</c> when the pane sits on the physical left edge; otherwise, <c>false</c>.</returns>
    public static bool IsStartSide(BladePaneAnchor anchor, bool isRightToLeft)
    {
        var resolved = Resolve(anchor, isRightToLeft);

        // In LTR, the logical Start edge is on the left; in RTL it is mirrored to the right.
        return isRightToLeft
            ? resolved == BladePaneAnchor.End
            : resolved == BladePaneAnchor.Start;
    }

    /// <summary>
    /// Determines whether the resolved anchor maps to the physical LEFT edge for the
    /// given culture. A <c>null</c> or whitespace culture name is treated as left-to-right.
    /// </summary>
    /// <param name="anchor">The logical anchor.</param>
    /// <param name="cultureName">The culture name used to determine flow direction; may be <c>null</c>.</param>
    /// <returns><c>true</c> when the pane sits on the physical left edge; otherwise, <c>false</c>.</returns>
    /// <exception cref="System.Globalization.CultureNotFoundException">
    /// Thrown when <paramref name="cultureName"/> is not a valid culture name.
    /// </exception>
    public static bool IsStartSide(BladePaneAnchor anchor, string? cultureName)
    {
        var isRightToLeft = !string.IsNullOrWhiteSpace(cultureName)
            && GlobalizationConstants.IsRightToLeftLanguage(cultureName!);

        return IsStartSide(anchor, isRightToLeft);
    }

    /// <summary>
    /// Returns the horizontal direction sign for the physical edge the pane sits on:
    /// <c>+1</c> for the physical RIGHT edge and <c>-1</c> for the physical LEFT edge.
    /// Used to steer slide-in animations and shadows, which are never mirrored automatically.
    /// </summary>
    /// <param name="anchor">The logical anchor.</param>
    /// <param name="isRightToLeft">Whether the active flow direction is right-to-left.</param>
    /// <returns><c>+1</c> when the pane sits on the physical right edge; otherwise, <c>-1</c>.</returns>
    public static int HorizontalSign(BladePaneAnchor anchor, bool isRightToLeft) =>
        HorizontalSign(IsStartSide(anchor, isRightToLeft), isRightToLeft);

    /// <summary>
    /// Returns the horizontal direction sign from an already-resolved physical side:
    /// <c>-1</c> for the physical LEFT edge and <c>+1</c> for the physical RIGHT edge.
    /// </summary>
    /// <param name="isStartSide"><c>true</c> when the pane sits on the physical left edge.</param>
    /// <param name="isRightToLeft">Whether the active flow direction is right-to-left.</param>
    /// <returns><c>-1</c> when the pane sits on the physical left edge; otherwise, <c>+1</c>.</returns>
    public static int HorizontalSign(bool isStartSide, bool isRightToLeft) =>
        isStartSide ? -1 : 1;
}
