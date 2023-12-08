using System.Windows.Input;

namespace ISynergy.Framework.UI.Models;

/// <summary>
/// Class NavigationItem.
/// </summary>
public partial class NavigationItem
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
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public ICommand Command { get; private set; }

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
    /// Gets or sets the symbol.
    /// </summary>
    /// <value>The symbol.</value>
    public object Symbol { get; set; }

    /// <summary>
    /// Gets or sets the foreground.
    /// </summary>
    /// <value>The foreground.</value>
    public string Foreground { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationItem"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="symbol">The symbol.</param>
    /// <param name="foreground">The foreground.</param>
    /// <param name="command">The command.</param>
    /// <param name="commandParameter">The command parameter.</param>
    public NavigationItem(string name, object symbol, string foreground, ICommand command, object commandParameter = null)
    {
        Name = name;
        ToolTipMenu = name;
        Symbol = symbol;
        Foreground = foreground;
        Command = command;
        CommandParameter = commandParameter;
    }
}
