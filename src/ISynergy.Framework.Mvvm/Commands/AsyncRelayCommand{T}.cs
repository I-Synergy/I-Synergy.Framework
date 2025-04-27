using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A generic _command that provides a more specific version of <see cref="AsyncRelayCommand"/>.
/// </summary>
/// <typeparam name="T">The type of parameter being passed as input to the callbacks.</typeparam>
public sealed class AsyncRelayCommand<T> : BaseAsyncRelayCommand, IAsyncRelayCommand<T>
{
    private readonly Func<T, Task>? _execute;
    private readonly Func<T, CancellationToken, Task>? _cancelableExecute;
    private readonly Predicate<T>? _canExecute;

    protected override bool IsBaseExecute => _execute is not null;

    public AsyncRelayCommand(Func<T, Task> execute)
        : base(AsyncRelayCommandOptions.None)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    public AsyncRelayCommand(Func<T, Task> execute, AsyncRelayCommandOptions options)
        : base(options)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute)
        : base(AsyncRelayCommandOptions.None)
    {
        Argument.IsNotNull(cancelableExecute);
        _cancelableExecute = cancelableExecute;
    }

    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
        : base(options)
    {
        Argument.IsNotNull(cancelableExecute);
        _cancelableExecute = cancelableExecute;
    }

    public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute)
        : this(execute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute, AsyncRelayCommandOptions options)
        : this(execute, options)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, Predicate<T> canExecute)
        : this(cancelableExecute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    public AsyncRelayCommand(Func<T, CancellationToken, Task> cancelableExecute, Predicate<T> canExecute, AsyncRelayCommandOptions options)
        : this(cancelableExecute, options)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(T? parameter)
    {
        var canExecute = _canExecute?.Invoke(parameter!) != false;
        return canExecute && ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 || ExecutionTask is not { IsCompleted: false });
    }

    public override bool CanExecute(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
        {
            if (parameter is not null)
                RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);
        }

        return CanExecute(result);
    }

    public void Execute(T? parameter)
    {
        Task executionTask = ExecuteAsync(parameter);

        if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
            AwaitAndThrowIfFailed(executionTask);
    }

    public override void Execute(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        Execute(result);
    }

    public async Task ExecuteAsync(T? parameter)
    {
        if (IsRunning && (_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
            return;

        try
        {
            Task executionTask;

            if (_execute is not null)
            {
                executionTask = _execute(parameter!);
            }
            else
            {
                _cancellationTokenSource?.Cancel();
                var cancellationTokenSource = _cancellationTokenSource = new();
                _cancellationTokenSources.Add(cancellationTokenSource);

                executionTask = _cancelableExecute!(parameter!, cancellationTokenSource.Token);
            }

            ExecutionTask = executionTask;
            await executionTask;
        }
        finally
        {
            ExecutionTask = null;

            if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                OnCanExecuteChanged();
        }
    }

    public override async Task ExecuteAsync(object? parameter)
    {
        if (!RelayCommand<T>.TryGetCommandArgument(parameter, out T? result))
            RelayCommand<T>.ThrowArgumentExceptionForInvalidCommandArgument(parameter);

        await ExecuteAsync(result);
    }
}
