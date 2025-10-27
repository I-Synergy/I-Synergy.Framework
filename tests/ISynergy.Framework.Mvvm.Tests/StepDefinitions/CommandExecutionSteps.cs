using ISynergy.Framework.Mvvm.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.Mvvm.Tests.StepDefinitions;

/// <summary>
/// Step definitions for command execution scenarios.
/// Demonstrates BDD testing for MVVM command patterns.
/// </summary>
[Binding]
public class CommandExecutionSteps
{
    private readonly ILogger<CommandExecutionSteps> _logger;
    private readonly MvvmTestContext _context;

    private Task? _runningCommandTask;

    public CommandExecutionSteps(MvvmTestContext context)
    {
    var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
 _logger = loggerFactory.CreateLogger<CommandExecutionSteps>();
        _context = context;
    }

    [Given(@"I have a test ViewModel")]
    public void GivenIHaveATestViewModel()
 {
        _logger.LogInformation("Creating test ViewModel");
   _context.ViewModel = new TestViewModel();
        _context.SubscribeToPropertyChanged();
    }

 [Given(@"the ViewModel has a RelayCommand")]
    public void GivenTheViewModelHasARelayCommand()
  {
        _logger.LogInformation("ViewModel has RelayCommand configured");
     ArgumentNullException.ThrowIfNull(_context.ViewModel);
        ArgumentNullException.ThrowIfNull(_context.ViewModel.TestCommand);
    }

    [Given(@"the ViewModel has an AsyncRelayCommand")]
    public void GivenTheViewModelHasAnAsyncRelayCommand()
    {
_logger.LogInformation("ViewModel has AsyncRelayCommand configured");
 ArgumentNullException.ThrowIfNull(_context.ViewModel);
ArgumentNullException.ThrowIfNull(_context.ViewModel.AsyncCommand);
    }

    [Given(@"the ViewModel has a command with CanExecute logic")]
    public void GivenTheViewModelHasACommandWithCanExecuteLogic()
    {
        _logger.LogInformation("ViewModel has command with CanExecute logic");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
        ArgumentNullException.ThrowIfNull(_context.ViewModel.TestCommand);
    }

    [Given(@"the CanExecute condition returns false")]
    public void GivenTheCanExecuteConditionReturnsFalse()
    {
        _logger.LogInformation("Setting CanExecute to false");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
     _context.ViewModel.CanExecuteCommand = false;
  }

