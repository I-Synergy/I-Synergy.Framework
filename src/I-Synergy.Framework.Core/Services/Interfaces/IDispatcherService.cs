using System;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public partial interface IDispatcherService
    {
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        Task InvokeAsync(Action action);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        Task InvokeAsync(Delegate method, params object[] args);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        Task<T> InvokeAsync<T>(Func<T> func);

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void Invoke(Action action, bool onlyInvokeWhenNoAccess = true);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true);

        object CurrentDispatcher { get; }
    }
}