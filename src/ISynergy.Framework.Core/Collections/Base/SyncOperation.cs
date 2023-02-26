using System.ComponentModel;

namespace ISynergy.Framework.Core.Collections.Base
{
    /// <summary>
    /// <para>Contains helpers for <see cref="SynchronizationContext"/> operations.</para>
    /// <para>Can be use as UI safe operations.</para>
    /// </summary>
    public class SyncOperation
    {
        private Action<(Action callback, object?[] parameters)> _contextPost;
        private Action<(Action callback, object?[] parameters)> _contextSend;

        /// <summary>
        /// Creates new instance of the <c>SyncOperation</c> class.
        /// </summary>
        /// <remarks>
        /// <para>This will used the current thread as base synchronization context for all context operations.</para>
        /// <para>Use <see cref="SetContext()"/> method to change the current synchronization context.</para>
        /// </remarks>
        public SyncOperation()
        {
            var context = AsyncOperationManager.SynchronizationContext;
            _contextPost = action => context.Post(s => action.callback(), null);
            _contextSend = action => context.Send(s => action.callback(), null);
        }

        /// <summary>
        /// Sets the current synchronization context to the current thread.
        /// </summary>
        public void SetContext()
        {
            SetContext(AsyncOperationManager.SynchronizationContext);
        }

        /// <summary>
        /// Sets the current synchronization context to use the custom operations.
        /// </summary>
        /// <param name="contextPost">
        /// Operation implementation for post.
        /// </param>
        /// <param name="contextSend">
        /// Operation implementation for send.
        /// </param>
        public void SetContext(Action<(Action callback, object?[] parameters)> contextPost, Action<(Action callback, object?[] parameters)> contextSend)
        {
            _contextPost = contextPost;
            _contextSend = contextSend;
        }

        /// <summary>
        /// Sets the current synchronization context to the specified <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="context">
        /// The provided <see cref="SynchronizationContext"/> for context operations.
        /// </param>
        public void SetContext(SynchronizationContext context)
        {
            SetContext(
                action => context.Post(s => action.callback(), null),
                action => context.Send(s => action.callback(), null));
        }

        /// <summary>
        /// Sets the current synchronization context to use another <see cref="SyncOperation"/> as base context operation.
        /// </summary>
        /// <param name="syncOperation">
        /// The <see cref="SyncOperation"/> to be use as base context operation.
        /// </param>
        public void SetContext(SyncOperation syncOperation)
        {
            SetContext(
                action => syncOperation._contextPost(action),
                action => syncOperation._contextSend(action));
        }

        /// <summary>
        /// Executes <paramref name="action"/> to the current synchronization context without blocking the executing thread.
        /// </summary>
        /// <param name="action">
        /// The action to be executed at the current synchronization context.
        /// </param>
        /// <param name="parameters">
        /// The parameters to be pass at the current synchronization context.
        /// </param>
        public void ContextPost(Action action, params object?[] parameters)
        {
            _contextPost((action, parameters));
        }

        /// <summary>
        /// Executes <paramref name="action"/> to the current synchronization context and blocking the executing thread until the action ended.
        /// </summary>
        /// <param name="action">
        /// The action to be executed at the current synchronization context.
        /// </param>
        /// <param name="parameters">
        /// The parameters to be pass at the current synchronization context.
        /// </param>
        public void ContextSend(Action action, params object[] parameters)
        {
            _contextSend((action, parameters));
        }

        /// <summary>
        /// Executes <paramref name="action"/> to the current synchronization context asynchronously.
        /// </summary>
        /// <param name="action">
        /// The action to be executed at the current synchronization context.
        /// </param>
        /// <param name="parameters">
        /// The parameters to be pass at the current synchronization context.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents a proxy for the task returned by <paramref name="action"/>.
        /// </returns>
        public Task ContextSendAsync(Action action, params object[] parameters) =>
            Task.Run(() => _contextSend((action, parameters)));

        /// <summary>
        /// Executes <paramref name="func"/> to the current synchronization context asynchronously.
        /// </summary>
        /// <param name="func">
        /// The action to be executed at the current synchronization context.
        /// </param>
        /// <param name="parameters">
        /// The parameters to be pass at the current synchronization context.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents a proxy for the task returned by <paramref name="func"/>.
        /// </returns>
        public Task ContextSendAsync(Func<Task> func, params object[] parameters) =>
            Task.Run(() => _contextSend((async () => await func(), parameters)));
    }
}
