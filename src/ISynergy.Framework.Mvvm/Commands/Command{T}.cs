using ISynergy.Framework.Core.Events;
using ISynergy.Framework.Core.Validation;
using System;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Commands
{
    /// <summary>
    /// Class RelayCommand.
    /// Implements the <see cref="ICommand" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ICommand" />
    public class Command<T> : ICommand
    {
        /// <summary>
        /// The execute
        /// </summary>
        private readonly WeakAction<T> _execute;

        /// <summary>
        /// The can execute
        /// </summary>
        private readonly WeakFunc<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        public Command(Action<T> execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{T}"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <param name="keepTargetAlive">if set to <c>true</c> [keep target alive].</param>
        /// <exception cref="ArgumentNullException">execute</exception>
        public Command(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false)
        {
            Argument.IsNotNull(nameof(execute), execute);

            _execute = new WeakAction<T>(execute, keepTargetAlive);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T, bool>(canExecute, keepTargetAlive);
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
            if (_canExecute is null)
            {
                return true;
            }

            if (_canExecute.IsStatic || _canExecute.IsAlive)
            {
                if (parameter is null
                    && typeof(T).IsValueType)
                {
                    return _canExecute.Execute(default);
                }

                if (parameter is null || parameter is T)
                {
                    return _canExecute.Execute((T)parameter);
                }
            }

            return false;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public virtual void Execute(object parameter)
        {
            var val = parameter;

            if (parameter != null
                && parameter.GetType() != typeof(T))
            {
                if (parameter is IConvertible)
                {
                    val = Convert.ChangeType(parameter, typeof(T), null);
                }
            }

            if (CanExecute(val)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                if (val is null)
                {
                    if (typeof(T).IsValueType)
                    {
                        _execute.Execute(default);
                    }
                    else
                    {
                        _execute.Execute((T)val);
                    }
                }
                else
                {
                    _execute.Execute((T)val);
                }
            }
        }
    }
}
