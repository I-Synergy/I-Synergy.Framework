using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Validation;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Commands
{
    /// <summary>
    /// Class Command.
    /// Implements the <see cref="ICommand" />
    /// </summary>
    /// <seealso cref="ICommand" />
    public class Command : ICommand
    {
        /// <summary>
        /// The execute
        /// </summary>
        private readonly WeakAction _execute;

        /// <summary>
        /// The can execute
        /// </summary>
        private readonly WeakFunc<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public Command(Action execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        /// <exception cref="ArgumentNullException">execute</exception>
        public Command(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
        {
            Argument.IsNotNull(execute);

            _execute = new WeakAction(execute, keepTargetAlive);

            if (canExecute is not null)
            {
                _canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
            }
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute is null
                || (_canExecute.IsStatic || _canExecute.IsAlive)
                    && _canExecute.Execute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter)
                && _execute is not null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }
}