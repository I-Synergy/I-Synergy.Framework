namespace ISynergy.Framework.CQRS.Commands;

/// <summary>
/// Command interface with result
/// </summary>
/// <typeparam name="TResult">
/// The type of the result produced when the command is handled. The type parameter is a marker:
/// dispatchers resolve the handler from the closed command type, so no member has to use it.
/// </typeparam>
#pragma warning disable S2326 // marker type parameter resolved through the closed generic type
public interface ICommand<TResult> : ICommand
{
}
#pragma warning restore S2326
