namespace ISynergy.Framework.UI.Processes;

/// <summary>
/// The ProcessEventArgs are arguments for a console event.
/// </summary>
public class ProcessEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessEventArgs" /> class.
    /// </summary>
    public ProcessEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessEventArgs" /> class.
    /// </summary>
    /// <param name="content">The content.</param>
    public ProcessEventArgs(string content)
    {
        //  Set the content and code.
        Content = content;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessEventArgs" /> class.
    /// </summary>
    /// <param name="code">The code.</param>
    public ProcessEventArgs(int code)
    {
        //  Set the content and code.
        Code = code;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessEventArgs" /> class.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="code">The code.</param>
    public ProcessEventArgs(string content, int code)
    {
        //  Set the content and code.
        Content = content;
        Code = code;
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    /// <value>The content.</value>
    public string? Content { get; }

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    public int? Code { get; }
}
