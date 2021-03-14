using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Abstractions
{
	/// <summary>
	/// An Async implementation of ICommand for Task
	/// </summary>
	public interface IAsyncCommand<in TExecute, in TCanExecute> : IAsyncCommand<TExecute>
	{
		/// <summary>
		/// Determines whether the command can execute in its current state
		/// </summary>
		/// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
		bool CanExecute(TCanExecute parameter);
	}

	/// <summary>
	/// An Async implementation of ICommand for Task
	/// </summary>
	public interface IAsyncCommand<in T> : ICommand
	{
		/// <summary>
		/// Executes the Command as a Task
		/// </summary>
		/// <returns>The Task to execute</returns>
		/// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
		Task ExecuteAsync(T parameter);

		/// <summary>
		/// Raises the CanExecuteChanged event.
		/// </summary>
		void RaiseCanExecuteChanged();
	}

	/// <summary>
	/// An Async implementation of ICommand for Task
	/// </summary>
	public interface IAsyncCommand : ICommand
	{
		/// <summary>
		/// Executes the Command as a Task
		/// </summary>
		/// <returns>The Task to execute</returns>
		Task ExecuteAsync();

		/// <summary>
		/// Raises the CanExecuteChanged event.
		/// </summary>
		void RaiseCanExecuteChanged();
	}
}
