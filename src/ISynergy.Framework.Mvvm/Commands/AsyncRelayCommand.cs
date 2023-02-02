using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Enumerations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mvvm.Commands
{
    /// <summary>
    /// A _command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
    /// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the _execute
    /// action, and providing an <see cref="ExecutionTask"/> property that notifies changes when
    /// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
    /// </summary>
    public sealed class AsyncRelayCommand : IAsyncRelayCommand, ICancellationAwareCommand
    {
        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="ExecutionTask"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs ExecutionTaskChangedEventArgs = new(nameof(ExecutionTask));

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="CanBeCanceled"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new(nameof(CanBeCanceled));

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsCancellationRequested"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new(nameof(IsCancellationRequested));

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsRunning"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new(nameof(IsRunning));

        /// <summary>
        /// The <see cref="Func{TResult}"/> to invoke when <see cref="Execute"/> is used.
        /// </summary>
        private readonly Func<Task>? _execute;

        /// <summary>
        /// Execution task.
        /// </summary>
        private Task? _executionTask;

        /// <summary>
        /// The cancelable <see cref="Func{T,TResult}"/> to invoke when <see cref="Execute"/> is used.
        /// </summary>
        /// <remarks>Only one between this and <see cref="_execute"/> is not <see langword="null"/>.</remarks>
        private readonly Func<CancellationToken, Task>? _cancelableExecute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute"/> is used.
        /// </summary>
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// The _options being set for the current _command.
        /// </summary>
        private readonly AsyncRelayCommandOptions _options;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> instance to use to cancel <see cref="_cancelableExecute"/>.
        /// </summary>
        /// <remarks>This is only used when <see cref="_cancelableExecute"/> is not <see langword="null"/>.</remarks>
        private CancellationTokenSource? _cancellationTokenSource;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<Task> execute)
        {
            Argument.IsNotNull(execute);

            _execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="options">The _options to use to configure the async _command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<Task> execute, AsyncRelayCommandOptions options)
            : this(execute)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> is <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute)
        {
            Argument.IsNotNull(cancelableExecute);

            _cancelableExecute = cancelableExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <param name="options">The _options to use to configure the async _command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> is <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
            : this(cancelableExecute)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
            : this(execute)
        {
            Argument.IsNotNull(canExecute);

            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <param name="options">The _options to use to configure the async _command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="execute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, AsyncRelayCommandOptions options)
            : this(execute, canExecute)
        {
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
            : this (cancelableExecute)
        {
            Argument.IsNotNull(canExecute);

            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <param name="options">The _options to use to configure the async _command.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="cancelableExecute"/> or <paramref name="canExecute"/> are <see langword="null"/>.</exception>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, AsyncRelayCommandOptions options)
            : this (cancelableExecute, canExecute)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public Task? ExecutionTask
        {
            get => _executionTask;
            private set
            {
                if (ReferenceEquals(_executionTask, value))
                    return;

                _executionTask = value;

                PropertyChanged?.Invoke(this, ExecutionTaskChangedEventArgs);
                PropertyChanged?.Invoke(this, IsRunningChangedEventArgs);

                bool isAlreadyCompletedOrNull = value?.IsCompleted ?? true;

                if (_cancellationTokenSource is not null)
                {
                    PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
                    PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
                }

                // The branch is on a condition evaluated before raising the events above if
                // needed, to avoid race conditions with a task completing right after them.
                if (isAlreadyCompletedOrNull)
                    return;

                static async void MonitorTask(AsyncRelayCommand @this, Task task)
                {
                    await task.GetAwaitableWithoutEndValidation();

                    if (ReferenceEquals(@this._executionTask, task))
                    {
                        @this.PropertyChanged?.Invoke(@this, ExecutionTaskChangedEventArgs);
                        @this.PropertyChanged?.Invoke(@this, IsRunningChangedEventArgs);

                        if (@this._cancellationTokenSource is not null)
                            @this.PropertyChanged?.Invoke(@this, CanBeCanceledChangedEventArgs);

                        if ((@this._options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                            @this.CanExecuteChanged?.Invoke(@this, EventArgs.Empty);
                    }
                }

                MonitorTask(this, value!);
            }
        }

        /// <inheritdoc/>
        public bool CanBeCanceled => IsRunning && _cancellationTokenSource is { IsCancellationRequested: false };

        /// <inheritdoc/>
        public bool IsCancellationRequested => _cancellationTokenSource is { IsCancellationRequested: true };

        /// <inheritdoc/>
        public bool IsRunning => ExecutionTask is { IsCompleted: false };

        /// <inheritdoc/>
        bool ICancellationAwareCommand.IsCancellationSupported => _execute is null;

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            bool canExecute = _canExecute?.Invoke() != false;
            return canExecute && ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 || ExecutionTask is not { IsCompleted: false });
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            Task executionTask = ExecuteAsync(parameter);

            // If exceptions shouldn't flow to the task scheduler, await the resulting task. This is
            // delegated to a separate method to keep this one more compact in case the option is set.
            if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
                AwaitAndThrowIfFailed(executionTask);
        }

        /// <inheritdoc/>
        public Task ExecuteAsync(object? parameter)
        {
            try
            {
                Task executionTask;

                if (_execute is not null)
                {
                    // Non cancelable _command delegate
                    executionTask = ExecutionTask = _execute();
                }
                else
                {
                    // Cancel the previous operation, if one is pending
                    _cancellationTokenSource?.Cancel();

                    CancellationTokenSource cancellationTokenSource = _cancellationTokenSource = new();

                    // Invoke the cancelable _command delegate with a new linked token
                    executionTask = ExecutionTask = _cancelableExecute!(cancellationTokenSource.Token);
                }

                // If concurrent executions are disabled, notify the can _execute change as well
                if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);

                return executionTask;
            }
            catch (Exception ex)
            {
                var exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

                if (ex.InnerException != null)
                    return exceptionHandlerService.HandleExceptionAsync(ex.InnerException);
                else
                    return exceptionHandlerService.HandleExceptionAsync(ex);
            }
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            if (_cancellationTokenSource is CancellationTokenSource { IsCancellationRequested: false } cancellationTokenSource)
            {
                cancellationTokenSource.Cancel();

                PropertyChanged?.Invoke(this, CanBeCanceledChangedEventArgs);
                PropertyChanged?.Invoke(this, IsCancellationRequestedChangedEventArgs);
            }
        }

        /// <summary>
        /// Awaits an input <see cref="Task"/> and throws an exception on the calling context, if the task fails.
        /// </summary>
        /// <param name="executionTask">The input <see cref="Task"/> instance to await.</param>
        internal static async void AwaitAndThrowIfFailed(Task executionTask)
        {
            // Note: this method is purposefully an async void method awaiting the input task. This is done so that
            // if an async relay _command is invoked synchronously (ie. when Execute is called, eg. from a binding),
            // exceptions in the wrapped delegate will not be ignored or just become visible through the ExecutionTask
            // property, but will be rethrown in the original synchronization context by default. This makes the behavior
            // more consistent with how normal commands work (where exceptions are also just normally propagated to the
            // caller context), and avoids getting an app into an inconsistent state in case a method faults without
            // other components being notified. It is also possible to not await this task and to instead ignore exceptions
            // and then inspect them manually from the ExecutionTask property, by constructing an async _command instance
            // using the AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler option. That will cause this call to
            // be skipped, and exceptions will just either normally be available through that property, or will otherwise
            // flow to the static TaskScheduler.UnobservedTaskException event if otherwise unobserved (eg. for logging).
            try
            {
                await executionTask;
            }
            catch (Exception ex)
            {
                var exceptionHandlerService = ServiceLocator.Default.GetInstance<IExceptionHandlerService>();

                if (ex.InnerException != null)
                    exceptionHandlerService.HandleExceptionAsync(ex.InnerException).Await();
                else
                    exceptionHandlerService.HandleExceptionAsync(ex).Await();
            }
        }
    }
}
