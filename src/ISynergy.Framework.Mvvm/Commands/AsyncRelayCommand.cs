using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Enumerations;
using System.Runtime.CompilerServices;

#nullable enable

namespace ISynergy.Framework.Mvvm.Commands;

/// <summary>
/// A _command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
/// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the _execute
/// action, and providing an ExecutionTask property that notifies changes when
/// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
/// </summary>
public sealed class AsyncRelayCommand : BaseAsyncRelayCommand
{
    private readonly Func<Task>? _execute;
    private readonly Func<CancellationToken, Task>? _cancelableExecute;
    private readonly Func<bool>? _canExecute;

    protected override bool IsBaseExecute => _execute is not null;

    public AsyncRelayCommand(Func<Task> execute)
        : base(AsyncRelayCommandOptions.None)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    public AsyncRelayCommand(Func<Task> execute, AsyncRelayCommandOptions options)
        : base(options)
    {
        Argument.IsNotNull(execute);
        _execute = execute;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute)
        : base(AsyncRelayCommandOptions.None)
    {
        Argument.IsNotNull(cancelableExecute);
        _cancelableExecute = cancelableExecute;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
        : base(options)
    {
        Argument.IsNotNull(cancelableExecute);
        _cancelableExecute = cancelableExecute;
    }

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        : this(execute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, AsyncRelayCommandOptions options)
        : this(execute, options)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
        : this(cancelableExecute)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, AsyncRelayCommandOptions options)
        : this(cancelableExecute, options)
    {
        Argument.IsNotNull(canExecute);
        _canExecute = canExecute;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool CanExecute(object? parameter)
    {
        bool canExecute = _canExecute?.Invoke() != false;
        return canExecute && ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) != 0 || ExecutionTask is not { IsCompleted: false });
    }

    public override void Execute(object? parameter)
    {
        Task executionTask = ExecuteAsync(parameter);

        if ((_options & AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler) == 0)
            AwaitAndThrowIfFailed(executionTask);
    }

    public override async Task ExecuteAsync(object? parameter)
    {
        if (IsRunning && (_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
            return;

        try
        {
            Task executionTask;

            if (_execute is not null)
            {
                executionTask = _execute();
            }
            else
            {
                _cancellationTokenSource?.Cancel();
                var cancellationTokenSource = _cancellationTokenSource = new();
                _cancellationTokenSources.Add(cancellationTokenSource);

                executionTask = _cancelableExecute!(cancellationTokenSource.Token);
            }

            ExecutionTask = executionTask;
            await executionTask;
        }
        catch (Exception ex)
        {
            var exceptionHandlerService = ServiceLocator.Default.GetService<IExceptionHandlerService>();
            if (ex.InnerException != null)
                await exceptionHandlerService.HandleExceptionAsync(ex.InnerException);
            else
                await exceptionHandlerService.HandleExceptionAsync(ex);
        }
        finally
        {
            ExecutionTask = null;

            if ((_options & AsyncRelayCommandOptions.AllowConcurrentExecutions) == 0)
                OnCanExecuteChanged();
        }
    }
}
