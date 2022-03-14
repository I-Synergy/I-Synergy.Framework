using System.Windows.Input;

namespace ISynergy.Framework.UI.Navigation
{
    /// <summary>
    /// Class NavigationItem.
    /// Implements the <see cref="NavigationBase" />
    /// </summary>
    /// <seealso cref="NavigationBase" />
    public partial class NavigationItem : NavigationBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        /// <value>The symbol.</value>
        public string Symbol { get; set; }
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; set; }
        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter { get; set; }
        /// <summary>
        /// Gets or sets the tool tip menu.
        /// </summary>
        /// <value>The tool tip menu.</value>
        public string ToolTipMenu { get; set; }
    }
}
