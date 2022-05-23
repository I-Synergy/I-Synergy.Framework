using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace ISynergy.Framework.UI.Navigation
{
    /// <summary>
    /// Class NavigationItem.
    /// Implements the <see cref="NavigationBase" />
    /// </summary>
    /// <seealso cref="NavigationBase" />
    public partial class NavigationItem
    {
        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        /// <value>The symbol.</value>
        public string Symbol { get; set; }
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
}
