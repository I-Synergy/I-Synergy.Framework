namespace ISynergy.Framework.UI.Navigation
{
    /// <summary>
    /// Interface for navigational item.
    /// </summary>
    public interface INavigationItem { }

    /// <summary>
    /// Class NavigationBase.
    /// </summary>
    public abstract class NavigationBase : INavigationItem { }

    /// <summary>
    /// Class Separator.
    /// Implements the <see cref="NavigationBase" />
    /// </summary>
    /// <seealso cref="NavigationBase" />
    public class Separator : NavigationBase { }

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
        public string Name { get; set; }
    }
}
