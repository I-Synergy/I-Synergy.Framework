using ISynergy.Framework.Mvvm.Abstractions;

namespace ISynergy.Framework.UI.Services;

/// <summary>
/// Host control that renders the page the navigation service navigated to. It is the MAUI
/// equivalent of the content <c>Frame</c> a WinUI or WPF shell exposes, so navigation replaces
/// the shell's content instead of pushing pages onto the window.
/// </summary>
public interface INavigationHost
{
    /// <summary>
    /// Gets the page currently hosted, or <see langword="null"/> when nothing has been shown yet.
    /// </summary>
    IView? CurrentPage { get; }

    /// <summary>
    /// Shows <paramref name="page"/> in this host.
    /// </summary>
    /// <param name="page">The page to show.</param>
    void SetContent(IView page);
}

/// <summary>
/// Registry that lets the navigation service find the <see cref="INavigationHost"/> of the
/// current shell. The host control registers itself while it is attached to the visual tree.
/// </summary>
public interface INavigationHostRegistry
{
    /// <summary>
    /// Gets the host of the current shell, or <see langword="null"/> when no host is attached.
    /// </summary>
    INavigationHost? Current { get; }

    /// <summary>
    /// Registers <paramref name="host"/> as the current navigation host.
    /// </summary>
    /// <param name="host">The host control.</param>
    void Register(INavigationHost host);

    /// <summary>
    /// Removes <paramref name="host"/> when it leaves the visual tree.
    /// </summary>
    /// <param name="host">The host control.</param>
    void Unregister(INavigationHost host);
}
