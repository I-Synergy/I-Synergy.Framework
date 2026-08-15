using ISynergy.Framework.Mvvm.Events;

namespace ISynergy.Framework.Mvvm.Commands.Base;

/// <summary>
/// Provides a UI-framework-agnostic way for relay commands to report exceptions thrown by their
/// execute delegate to interested listeners (typically the platform-specific <c>Application</c>
/// class), without requiring a DI-resolved service instance at the point where the command is
/// constructed. Commands are usually created directly (e.g. <c>new RelayCommand(...)</c>) rather
/// than through the DI container, so there is no injection point through which a command could
/// obtain an <c>IExceptionHandlerService</c> instance itself.
/// </summary>
/// <remarks>
/// This mirrors the existing pattern already used for asynchronous command failures: a fault in
/// an async command's execution task becomes an unobserved task exception, which every platform
/// head's <c>Application</c> class already subscribes to via
/// <see cref="TaskScheduler.UnobservedTaskException"/> and forwards to
/// <c>IExceptionHandlerService.HandleException</c>. Subscribing to this event and forwarding to
/// the same service gives synchronous <see cref="RelayCommand"/> / <see cref="RelayCommand{T}"/>
/// failures (and pre-await synchronous faults from async commands) the same friendly-dialog
/// behavior instead of crashing the process.
/// </remarks>
public static class CommandExceptionDispatcher
{
    /// <summary>
    /// Occurs when a command's execute delegate throws an exception that could not safely be
    /// allowed to propagate as a synchronous throw.
    /// </summary>
    public static event EventHandler<CommandExceptionEventArgs>? UnhandledCommandException;

    /// <summary>
    /// Raises the <see cref="UnhandledCommandException"/> event.
    /// </summary>
    /// <param name="sender">The command instance that caught the exception.</param>
    /// <param name="exception">The exception thrown by the command's execute delegate.</param>
    internal static void Raise(object? sender, Exception exception) =>
        UnhandledCommandException?.Invoke(sender, new CommandExceptionEventArgs(exception));
}
