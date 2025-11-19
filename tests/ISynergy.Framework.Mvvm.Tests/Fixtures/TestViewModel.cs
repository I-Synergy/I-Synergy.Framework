using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Mvvm.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Tests.Fixtures;

/// <summary>
/// Test ViewModel for BDD scenarios.
/// Demonstrates MVVM patterns including commands, property notification, and lifecycle.
/// </summary>
public class TestViewModel : ObservableClass
{
    // Properties
    public string Name
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string FirstName
    {
        get => GetValue<string>();
        set
        {
            SetValue(value);
            RaisePropertyChanged(nameof(FullName));
        }
    }

    public string LastName
    {
        get => GetValue<string>();
        set
        {
            SetValue(value);
            RaisePropertyChanged(nameof(FullName));
        }
    }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Title
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string Subtitle
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public bool IsBusy
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public bool IsInitialized
    {
        get => GetValue<bool>();
        private set => SetValue(value);
    }

    public bool CanExecuteCommand
    {
        get => GetValue<bool>();
        set
        {
            SetValue(value);
            TestCommand?.NotifyCanExecuteChanged();
        }
    }

    // Commands
    public RelayCommand? TestCommand { get; private set; }
    public RelayCommand<string>? ParameterCommand { get; private set; }
    public AsyncRelayCommand? AsyncCommand { get; private set; }
    public AsyncRelayCommand? CancellableAsyncCommand { get; private set; }

    // Tracking
    public int CommandExecutionCount { get; private set; }
    public string? ReceivedParameter { get; private set; }
    public bool AsyncCommandExecuted { get; private set; }
    public bool WasCancelled { get; private set; }

    public TestViewModel()
    {
        // Set default values
        SetValue(string.Empty, nameof(Name));
        SetValue(string.Empty, nameof(FirstName));
        SetValue(string.Empty, nameof(LastName));
        SetValue(string.Empty, nameof(Title));
        SetValue(string.Empty, nameof(Subtitle));
        SetValue(false, nameof(IsBusy));
        SetValue(false, nameof(IsInitialized));
        SetValue(true, nameof(CanExecuteCommand));

        InitializeCommands();
    }

    private void InitializeCommands()
    {
        TestCommand = new RelayCommand(ExecuteTestCommand, () => CanExecuteCommand);
        ParameterCommand = new RelayCommand<string>(ExecuteParameterCommand);
        AsyncCommand = new AsyncRelayCommand(ExecuteAsyncCommandAsync);
        CancellableAsyncCommand = new AsyncRelayCommand(ExecuteCancellableAsync);
    }

    private void ExecuteTestCommand()
    {
        CommandExecutionCount++;
    }

    private void ExecuteParameterCommand(string? parameter)
    {
        ReceivedParameter = parameter;
        CommandExecutionCount++;
    }

    private async Task ExecuteAsyncCommandAsync()
    {
        IsBusy = true;
        await Task.Delay(100);
        AsyncCommandExecuted = true;
        IsBusy = false;
    }

    private async Task ExecuteCancellableAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        try
        {
            await Task.Delay(1000, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            WasCancelled = true;
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task InitializeAsync()
    {
        await Task.Delay(10); // Simulate initialization
        IsInitialized = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Cleanup resources
        }
        base.Dispose(disposing);
    }
}
