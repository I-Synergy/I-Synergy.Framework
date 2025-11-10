using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Services.Executors;
using ISynergy.Framework.Automations.Services.Operators;
using ISynergy.Framework.Automations.Tests.Fixtures;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Reqnroll;
using System.Diagnostics;

namespace ISynergy.Framework.Automations.Tests.StepDefinitions;

/// <summary>
/// Step definitions for automation workflow execution scenarios.
/// Demonstrates BDD testing for automation framework workflows.
/// </summary>
[Binding]
public class AutomationWorkflowExecutionSteps
{
    private readonly ILogger<AutomationWorkflowExecutionSteps> _logger;
    private readonly AutomationTestContext _context;
    private IAutomationService? _automationService;
    private Automation? _automation;
    private Customer? _customer;
    private ActionResult? _result;
    private Exception? _caughtException;
    private Stopwatch _stopwatch = new Stopwatch();
    private TimeSpan _expectedDelay;
    private int _initialAge;
    private int _repeatCount;

    public AutomationWorkflowExecutionSteps(AutomationTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<AutomationWorkflowExecutionSteps>();
        _context = context;
    }

    [Given(@"the automation service is initialized")]
    public void GivenTheAutomationServiceIsInitialized()
    {
        _logger.LogInformation("Initializing automation service");

        // Setup dependency injection container for real implementations
        var services = new ServiceCollection();

        // Register action executors
        services.AddScoped<IActionExecutor<DelayAction>, DelayActionExecutor>();
        services.AddScoped<IActionExecutor<CommandAction>, CommandActionExecutor>();
        services.AddScoped<IActionExecutor<AutomationAction>, AutomationActionExecutor>();

        services.AddScoped<IActionExecutorFactory, ActionExecutorFactory>();

        // Register operator strategies by concrete type
        services.AddScoped<AndOperatorStrategy>();
        services.AddScoped<OrOperatorStrategy>();

        services.AddScoped<IOperatorStrategyFactory, OperatorStrategyFactory>();

        var serviceProvider = services.BuildServiceProvider();

        // Get the dependencies from the service provider
        var executorFactory = serviceProvider.GetRequiredService<IActionExecutorFactory>();
        var operatorStrategyFactory = serviceProvider.GetRequiredService<IOperatorStrategyFactory>();

        // Create the queue builder with real executor factory
        var queueBuilder = new ActionQueueBuilder(executorFactory, null!);

        // Use real condition validator that actually checks conditions
        var conditionValidator = new AutomationConditionValidator(operatorStrategyFactory);

        _automationService = new AutomationService(
            Mock.Of<IAutomationManager>(),
            Mock.Of<IOptions<AutomationOptions>>(),
            conditionValidator,
            queueBuilder,
            Mock.Of<ILogger<AutomationService>>());

        // Share with other step classes
        _context.AutomationService = _automationService;

        ArgumentNullException.ThrowIfNull(_automationService);
    }

    [Given(@"I have a valid customer object")]
    public void GivenIHaveAValidCustomerObject()
    {
        _logger.LogInformation("Creating valid customer object");
        _customer = new Customer
        {
            Name = "Test Customer",
            Email = "test@example.com",
            Age = 25,
            Active = false
        };

        // Share with other step classes
        _context.Customer = _customer;
    }

    [Given(@"I have a customer object")]
    public void GivenIHaveACustomerObject()
    {
        GivenIHaveAValidCustomerObject();
    }

    [Given(@"I have an automation with age validation condition")]
    public void GivenIHaveAnAutomationWithAgeValidationCondition()
    {
        _logger.LogInformation("Creating automation with age validation");
        _automation = new Automation { IsActive = true };

        _automation.Conditions.Add(new Condition<Customer>(_automation.AutomationId, (e) => !string.IsNullOrEmpty(e.Name)));
        _automation.Conditions.Add(new Condition<Customer>(_automation.AutomationId, (e) => !string.IsNullOrEmpty(e.Email)));
        _automation.Conditions.Add(new Condition<Customer>(_automation.AutomationId, (e) => e.Age >= 18));
    }

