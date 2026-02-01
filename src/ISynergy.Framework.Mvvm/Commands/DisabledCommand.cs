using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A reusable <see cref="ICommand"/> instance that is always disabled.
/// </summary>
internal sealed class DisabledCommand : ICommand
{
    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    /// <summary>
    /// Gets a shared, reusable <see cref="DisabledCommand"/> instance.
    /// </summary>
    /// <remarks>
    /// This instance can safely be used across multiple objects without having
    /// to worry about this static keeping others alive, as the event uses a
    /// custom accessor that just discards items (as the event is known to never
    /// be raised). As such, this instance will never act as root for other objects.
    /// </remarks>
    public static DisabledCommand Instance { get; } = new();

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => false;

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
    }
}
