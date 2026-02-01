using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Windows.UI.Core;

/// <summary>
/// Class DispatcherExtensions.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="action">The action to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static Task RunAndAwaitAsync(this CoreDispatcher dispatcher, Action action) =>
        dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, action);

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="priority">Specifies the priority for event dispatch.</param>
    /// <param name="action">The action to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static async Task RunAndAwaitAsync(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Action action)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        await dispatcher.RunAsync(priority, () =>
        {
            try
            {
                action();
                taskCompletionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        await taskCompletionSource.Task;
    }

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="function">The function to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static Task<T> RunAndAwaitAsync<T>(this CoreDispatcher dispatcher, Func<T> function) =>
        dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, function);

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="priority">Specifies the priority for event dispatch.</param>
    /// <param name="function">The function to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static async Task<T> RunAndAwaitAsync<T>(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Func<T> function)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        await dispatcher.RunAsync(priority, () =>
        {
            try
            {
                var result = function();
                taskCompletionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="asyncAction">The async action to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static Task RunAndAwaitAsync(this CoreDispatcher dispatcher, Func<Task> asyncAction) =>
        dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, asyncAction);

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="priority">Specifies the priority for event dispatch.</param>
    /// <param name="asyncAction">The async action to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static async Task RunAndAwaitAsync(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Func<Task> asyncAction)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        await dispatcher.RunAsync(priority, async () =>
        {
            try
            {
                await asyncAction();
                taskCompletionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        await taskCompletionSource.Task;
    }

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="asyncFunction">The async function to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static Task<T> RunAndAwaitAsync<T>(this CoreDispatcher dispatcher, Func<Task<T>> asyncFunction) =>
        dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, asyncFunction);

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="priority">Specifies the priority for event dispatch.</param>
    /// <param name="asyncFunction">The async function to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static async Task<T> RunAndAwaitAsync<T>(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Func<Task<T>> asyncFunction)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        await dispatcher.RunAsync(priority, async () =>
        {
            try
            {
                var result = await asyncFunction();
                taskCompletionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="asyncAction">The async action to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static Task RunAndAwaitAsync(this CoreDispatcher dispatcher, Func<IAsyncAction> asyncAction) =>
        dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, asyncAction);

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="priority">Specifies the priority for event dispatch.</param>
    /// <param name="asyncAction">The async action to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static async Task RunAndAwaitAsync(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Func<IAsyncAction> asyncAction)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        await dispatcher.RunAsync(priority, async () =>
        {
            try
            {
                await asyncAction();
                taskCompletionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        await taskCompletionSource.Task;
    }

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="asyncFunction">The async function to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static Task<T> RunAndAwaitAsync<T>(this CoreDispatcher dispatcher, Func<IAsyncOperation<T>> asyncFunction) =>
        dispatcher.RunAndAwaitAsync(CoreDispatcherPriority.Normal, asyncFunction);

    /// <summary>
    /// Runs the event dispatcher and awaits for it to complete before returning the results for dispatched events asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="priority">Specifies the priority for event dispatch.</param>
    /// <param name="asyncFunction">The async function to call.</param>
    /// <returns>The <see cref="Task" /> object representing the asynchronous operation.</returns>
    public static async Task<T> RunAndAwaitAsync<T>(this CoreDispatcher dispatcher, CoreDispatcherPriority priority, Func<IAsyncOperation<T>> asyncFunction)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        await dispatcher.RunAsync(priority, async () =>
        {
            try
            {
                var result = await asyncFunction();
                taskCompletionSource.TrySetResult(result);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        });

        return await taskCompletionSource.Task;
    }
}
