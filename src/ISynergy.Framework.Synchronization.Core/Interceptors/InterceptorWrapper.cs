using ISynergy.Framework.Synchronization.Core.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Interceptors
{
    /// <summary>
    /// Encapsulate 1 func to intercept one event
    /// </summary>
    public class InterceptorWrapper<T> : ISyncInterceptor<T> where T : class
    {
        private Func<T, Task> wrapperAsync;
        internal static Func<T, Task> Empty = new Func<T, Task>(t => Task.CompletedTask);


        /// <summary>
        /// Create a new empty interceptor
        /// </summary>
        public InterceptorWrapper() => wrapperAsync = Empty;

        /// <summary>
        /// Gets a boolean indicating if the interceptor is not used by user (ie : is Empty)
        /// </summary>
        public bool IsEmpty => wrapperAsync == Empty;

        /// <summary>
        /// Set a Func{T, Task} as interceptor
        /// </summary>
        public void Set(Func<T, Task> run) => wrapperAsync = run is not null ? run : Empty;

        /// <summary>
        /// Set an Action{T} as interceptor
        /// </summary>
        [DebuggerStepThrough]
        public void Set(Action<T> run)
        {
            wrapperAsync = run is not null ? (t =>
            {
                run(t);
                return Task.CompletedTask;
            }) : Empty;

        }

        /// <summary>
        /// Run the Action or Func as the Interceptor
        /// </summary>
        [DebuggerStepThrough]
        public async Task RunAsync(T args, CancellationToken cancellationToken)
        {
            if (wrapperAsync is not null)
                await wrapperAsync(args);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanup) => wrapperAsync = null;
    }
}

