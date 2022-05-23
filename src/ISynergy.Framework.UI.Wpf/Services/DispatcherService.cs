using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.UI.Extensions;
using ISynergy.Framework.UI.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Dispatcher = System.Windows.Threading.Dispatcher;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Dispatcher service.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherService"/> class.
        /// </summary>
        public DispatcherService(ILogger<DispatcherService> logger)
        {
            // Get current dispatcher to make sure we have one
            var currentDispatcher = DispatcherHelper.CurrentDispatcher;

            if (currentDispatcher != null)
            {
                logger.LogDebug("Successfully Initialized current dispatcher");
            }
            else
            {
                logger.LogWarning("Failed to retrieve the current dispatcher at this point, will try again later");
            }
        }

        /// <summary>
        /// Gets the current dispatcher.
        /// <para />
        /// Internally, this property uses the <see cref="DispatcherHelper"/>, but can be overriden if required.
        /// </summary>
        public virtual object CurrentDispatcher
        {
            get { return DispatcherHelper.CurrentDispatcher; }
        }

        /// <summary>
        /// Invokes action with the dispatcher.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task InvokeAsync(Action action) =>
            ((Dispatcher)CurrentDispatcher).InvokeAsync(action).Task;


        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public Task InvokeAsync(Delegate method, params object[] args)
        {
            return DispatcherExtensions.InvokeAsync((Dispatcher)CurrentDispatcher, method, args);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        public Task<T> InvokeAsync<T>(Func<T> func)
        {
            return DispatcherExtensions.InvokeAsync((Dispatcher)CurrentDispatcher, func);
        }

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public void Invoke(Action action, bool onlyInvokeWhenNoAccess = true)
        {
            Argument.IsNotNull(action);

            DispatcherExtensions.Invoke((Dispatcher)CurrentDispatcher, action, onlyInvokeWhenNoAccess);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true)
        {
            Argument.IsNotNull(action);

            DispatcherExtensions.BeginInvoke((Dispatcher)CurrentDispatcher, action, onlyBeginInvokeWhenNoAccess);
        }
    }
}
