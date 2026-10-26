namespace ISynergy.Framework.UI.Enumerations;

/// <summary>
/// Logical edge a blade pane is anchored to. Logical means it follows the
/// active flow direction: <see cref="Start"/> is left in left-to-right
/// cultures and right in right-to-left cultures.
/// </summary>
public enum BladePaneAnchor
{
    /// <summary>Anchor at the logical end edge (the default).</summary>
    Auto = 0,
    /// <summary>Anchor at the logical start edge.</summary>
    Start = 1,
    /// <summary>Anchor at the logical end edge.</summary>
    End = 2
}