    [Given(@"the ViewModel has a RelayCommand that accepts a parameter")]
    public void GivenTheViewModelHasARelayCommandThatAcceptsAParameter()
  {
        _logger.LogInformation("ViewModel has parameterized RelayCommand");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);
ArgumentNullException.ThrowIfNull(_context.ViewModel.ParameterCommand);
    }

    [Given(@"the ViewModel has a cancellable AsyncRelayCommand")]
    public void GivenTheViewModelHasACancellableAsyncRelayCommand()
    {
        _logger.LogInformation("ViewModel has cancellable AsyncRelayCommand");
   ArgumentNullException.ThrowIfNull(_context.ViewModel);
   ArgumentNullException.ThrowIfNull(_context.ViewModel.CancellableAsyncCommand);
    }

    [Given(@"the ViewModel has a command with dynamic CanExecute")]
  public void GivenTheViewModelHasACommandWithDynamicCanExecute()
    {
   _logger.LogInformation("ViewModel has command with dynamic CanExecute");
ArgumentNullException.ThrowIfNull(_context.ViewModel);

     // Subscribe to CanExecuteChanged
  if (_context.ViewModel.TestCommand != null)
{
_context.ViewModel.TestCommand.CanExecuteChanged += (s, e) => _context.CanExecuteChangedFired = true;
  }
    }

    [When(@"I execute the command")]
    public void WhenIExecuteTheCommand()
    {
  _logger.LogInformation("Executing command");
   ArgumentNullException.ThrowIfNull(_context.ViewModel);

        try
  {
     _context.ViewModel.TestCommand?.Execute(null);
        }
        catch (Exception ex)
        {
  _logger.LogError(ex, "Error executing command");
            _context.CaughtException = ex;
   }
    }

    [When(@"I execute the async command")]
    public async Task WhenIExecuteTheAsyncCommand()
    {
_logger.LogInformation("Executing async command");
    ArgumentNullException.ThrowIfNull(_context.ViewModel);

   try
        {
            await _context.ViewModel.AsyncCommand!.ExecuteAsync(null);
        }
        catch (Exception ex)
   {
    _logger.LogError(ex, "Error executing async command");
        _context.CaughtException = ex;
     }
    }

    [When(@"I attempt to execute the command")]
    public void WhenIAttemptToExecuteTheCommand()
    {
   _logger.LogInformation("Attempting to execute command with CanExecute = false");
  ArgumentNullException.ThrowIfNull(_context.ViewModel);

        if (_context.ViewModel.TestCommand?.CanExecute(null) == true)
        {
    _context.ViewModel.TestCommand.Execute(null);
}
        else
 {
   _logger.LogInformation("Command execution prevented by CanExecute");
        }
    }

    [When(@"I execute the command with parameter ""(.*)""")]
 public void WhenIExecuteTheCommandWithParameter(string parameter)
    {
   _logger.LogInformation("Executing command with parameter: {Parameter}", parameter);
   ArgumentNullException.ThrowIfNull(_context.ViewModel);

        try
   {
         _context.ViewModel.ParameterCommand?.Execute(parameter);
  }
  catch (Exception ex)
   {
  _logger.LogError(ex, "Error executing command with parameter");
   _context.CaughtException = ex;
        }
    }

    [When(@"I start executing the async command")]
    public void WhenIStartExecutingTheAsyncCommand()
  {
   _logger.LogInformation("Starting async command execution");
  ArgumentNullException.ThrowIfNull(_context.ViewModel);

    _context.CancellationTokenSource = new CancellationTokenSource();
 
// Start the command and store the task
_runningCommandTask = _context.ViewModel.CancellableAsyncCommand!.ExecuteAsync(_context.CancellationTokenSource.Token);
        
     // Give it a moment to start
        Thread.Sleep(100);
 }

    [When(@"I cancel the command execution")]
    public async Task WhenICancelTheCommandExecution()
    {
     _logger.LogInformation("Cancelling command execution");
 ArgumentNullException.ThrowIfNull(_context.CancellationTokenSource);
      ArgumentNullException.ThrowIfNull(_runningCommandTask);
    
      _context.CancellationTokenSource.Cancel();

   // Wait for the task to complete (it should throw OperationCanceledException)
  try
    {
     await _runningCommandTask;
      }
  catch (OperationCanceledException)
        {
   // Expected - cancellation was successful
      _logger.LogInformation("Command cancellation processed successfully");
}
 }

    [When(@"a property that affects CanExecute changes")]
    public void WhenAPropertyThatAffectsCanExecuteChanges()
{
 _logger.LogInformation("Changing property that affects CanExecute");
ArgumentNullException.ThrowIfNull(_context.ViewModel);
        _context.ViewModel.CanExecuteCommand = false;
    }

    [Then(@"the command handler should be invoked")]
    public void ThenTheCommandHandlerShouldBeInvoked()
    {
   _logger.LogInformation("Verifying command handler was invoked");
     ArgumentNullException.ThrowIfNull(_context.ViewModel);

     if (_context.ViewModel.CommandExecutionCount == 0)
{
   throw new InvalidOperationException("Expected command handler to be invoked");
      }
    }

    [Then(@"the command execution count should be (.*)")]
    public void ThenTheCommandExecutionCountShouldBe(int expectedCount)
    {
        _logger.LogInformation("Verifying execution count: {Count}", expectedCount);
        ArgumentNullException.ThrowIfNull(_context.ViewModel);

        if (_context.ViewModel.CommandExecutionCount != expectedCount)
    {
      throw new InvalidOperationException($"Expected {expectedCount} executions but got {_context.ViewModel.CommandExecutionCount}");
}
    }

    [Then(@"the async command handler should be invoked")]
    public void ThenTheAsyncCommandHandlerShouldBeInvoked()
    {
   _logger.LogInformation("Verifying async command handler was invoked");
  ArgumentNullException.ThrowIfNull(_context.ViewModel);

  if (!_context.ViewModel.AsyncCommandExecuted)
        {
            throw new InvalidOperationException("Expected async command to be executed");
 }
    }

    [Then(@"the command should complete successfully")]
    public void ThenTheCommandShouldCompleteSuccessfully()
  {
        _logger.LogInformation("Verifying command completed successfully");

if (_context.CaughtException != null)
{
  throw new InvalidOperationException($"Expected successful completion but got exception: {_context.CaughtException.Message}");
        }
    }

    [Then(@"the command should not execute")]
    public void ThenTheCommandShouldNotExecute()
  {
        _logger.LogInformation("Verifying command did not execute");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);

        if (_context.ViewModel.CommandExecutionCount > 0)
        {
throw new InvalidOperationException("Expected command not to execute");
  }
    }

    [Then(@"the execution count should remain (.*)")]
 public void ThenTheExecutionCountShouldRemain(int expectedCount)
  {
     ThenTheCommandExecutionCountShouldBe(expectedCount);
    }

    [Then(@"the command handler should receive the parameter")]
    public void ThenTheCommandHandlerShouldReceiveTheParameter()
    {
  _logger.LogInformation("Verifying parameter was received");
        ArgumentNullException.ThrowIfNull(_context.ViewModel);

     if (string.IsNullOrEmpty(_context.ViewModel.ReceivedParameter))
        {
throw new InvalidOperationException("Expected parameter to be received");
 }
    }

    [Then(@"the parameter value should be ""(.*)""")]
    public void ThenTheParameterValueShouldBe(string expectedParameter)
    {
        _logger.LogInformation("Verifying parameter value: {Expected}", expectedParameter);
        ArgumentNullException.ThrowIfNull(_context.ViewModel);

        if (_context.ViewModel.ReceivedParameter != expectedParameter)
 {
throw new InvalidOperationException($"Expected parameter '{expectedParameter}' but got '{_context.ViewModel.ReceivedParameter}'");
   }
    }

    [Then(@"the command should be cancelled")]
    public void ThenTheCommandShouldBeCancelled()
    {
 _logger.LogInformation("Verifying command was cancelled");
  ArgumentNullException.ThrowIfNull(_context.ViewModel);

      if (!_context.ViewModel.WasCancelled)
   {
     _logger.LogWarning("WasCancelled is false. Cancellation might not have been processed yet.");
          throw new InvalidOperationException("Expected command to be cancelled");
      }
        
    _logger.LogInformation("Command successfully cancelled");
    }

    [Then(@"the cancellation token should be triggered")]
    public void ThenTheCancellationTokenShouldBeTriggered()
    {
        _logger.LogInformation("Verifying cancellation token");
        ArgumentNullException.ThrowIfNull(_context.CancellationTokenSource);

      if (!_context.CancellationTokenSource.Token.IsCancellationRequested)
     {
      throw new InvalidOperationException("Expected cancellation token to be triggered");
        }
    }

    [Then(@"CanExecuteChanged event should be raised")]
  public void ThenCanExecuteChangedEventShouldBeRaised()
    {
        _logger.LogInformation("Verifying CanExecuteChanged event");

  if (!_context.CanExecuteChangedFired)
        {
            throw new InvalidOperationException("Expected CanExecuteChanged event to be raised");
}
  }

    [Then(@"the command CanExecute should reflect the new state")]
    public void ThenTheCommandCanExecuteShouldReflectTheNewState()
    {
  _logger.LogInformation("Verifying CanExecute state");
     ArgumentNullException.ThrowIfNull(_context.ViewModel);

  var canExecute = _context.ViewModel.TestCommand?.CanExecute(null) ?? false;

        if (canExecute != _context.ViewModel.CanExecuteCommand)
        {
   throw new InvalidOperationException("CanExecute state does not match expected value");
   }
    }
}
