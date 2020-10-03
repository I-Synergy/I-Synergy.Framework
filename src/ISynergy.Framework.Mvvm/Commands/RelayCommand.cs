using System;
using System.Windows.Input;
using ISynergy.Framework.Core.Events;

namespace ISynergy.Framework.Mvvm.Commands
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'.  This class does not allow you to accept command parameters in the
    /// Execute and CanExecute callback methods.
    /// </summary>
    public class RelayCommand : ICommand
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
        /// Initializes a new instance of the RelayCommand class that
        /// can always execute.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure.</param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public RelayCommand(Action execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closures.</param>
        /// <exception cref="ArgumentNullException">execute</exception>
        public RelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute is null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = new WeakAction(execute, keepTargetAlive);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
            }
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        /// <returns></returns>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute is null
                || (_canExecute.IsStatic || _canExecute.IsAlive)
                    && _canExecute.Execute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }
}