    [Given(@"the customer age is (.*) or older")]
    public void GivenTheCustomerAgeIsOrOlder(int age)
    {
        _logger.LogInformation("Setting customer age to {Age}", age);
        ArgumentNullException.ThrowIfNull(_customer);
        _customer.Age = age;
    }

    [Given(@"the customer age is below (.*)")]
    public void GivenTheCustomerAgeIsBelow(int age)
    {
        _logger.LogInformation("Setting customer age below {Age}", age);
        ArgumentNullException.ThrowIfNull(_customer);
        _customer.Age = age - 2; // Set to 16 for 18
    }

    [Given(@"I have an automation with a delay action of (.*) seconds")]
    public void GivenIHaveAnAutomationWithADelayActionOfSeconds(int seconds)
    {
        _logger.LogInformation("Creating automation with {Seconds} second delay", seconds);
        _automation = new Automation { IsActive = true };
        _expectedDelay = TimeSpan.FromSeconds(seconds);
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, _expectedDelay));
    }

    [Given(@"I have an automation with command actions")]
    public void GivenIHaveAnAutomationWithCommandActions()
    {
        _logger.LogInformation("Creating automation with command actions");
        ArgumentNullException.ThrowIfNull(_customer);

        _automation = new Automation { IsActive = true };
        _initialAge = _customer.Age;

        RelayCommand<Customer> command = new((e) => e!.Age += 5);
        _automation.Actions.Add(new CommandAction(_automation.AutomationId, command, _customer));
    }

    [Given(@"the commands modify customer properties")]
    public void GivenTheCommandsModifyCustomerProperties()
    {
        _logger.LogInformation("Commands configured to modify customer properties");
        // Already configured in previous step
    }

    [Given(@"I have an automation with repeat actions")]
    public void GivenIHaveAnAutomationWithRepeatActions()
    {
        _logger.LogInformation("Creating automation with repeat actions");
        ArgumentNullException.ThrowIfNull(_customer);

        _automation = new Automation { IsActive = true };
        _initialAge = _customer.Age;
        _customer.Age = 20; // Start at 20

        RelayCommand<Customer> command = new((e) => e!.Age += 1);
        _automation.Actions.Add(new CommandAction(_automation.AutomationId, command, _customer));
    }

    [Given(@"the repeat is configured to run until a condition")]
    public void GivenTheRepeatIsConfiguredToRunUntilACondition()
    {
        _logger.LogInformation("Configuring repeat until condition");
        ArgumentNullException.ThrowIfNull(_automation);
        ArgumentNullException.ThrowIfNull(_customer);

        _repeatCount = 9; // Start at 20, repeat 9 times = 29, then one final check gets to 30
        _automation.Actions.Add(new RepeatPreviousAction<Customer>(
            _automation.AutomationId,
            RepeatTypes.Until,
            (e) => e.Age >= 30,
            _repeatCount));
    }

    [Given(@"I have an automation with a (.*) second timeout")]
    public void GivenIHaveAnAutomationWithASecondTimeout(int seconds)
    {
        _logger.LogInformation("Creating automation with {Seconds} second timeout", seconds);
        _automation = new Automation
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(seconds)
        };
    }

    [Given(@"the automation has actions exceeding the timeout")]
    public void GivenTheAutomationHasActionsExceedingTheTimeout()
    {
        _logger.LogInformation("Adding actions that exceed timeout");
        ArgumentNullException.ThrowIfNull(_automation);

        // Add a delay longer than the timeout
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromSeconds(10)));
    }

    [When(@"I execute the automation")]
    public async Task WhenIExecuteTheAutomation()
    {
        _logger.LogInformation("Executing automation");
        ArgumentNullException.ThrowIfNull(_automationService);
        ArgumentNullException.ThrowIfNull(_automation);

        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            _stopwatch.Restart();
            _result = await _automationService.ExecuteAsync(_automation, _customer, cancellationTokenSource);
            _stopwatch.Stop();
            cancellationTokenSource.Dispose();
        }
        catch (Exception ex)
        {
            _stopwatch.Stop();
            _logger.LogWarning(ex, "Caught exception during execution");
            _caughtException = ex;
        }
    }

    [Then(@"the automation should succeed")]
    public void ThenTheAutomationShouldSucceed()
    {
        _logger.LogInformation("Verifying automation succeeded");

        if (_caughtException != null)
        {
            throw new InvalidOperationException($"Expected success but got exception: {_caughtException.Message}", _caughtException);
        }

        ArgumentNullException.ThrowIfNull(_result);

        if (!_result.Succeeded)
        {
            throw new InvalidOperationException("Expected automation to succeed");
        }
    }

    [Then(@"all actions should be executed")]
    public void ThenAllActionsShouldBeExecuted()
    {
        _logger.LogInformation("Verifying all actions executed");
        ArgumentNullException.ThrowIfNull(_result);

        if (!_result.Succeeded)
        {
            throw new InvalidOperationException("Actions did not execute successfully");
        }
    }

    [Then(@"the automation should fail")]
    public void ThenTheAutomationShouldFail()
    {
        _logger.LogInformation("Verifying automation failed");
        ArgumentNullException.ThrowIfNull(_result);

        if (_result.Succeeded)
        {
            throw new InvalidOperationException("Expected automation to fail");
        }
    }

    [Then(@"no actions should be executed")]
    public void ThenNoActionsShouldBeExecuted()
    {
        _logger.LogInformation("Verifying no actions executed");
        ArgumentNullException.ThrowIfNull(_result);

        if (_result.Succeeded)
        {
            throw new InvalidOperationException("Expected no actions to execute");
        }
    }

    [Then(@"the execution time should be approximately (.*) seconds")]
    public void ThenTheExecutionTimeShouldBeApproximatelySeconds(int seconds)
    {
        _logger.LogInformation("Verifying execution time");

        var tolerance = TimeSpan.FromMilliseconds(500); // 500ms tolerance
        var expected = TimeSpan.FromSeconds(seconds);
        var lowerBound = expected - tolerance;
        var upperBound = expected + tolerance;

        if (_stopwatch.Elapsed < lowerBound || _stopwatch.Elapsed > upperBound)
        {
            throw new InvalidOperationException(
            $"Expected execution time around {expected.TotalSeconds}s but got {_stopwatch.Elapsed.TotalSeconds}s");
        }

        _logger.LogInformation("Execution time: {Elapsed}ms", _stopwatch.Elapsed.TotalMilliseconds);
    }

    [Then(@"the customer properties should be modified")]
    public void ThenTheCustomerPropertiesShouldBeModified()
    {
        _logger.LogInformation("Verifying customer properties were modified");
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_result);

        var resultCustomer = _result.Result as Customer;
        ArgumentNullException.ThrowIfNull(resultCustomer);

        if (resultCustomer.Age == _initialAge)
        {
            throw new InvalidOperationException("Customer age should have been modified");
        }

        _logger.LogInformation("Age changed from {Initial} to {Current}", _initialAge, resultCustomer.Age);
    }

    [Then(@"the action should repeat the specified number of times")]
    public void ThenTheActionShouldRepeatTheSpecifiedNumberOfTimes()
    {
        _logger.LogInformation("Verifying action repeated correctly");
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_result);

        var resultCustomer = _result.Result as Customer;
        ArgumentNullException.ThrowIfNull(resultCustomer);

        // Started at 20, should reach 30 (10 increments)
        if (resultCustomer.Age != 30)
        {
            throw new InvalidOperationException($"Expected age to be 30 but got {resultCustomer.Age}");
        }

        _logger.LogInformation("Action repeated successfully, final age: {Age}", resultCustomer.Age);
    }

    [Then(@"the automation should be cancelled")]
    public void ThenTheAutomationShouldBeCancelled()
    {
        _logger.LogInformation("Verifying automation was cancelled");

        if (_caughtException == null)
        {
            throw new InvalidOperationException("Expected cancellation exception");
        }
    }

    [Then(@"a timeout exception should be raised")]
    public void ThenATimeoutExceptionShouldBeRaised()
    {
        _logger.LogInformation("Verifying timeout exception");
        ArgumentNullException.ThrowIfNull(_caughtException);

        if (_caughtException is not TaskCanceledException)
        {
            throw new InvalidOperationException($"Expected TaskCanceledException but got {_caughtException.GetType().Name}");
        }

        _logger.LogInformation("Timeout exception raised as expected");
    }
}
