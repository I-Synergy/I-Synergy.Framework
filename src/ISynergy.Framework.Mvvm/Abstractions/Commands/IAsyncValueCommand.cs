using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Abstractions
{
    /// <summary>
    /// An Async implementation of ICommand for ValueTask
    /// </summary>
    public interface IAsyncValueCommand<in TExecute, in TCanExecute> : IAsyncValueCommand<TExecute>
    {
        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        bool CanExecute(TCanExecute parameter);
    }

    /// <summary>
    /// An Async implementation of ICommand for ValueTask
    /// </summary>
    public interface IAsyncValueCommand<in T> : ICommand
    {
        /// <summary>
        /// Executes the Command as a ValueTask
        /// </summary>
        /// <returns>The ValueTask to execute</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        ValueTask ExecuteAsync(T parameter);

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }

    /// <summary>
    /// An Async implementation of ICommand for ValueTask
    /// </summary>
    public interface IAsyncValueCommand : ICommand
    {
        /// <summary>
        /// Executes the Command as a ValueTask
        /// </summary>
        /// <returns>The ValueTask to execute</returns>
        ValueTask ExecuteAsync();

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
