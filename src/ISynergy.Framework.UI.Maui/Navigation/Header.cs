using ISynergy.Framework.UI.Navigation.Base;

namespace ISynergy.Framework.UI.Navigation;

/// <summary>
/// Class Header.
/// Implements the <see cref="NavigationBase" />
/// </summary>
/// <seealso cref="NavigationBase" />
public class Header : NavigationBase
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public required string Name { get; set; }
}
