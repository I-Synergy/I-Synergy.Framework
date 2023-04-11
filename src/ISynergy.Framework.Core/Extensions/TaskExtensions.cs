using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Core.Extensions
{
    /// <summary>
    /// Class TaskExtensions.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Gets an awaitable object that skips end validation.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> to get the awaitable for.</param>
        /// <returns>A <see cref="TaskAwaitableWithoutEndValidation"/> object wrapping <paramref name="task"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TaskAwaitableWithoutEndValidation GetAwaitableWithoutEndValidation(this Task task) =>
            new(task);

        /// <summary>
        /// A custom _task awaitable object that skips end validation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly struct TaskAwaitableWithoutEndValidation
        {
            /// <summary>
            /// The wrapped <see cref="Task"/> instance to create an awaiter for.
            /// </summary>
            private readonly Task _task;

            /// <summary>
            /// Creates a new <see cref="TaskAwaitableWithoutEndValidation"/> instance with the specified parameters.
            /// </summary>
            /// <param name="task">The wrapped <see cref="Task"/> instance to create an awaiter for.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TaskAwaitableWithoutEndValidation(Task task)
            {
                _task = task;
            }

            /// <summary>
            /// Gets an <see cref="Awaiter"/> instance for the current underlying _task.
            /// </summary>
            /// <returns>An <see cref="Awaiter"/> instance for the current underlying _task.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter GetAwaiter() => new(_task);

            /// <summary>
            /// An awaiter object for <see cref="TaskAwaitableWithoutEndValidation"/>.
            /// </summary>
            public readonly struct Awaiter : ICriticalNotifyCompletion
            {
                /// <summary>
                /// The underlying <see cref="TaskAwaiter"/> instance.
                /// </summary>
                private readonly TaskAwaiter _taskAwaiter;

                /// <summary>
                /// Creates a new <see cref="Awaiter"/> instance with the specified parameters.
                /// </summary>
                /// <param name="task">The wrapped <see cref="Task"/> instance to create an awaiter for.</param>
                public Awaiter(Task task)
                {
                    _taskAwaiter = task.GetAwaiter();
                }

                /// <summary>
                /// Gets whether the operation has completed or not.
                /// </summary>
                /// <remarks>This property is intended for compiler user rather than use directly in code.</remarks>
                public bool IsCompleted
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => _taskAwaiter.IsCompleted;
                }

                /// <summary>
                /// Ends the await operation.
                /// </summary>
                /// <remarks>This method is intended for compiler user rather than use directly in code.</remarks>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void GetResult()
                {
                }

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnCompleted(Action continuation) =>
                    _taskAwaiter.OnCompleted(continuation);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void UnsafeOnCompleted(Action continuation) =>
                    _taskAwaiter.UnsafeOnCompleted(continuation);
            }
        }
    }
}
