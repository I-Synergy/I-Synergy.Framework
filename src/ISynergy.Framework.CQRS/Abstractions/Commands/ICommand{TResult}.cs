namespace ISynergy.Framework.CQRS.Commands;

/// <summary>
/// Command interface with result
/// </summary>
public interface ICommand<TResult> : ICommand { }