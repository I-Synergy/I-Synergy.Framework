using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Tests.Fixtures;
using ISynergy.Framework.Automations.Triggers;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Reqnroll;

namespace ISynergy.Framework.Automations.Tests.StepDefinitions;

/// <summary>
/// Step definitions for automation trigger scenarios.
/// Demonstrates BDD testing for automation framework triggers.
/// </summary>
[Binding]
public class AutomationTriggersSteps
{
    private readonly ILogger<AutomationTriggersSteps> _logger;
    private readonly AutomationTestContext _context;
    private IAutomationService? _automationService;
    private Automation? _automation;
    private Customer? _customer;
    private bool _triggerActivated;
    private object? _triggerValue;
    private Exception? _caughtException;
    private bool _fromState;
    private bool _toState;

    public AutomationTriggersSteps(AutomationTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<AutomationTriggersSteps>();
        _context = context;
    }

    // Note: "Given the automation service is initialized" step is defined in AutomationWorkflowExecutionSteps
    // Note: "And I have a valid customer object" step is defined in AutomationWorkflowExecutionSteps
    // Both step definitions are shared between workflow and trigger tests via AutomationTestContext

    [Given(@"I have a boolean state trigger")]
    public void GivenIHaveABooleanStateTrigger()
    {
        _logger.LogInformation("Preparing boolean state trigger");
        // Get shared instances from context
        _customer = _context.Customer;
        _automationService = _context.AutomationService;
    }

