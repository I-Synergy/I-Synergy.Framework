using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ISynergy.Framework.Windows.Navigation
{
    /// <summary>
    /// Class NavigationBase.
    /// </summary>
    public abstract class NavigationBase { }

    /// <summary>
    /// Class NavigationItem.
    /// Implements the <see cref="NavigationBase" />
    /// </summary>
    /// <seealso cref="NavigationBase" />
    public class NavigationItem : NavigationBase
    {
        /// <summary>
        /// Gets or sets the selected visibility.
        /// </summary>
        /// <value>The selected visibility.</value>
        public Visibility SelectedVisibility { get; set; }
        /// <summary>
        /// Gets or sets the selected foreground.
        /// </summary>
        /// <value>The selected foreground.</value>
        public SolidColorBrush SelectedForeground { get; set; }
        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>The foreground.</value>
        public SolidColorBrush Foreground { get; set; }
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

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="foreground">The foreground.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        public NavigationItem(string name, string symbol, SolidColorBrush foreground, ICommand command, object commandParameter = null)
        {
            Name = name;
            ToolTipMenu = name;
            Symbol = symbol;
            Foreground = foreground;
            Command = command;
            CommandParameter = commandParameter;
        }
    }

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
