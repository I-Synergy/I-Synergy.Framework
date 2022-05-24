using ISynergy.Framework.Core.Validation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ISynergy.Framework.UI.Extensions
{
    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            var tcs = new TaskCompletionSource<bool>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    method.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), null);

            dispatcherOperation.Completed += (sender, e) => SetResult(tcs, true);
            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            return tcs.Task;
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        public static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            var result = default(T);

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    result = func();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), null);

            dispatcherOperation.Completed += (sender, e) => SetResult(tcs, result);
            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            return tcs.Task;
        }

        private static bool SetResult<T>(TaskCompletionSource<T> tcs, T result)
        {
            if (tcs.Task.IsCanceled)
                return false;

            if (!tcs.TrySetResult(result))
            {
                Debug.WriteLine("Failed to set the task result to '{0}', task was already completed. Current status is '{1}'", result, tcs.Task.Status);
                return false;
            }

            return true;
        }

        private static bool SetCanceled<T>(TaskCompletionSource<T> tcs)
        {
            if (tcs.Task.IsCanceled)
                return true;

            if (!tcs.TrySetCanceled())
            {
                Debug.WriteLine("Failed to set the task as canceled, task was already completed or canceled before");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action)
        {
            return BeginInvoke(dispatcher, action, false);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull(method);
            return BeginInvoke(dispatcher, () => method.DynamicInvoke(args), false);
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static DispatcherOperation BeginInvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            return BeginInvoke(dispatcher, action, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static DispatcherOperation BeginInvokeIfRequired(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull(method);

            return BeginInvoke(dispatcher, () => method.DynamicInvoke(args), true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull(action);

            if (dispatcher != null)
            {
                if (!onlyBeginInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
                    return dispatcher.BeginInvoke(action, null);
                }
            }

            action.Invoke();
            return null;
        }

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Action action)
        {
            Argument.IsNotNull(action);

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Executes the specified delegate with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull(method);

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(method, args);
            }
            else
            {
                method.DynamicInvoke(args);
            }
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            Invoke(dispatcher, action, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull(method);

            Invoke(dispatcher, () => method.DynamicInvoke(args), true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public static void Invoke(this Dispatcher dispatcher, Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull(action);

            if (dispatcher != null)
            {
                if (!onlyBeginInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(action, null);
                    return;
                }
            }

            action.Invoke();
        }

        /// <summary>
        /// Gets the managed thread identifier for the specified dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>The managed thread id.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcher" /> is <c>null</c>.</exception>
        public static int GetThreadId(this Dispatcher dispatcher)
        {
            Argument.IsNotNull(dispatcher);

            return dispatcher.Thread.ManagedThreadId;
        }
    }
}
