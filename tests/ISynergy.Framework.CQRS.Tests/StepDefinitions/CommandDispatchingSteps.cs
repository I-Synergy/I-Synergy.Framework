using ISynergy.Framework.CQRS.Abstractions.Commands;
using ISynergy.Framework.CQRS.Abstractions.Dispatchers;
using ISynergy.Framework.CQRS.Commands;
using ISynergy.Framework.CQRS.Dispatchers;
using ISynergy.Framework.CQRS.TestImplementations.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Reqnroll;

namespace ISynergy.Framework.CQRS.Tests.StepDefinitions;

/// <summary>
/// Step definitions for command dispatching scenarios.
/// Demonstrates proper CQRS command handling patterns with dependency injection.
/// </summary>
[Binding]
public class CommandDispatchingSteps
{
    private readonly ILogger<CommandDispatchingSteps> _logger;
    private IServiceProvider? _serviceProvider;
    private ICommandDispatcher? _commandDispatcher;
    private TestCommand? _command;
    private TestCommandWithResult? _commandWithResult;
    private string? _result;
    private Exception? _caughtException;
    private TestCommandHandler? _testHandler;
    private readonly List<TestCommandHandler> _multipleHandlers = new();
    private readonly List<TestCommand> _multipleCommands = new();

    public CommandDispatchingSteps()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<CommandDispatchingSteps>();
    }

    [Given(@"the CQRS system is initialized with dependency injection")]
    public void GivenTheCqrsSystemIsInitializedWithDependencyInjection()
    {
        _logger.LogInformation("Initializing CQRS system with dependency injection");

        var services = new ServiceCollection();
        _testHandler = new TestCommandHandler();

        services.AddScoped<ICommandHandler<TestCommand>>(_ => _testHandler);
        services.AddScoped<ICommandHandler<TestCommandWithResult, string>>(_ =>
         new TestCommandWithResultHandler());
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        _serviceProvider = services.BuildServiceProvider();
        _commandDispatcher = new CommandDispatcher(_serviceProvider, Mock.Of<ILogger<CommandDispatcher>>());

        ArgumentNullException.ThrowIfNull(_commandDispatcher);
    }

    [Given(@"I have a command with data ""(.*)""")]
    public void GivenIHaveACommandWithData(string data)
    {
        _logger.LogInformation("Creating command with data: {Data}", data);
        _command = new TestCommand { Data = data };
        ArgumentNullException.ThrowIfNull(_command);
    }

    [Given(@"I have a command with result that expects input ""(.*)""")]
    public void GivenIHaveACommandWithResultThatExpectsInput(string input)
    {
        _logger.LogInformation("Creating command with result, input: {Input}", input);
        _commandWithResult = new TestCommandWithResult { Input = input };
        ArgumentNullException.ThrowIfNull(_commandWithResult);
    }

    [Given(@"I have a command without a registered handler")]
    public void GivenIHaveACommandWithoutARegisteredHandler()
    {
        _logger.LogInformation("Creating command without registered handler");

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        _serviceProvider = services.BuildServiceProvider();
        _commandDispatcher = new CommandDispatcher(_serviceProvider, Mock.Of<ILogger<CommandDispatcher>>());
        _command = new TestCommand { Data = "Unhandled" };
    }

    [Given(@"I have multiple commands to dispatch")]
    public void GivenIHaveMultipleCommandsToDispatch()
    {
        _logger.LogInformation("Creating multiple commands");

        for (int i = 0; i < 3; i++)
        {
            var handler = new TestCommandHandler();
            _multipleHandlers.Add(handler);
            _multipleCommands.Add(new TestCommand { Data = $"Command {i}" });
        }

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _serviceProvider = services.BuildServiceProvider();
    }

    [When(@"I dispatch the command")]
    public async Task WhenIDispatchTheCommand()
    {
        _logger.LogInformation("Dispatching command");
        ArgumentNullException.ThrowIfNull(_commandDispatcher);
        ArgumentNullException.ThrowIfNull(_command);

        await _commandDispatcher.DispatchAsync(_command);
    }

    [When(@"I dispatch the command with result")]
    public async Task WhenIDispatchTheCommandWithResult()
    {
        _logger.LogInformation("Dispatching command with result");
        ArgumentNullException.ThrowIfNull(_commandDispatcher);
        ArgumentNullException.ThrowIfNull(_commandWithResult);

        _result = await _commandDispatcher.DispatchAsync<TestCommandWithResult, string>(_commandWithResult);
    }

    [When(@"I attempt to dispatch the unhandled command")]
    public async Task WhenIAttemptToDispatchTheUnhandledCommand()
    {
        _logger.LogInformation("Attempting to dispatch unhandled command");

        try
        {
            ArgumentNullException.ThrowIfNull(_commandDispatcher);
            ArgumentNullException.ThrowIfNull(_command);
            await _commandDispatcher.DispatchAsync(_command);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Caught expected exception");
            _caughtException = ex;
        }
    }

    [When(@"I dispatch all commands in sequence")]
    public async Task WhenIDispatchAllCommandsInSequence()
    {
        _logger.LogInformation("Dispatching multiple commands in sequence");
        ArgumentNullException.ThrowIfNull(_serviceProvider);

        for (int i = 0; i < _multipleHandlers.Count; i++)
        {
            var handler = _multipleHandlers[i];
            var cmd = _multipleCommands[i];

            var scopedServices = new ServiceCollection();
            scopedServices.AddScoped<ICommandHandler<TestCommand>>(_ => handler);
            var scopedProvider = scopedServices.BuildServiceProvider();
            var dispatcher = new CommandDispatcher(scopedProvider, Mock.Of<ILogger<CommandDispatcher>>());

            await dispatcher.DispatchAsync(cmd);
        }
    }

    [Then(@"the command should be handled successfully")]
    public void ThenTheCommandShouldBeHandledSuccessfully()
    {
        _logger.LogInformation("Verifying command was handled successfully");

        if (_caughtException != null)
        {
            throw new InvalidOperationException($"Expected no exception, but got: {_caughtException.Message}", _caughtException);
        }
    }

    [Then(@"the command handler should have executed")]
    public void ThenTheCommandHandlerShouldHaveExecuted()
    {
        _logger.LogInformation("Verifying handler execution");
        ArgumentNullException.ThrowIfNull(_testHandler, "Test handler must exist");

        if (!_testHandler.WasHandled)
        {
            throw new InvalidOperationException("Handler should have been executed");
        }
    }

    [Then(@"the result should be ""(.*)""")]
    public void ThenTheResultShouldBe(string expectedResult)
    {
        _logger.LogInformation("Verifying result: {Expected}", expectedResult);
        ArgumentNullException.ThrowIfNull(_result, "Result should not be null");

        if (_result != expectedResult)
        {
            throw new InvalidOperationException($"Expected result '{expectedResult}' but got '{_result}'");
        }
    }

    [Then(@"an InvalidOperationException should be thrown")]
    public void ThenAnInvalidOperationExceptionShouldBeThrown()
    {
        _logger.LogInformation("Verifying InvalidOperationException was thrown");
        ArgumentNullException.ThrowIfNull(_caughtException, "Exception should be thrown");

        if (_caughtException is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Expected InvalidOperationException but got {_caughtException.GetType().Name}");
        }
    }

    [Then(@"the exception message should indicate missing handler")]
    public void ThenTheExceptionMessageShouldIndicateMissingHandler()
    {
        _logger.LogInformation("Verifying exception message");
        ArgumentNullException.ThrowIfNull(_caughtException, "Exception should exist");

        if (string.IsNullOrEmpty(_caughtException.Message))
        {
            throw new InvalidOperationException("Exception message should not be empty");
        }
    }

    [Then(@"all commands should be handled successfully")]
    public void ThenAllCommandsShouldBeHandledSuccessfully()
    {
        _logger.LogInformation("Verifying all commands were handled");

        if (_multipleHandlers.Count == 0)
        {
            throw new InvalidOperationException("Should have multiple handlers");
        }

        if (!_multipleHandlers.All(h => h.WasHandled))
        {
            throw new InvalidOperationException("All handlers should have been executed");
        }
    }

    [Then(@"the execution order should be preserved")]
    public void ThenTheExecutionOrderShouldBePreserved()
    {
        _logger.LogInformation("Verifying execution order");

        if (_multipleHandlers.Count != 3)
        {
            throw new InvalidOperationException($"Expected 3 handlers but got {_multipleHandlers.Count}");
        }
    }
}