    [Given(@"I have an automation with a boolean state trigger")]
    public void GivenIHaveAnAutomationWithABooleanStateTrigger()
    {
        _logger.LogInformation("Creating automation with boolean state trigger");

        // Initialize if not already done by shared step
        if (_automationService == null)
        {
            _automationService = new AutomationService(
                Mock.Of<IAutomationManager>(),
                Mock.Of<IOptions<AutomationOptions>>(),
                Mock.Of<IAutomationConditionValidator>(),
                Mock.Of<IActionQueueBuilder>(),
                Mock.Of<ILogger<AutomationService>>());
        }

        if (_customer == null)
        {
            _customer = new Customer
            {
                Name = "Test Customer",
                Email = "test@example.com",
                Age = 25,
                Active = false
            };
        }

        _automation = new Automation
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };
    }

    [Given(@"the trigger monitors the customer Active property")]
    public void GivenTheTriggerMonitorsTheCustomerActiveProperty()
    {
        _logger.LogInformation("Configuring trigger to monitor Active property");
        // Configuration happens in next step
    }

    [Given(@"the trigger expects a change from false to true")]
    public void GivenTheTriggerExpectsAChangeFromFalseToTrue()
    {
        _logger.LogInformation("Setting trigger expectations: false -> true");
        ArgumentNullException.ThrowIfNull(_automation);
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_automationService);

        var trigger = new BooleanStateTrigger(
  _automation.AutomationId,
            () => new(_customer, _customer.GetProperty(x => x.Active)!),
     false, // from
            true,  // to
            async (active) =>
            {
                _logger.LogInformation("Boolean trigger activated with value: {Value}", active);
                _triggerActivated = true;
                _triggerValue = active;
                var cancellationTokenSource = new CancellationTokenSource();
                await _automationService.ExecuteAsync(_automation, _customer, cancellationTokenSource);
                cancellationTokenSource.Dispose();
            });

        _automation.Triggers.Add(trigger);
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromSeconds(1)));
    }

    [Given(@"I have an automation with an integer trigger")]
    public void GivenIHaveAnAutomationWithAnIntegerTrigger()
    {
        _logger.LogInformation("Creating automation with integer trigger");
        _automation = new Automation
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };
    }

    [Given(@"the trigger monitors the customer Age property")]
    public void GivenTheTriggerMonitorsTheCustomerAgeProperty()
    {
        _logger.LogInformation("Configuring trigger to monitor Age property");
        // Configuration happens in next step
    }

    [Given(@"the trigger expects the age to change")]
    public void GivenTheTriggerExpectsTheAgeToChange()
    {
        _logger.LogInformation("Setting up integer trigger for age changes");
        ArgumentNullException.ThrowIfNull(_automation);
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_automationService);

        var trigger = new IntegerTrigger(
_automation.AutomationId,
     () => new(_customer, (IProperty<int>)_customer.Properties[nameof(Customer.Age)]),
     65,  // to
        18,  // from
     async (age) =>
   {
       _logger.LogInformation("Integer trigger activated with age: {Age}", age);
       _triggerActivated = true;
       _triggerValue = age;
       var cancellationTokenSource = new CancellationTokenSource();
       await _automationService.ExecuteAsync(_automation, _customer, cancellationTokenSource);
       cancellationTokenSource.Dispose();
   });

        _automation.Triggers.Add(trigger);
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromSeconds(1)));
    }

    [Given(@"I have an automation with a string state trigger")]
    public void GivenIHaveAnAutomationWithAStringStateTrigger()
    {
        _logger.LogInformation("Creating automation with string state trigger");
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_automationService);

        _automation = new Automation
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };
    }

    [Given(@"the trigger monitors the customer Name property")]
    public void GivenTheTriggerMonitorsTheCustomerNameProperty()
    {
        _logger.LogInformation("Configuring string trigger for Name property");
        ArgumentNullException.ThrowIfNull(_automation);
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_automationService);

        var trigger = new StringStateTrigger(
   _automation.AutomationId,
            () => new(_customer, _customer.GetProperty(x => x.Name)!),
            _customer.Name,
        "New Name",
         async (newName) =>
 {
     _logger.LogInformation("String trigger activated with name: {Name}", newName);
     _triggerActivated = true;
     _triggerValue = newName;
     var cancellationTokenSource = new CancellationTokenSource();
     await _automationService.ExecuteAsync(_automation, _customer, cancellationTokenSource);
     cancellationTokenSource.Dispose();
 });

        _automation.Triggers.Add(trigger);
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromSeconds(1)));
    }

    [Given(@"I have an automation with an event trigger")]
    public void GivenIHaveAnAutomationWithAnEventTrigger()
    {
        _logger.LogInformation("Creating automation with event trigger");
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_automationService);

        _automation = new Automation
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };
    }

    [Given(@"the trigger monitors the customer Registered event")]
    public void GivenTheTriggerMonitorsTheCustomerRegisteredEvent()
    {
        _logger.LogInformation("Configuring event trigger for Registered event");
        ArgumentNullException.ThrowIfNull(_automation);
        ArgumentNullException.ThrowIfNull(_customer);
        ArgumentNullException.ThrowIfNull(_automationService);

        var trigger = new EventTrigger<Customer>(
   _automation.AutomationId,
            _customer,
            (s) => _customer.Registered += s,
   async (e) =>
   {
       _logger.LogInformation("Event trigger activated");
       _triggerActivated = true;
       _triggerValue = e;
       var cancellationTokenSource = new CancellationTokenSource();
       await _automationService.ExecuteAsync(_automation, e, cancellationTokenSource);
       cancellationTokenSource.Dispose();
   });

        _automation.Triggers.Add(trigger);
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromSeconds(1)));
    }

    [Given(@"the from and to states are the same")]
    public void GivenTheFromAndToStatesAreTheSame()
    {
        _logger.LogInformation("Setting same from/to states");
        _fromState = false;
        _toState = false;
    }

    [When(@"the customer Active property changes to true")]
    public async Task WhenTheCustomerActivePropertyChangesToTrue()
    {
        _logger.LogInformation("Changing Active property to true");
        ArgumentNullException.ThrowIfNull(_customer);

        _customer.Active = true;
        await Task.Delay(500); // Give trigger time to activate
    }

    [When(@"the customer age changes to (.*)")]
    public async Task WhenTheCustomerAgeChangesTo(int age)
    {
        _logger.LogInformation("Changing age to {Age}", age);
        ArgumentNullException.ThrowIfNull(_customer);

        _customer.Age = age;
        await Task.Delay(500); // Give trigger time to activate
    }

    [When(@"the customer name changes to a new value")]
    public async Task WhenTheCustomerNameChangesToANewValue()
    {
        _logger.LogInformation("Changing name to 'New Name'");
        ArgumentNullException.ThrowIfNull(_customer);

        _customer.Name = "New Name";
        await Task.Delay(500); // Give trigger time to activate
    }

    [When(@"the customer registration event is raised")]
    public async Task WhenTheCustomerRegistrationEventIsRaised()
    {
        _logger.LogInformation("Raising registration event");
        ArgumentNullException.ThrowIfNull(_customer);

        _customer.Register();
        await Task.Delay(500); // Give trigger time to activate
    }

    [When(@"I attempt to create the trigger")]
    public void WhenIAttemptToCreateTheTrigger()
    {
        _logger.LogInformation("Attempting to create trigger with same states");
        ArgumentNullException.ThrowIfNull(_customer);

        try
        {
            var automation = new Automation();
            var trigger = new BooleanStateTrigger(
                automation.AutomationId,
                  () => new(_customer, _customer.GetProperty(x => x.Active)!),
                      _fromState,
                  _toState,
            default!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Caught expected exception");
            _caughtException = ex;
        }
    }

    [Then(@"the automation should be triggered")]
    public void ThenTheAutomationShouldBeTriggered()
    {
        _logger.LogInformation("Verifying automation was triggered");

        if (!_triggerActivated)
        {
            throw new InvalidOperationException("Expected automation to be triggered");
        }
    }

    [Then(@"the automation should execute successfully")]
    public void ThenTheAutomationShouldExecuteSuccessfully()
    {
        _logger.LogInformation("Verifying automation executed successfully");

        if (!_triggerActivated)
        {
            throw new InvalidOperationException("Automation should have been triggered and executed");
        }
    }

    [Then(@"the trigger callback should receive the new value")]
    public void ThenTheTriggerCallbackShouldReceiveTheNewValue()
    {
        _logger.LogInformation("Verifying trigger received value: {Value}", _triggerValue);

        if (_triggerValue == null)
        {
            throw new InvalidOperationException("Trigger should have received a value");
        }
    }

    [Then(@"the new name should be passed to the callback")]
    public void ThenTheNewNameShouldBePassedToTheCallback()
    {
        _logger.LogInformation("Verifying new name in callback");

        if (_triggerValue is not string name || name != "New Name")
        {
            throw new InvalidOperationException($"Expected 'New Name' but got '{_triggerValue}'");
        }
    }

    [Then(@"the event arguments should be passed to the callback")]
    public void ThenTheEventArgumentsShouldBePassedToTheCallback()
    {
        _logger.LogInformation("Verifying event arguments in callback");

        if (_triggerValue is not Customer)
        {
            throw new InvalidOperationException("Expected Customer object in event args");
        }
    }

    [Then(@"an ArgumentException should be thrown")]
    public void ThenAnArgumentExceptionShouldBeThrown()
    {
        _logger.LogInformation("Verifying ArgumentException was thrown");

        if (_caughtException is not ArgumentException)
        {
            throw new InvalidOperationException($"Expected ArgumentException but got {_caughtException?.GetType().Name ?? "null"}");
        }
    }

    [Then(@"the exception should indicate invalid configuration")]
    public void ThenTheExceptionShouldIndicateInvalidConfiguration()
    {
        _logger.LogInformation("Verifying exception message");
        ArgumentNullException.ThrowIfNull(_caughtException);

        if (string.IsNullOrEmpty(_caughtException.Message))
        {
            throw new InvalidOperationException("Exception should have a message");
        }

        _logger.LogInformation("Exception message: {Message}", _caughtException.Message);
    }
}
