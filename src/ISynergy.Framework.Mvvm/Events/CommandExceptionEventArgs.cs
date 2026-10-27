namespace ISynergy.Framework.Mvvm.Events;

/// <summary>
/// Provides data for the <see cref="Commands.Base.CommandExceptionDispatcher.UnhandledCommandException"/>
/// event, raised when a command's execute delegate throws an exception that cannot safely be
/// allowed to propagate as a synchronous throw (e.g. from a UI-bound <c>ICommand.Execute(object)</c>
/// call originating on the main/UI thread).
/// </summary>
/// <seealso cref="EventArgs" />
public sealed class CommandExceptionEventArgs : EventArgs
{
    /// <summary>
    /// Gets the exception thrown by the command's execute delegate.
    /// </summary>
    /// <value>The exception.</value>
    public Exception Exception { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandExceptionEventArgs"/> class.
    /// </summary>
    /// <param name="exception">The exception that was thrown.</param>
    public CommandExceptionEventArgs(Exception exception)
    {
        Exception = exception;
    }
}
