namespace ISynergy.Framework.Mvvm.Enumerations;

/// <summary>
/// Specifies which view hosts an opened blade.
/// </summary>
public enum BladeHostTargets
{
    /// <summary>
    /// The shell-level blade host. The pane is rendered by the shell, so it spans the whole window
    /// (including the navigation pane). This is the default.
    /// </summary>
    Shell = 0,

    /// <summary>
    /// The owner passed to <c>OpenBladeAsync</c>. The pane is rendered inside that view, so the view
    /// has to host a blade pane itself.
    /// </summary>
    Owner = 1
}
