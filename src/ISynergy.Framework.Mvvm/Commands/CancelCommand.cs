using ISynergy.Framework.Mvvm.Abstractions.Commands;
using System.ComponentModel;
using System.Windows.Input;

#nullable enable

namespace ISynergy.Framework.Mvvm.Commands
{
    /// <summary>
    /// A <see cref="ICommand"/> implementation wrapping <see cref="IAsyncRelayCommand"/> to support cancellation.
    /// </summary>
    internal sealed class CancelCommand : ICommand
    {
        /// <summary>
        /// The wrapped <see cref="IAsyncRelayCommand"/> instance.
        /// </summary>
        private readonly IAsyncRelayCommand _command;

        /// <summary>
        /// Creates a new <see cref="CancelCommand"/> instance.
        /// </summary>
        /// <param name="command">The <see cref="IAsyncRelayCommand"/> instance to wrap.</param>
        public CancelCommand(IAsyncRelayCommand command)
        {
            _command = command;
            _command.PropertyChanged += OnPropertyChanged;
        }

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => _command.CanBeCanceled;

        /// <inheritdoc/>
        public void Execute(object? parameter) => _command.Cancel();

        /// <inheritdoc cref="PropertyChangedEventHandler"/>
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is null or nameof(IAsyncRelayCommand.CanBeCanceled))
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
