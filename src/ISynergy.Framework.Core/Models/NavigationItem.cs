using System.Windows.Input;

namespace ISynergy.Framework.Core.Models;

/// <summary>
/// Class NavigationItem.
/// </summary>
public partial class NavigationItem : IDisposable
{
    private bool _disposed = false;

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
    public ICommand? Command { get; private set; }

    /// <summary>
    /// Gets or sets the command parameter.
    /// </summary>
    /// <value>The command parameter.</value>
    public object? CommandParameter { get; set; }

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
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    public string Color { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationItem"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="symbol">The symbol.</param>
    /// <param name="color">The color.</param>
    /// <param name="command">The command.</param>
    /// <param name="commandParameter">The command parameter.</param>
    public NavigationItem(string name, object symbol, string color, ICommand? command, object? commandParameter = null)
    {
        Name = name;
        ToolTipMenu = name;
        Symbol = symbol;
        Color = color;
        Command = command;
        CommandParameter = commandParameter;
    }

    #region IDisposable & IAsyncDisposable
    // Dispose() calls Dispose(true)
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // The bulk of the clean-up code is implemented in Dispose(bool)
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // free managed resources
            Command = null;
        }

        // free native resources if there are any.
        _disposed = true;
    }
    #endregion
}
