namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Registry contract that lets a blade service discover whether a blade (edge pane)
/// host control is currently attached to the visual tree, so blades can be rendered
/// as an overlay pane instead of being pushed as a modal page.
/// </summary>
public interface IBladePaneHost
{
    /// <summary>
    /// Gets a value indicating whether at least one blade pane host is currently
    /// attached to the visual tree.
    /// </summary>
    /// <value><see langword="true"/> while a host is attached; otherwise, <see langword="false"/>.</value>
    bool IsHostAttached { get; }

    /// <summary>
    /// Registers a host as attached. Called by the host when it enters the visual tree.
    /// </summary>
    /// <param name="host">The host control instance.</param>
    void Register(object host);

    /// <summary>
    /// Unregisters a previously registered host. Called when the host leaves the visual tree
    /// or is disposed.
    /// </summary>
    /// <param name="host">The host control instance.</param>
    void Unregister(object host);
}
