using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Abstractions.Commands;

/// <summary>
/// An interface expanding <see cref="ICommand"/> with the ability to raise
/// the <see cref="ICommand.CanExecuteChanged"/> event externally.
/// </summary>
public interface IRelayCommand : ICommand
{
    /// <summary>
    /// Notifies that the <see cref="ICommand.CanExecute"/> property has changed.
    /// </summary>
    void NotifyCanExecuteChanged();
}
