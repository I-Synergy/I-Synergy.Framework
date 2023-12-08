using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Tests.Data;
using ISynergy.Framework.Automations.Triggers;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ISynergy.Framework.Automations.Tests;

[TestClass()]
public class AutomationTests
{
    private readonly Customer _defaultCustomer;
    private readonly Automation _defaultAutomation;
    private readonly IAutomationService _automationService;

    /// <summary>
    /// Default constructor for the test.
    /// </summary>
    public AutomationTests()
    {
        _automationService = new AutomationService(
            new Mock<IAutomationManager>().Object,
            new Mock<IOptions<AutomationOptions>>().Object,
            new Mock<ILogger<AutomationService>>().Object);

        _defaultCustomer = new Customer()
        {
            Name = "Test1",
            Email = "admin@demo.com",
            Age = 18,
            Active = false
        };

        _defaultAutomation = new Automation
        {
            IsActive = true
        };
        _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => !string.IsNullOrEmpty(e.Name)));
        _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => !string.IsNullOrEmpty(e.Email)));
        _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => e.Age >= 18));
    }

    /// <summary>
    /// Scenario where new automation is default disabled.
    /// </summary>
    [TestMethod()]
    public void AutomationDefaultInActiveTest()
    {
        Automation defaultAutomation = new();
        Assert.IsFalse(defaultAutomation.IsActive);
    }

    /// <summary>
    /// Scenario where new automation is activated.
    /// </summary>
    [TestMethod()]
    public void AutomationActiveTest()
    {
        Automation defaultAutomation = new()
        {
            IsActive = true
        };

        Assert.IsTrue(defaultAutomation.IsActive);
    }

    /// <summary>
    /// Scenario where 2 customers are checked against the conditions after a delay of 1 second.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AutomationScenario1TestAsync()
    {
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(1)));

        Stopwatch stopwatch = new();

        stopwatch.Start();
        ActionResult result1 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
        stopwatch.Stop();

        // Assert should succeed because condition is met.
        Assert.IsTrue(result1.Succeeded);
        // Assert should succeed because condition is met and delay is applied.
        Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromSeconds(1));

        stopwatch.Reset();

        Customer customer2 = new()
        {
            Name = "Test2",
            Age = 16
        };

        stopwatch.Start();
        ActionResult result2 = await _automationService.ExecuteAsync(_defaultAutomation, customer2);
        stopwatch.Stop();

        // Assert should fail because condition is not met. Age is below 18.
        Assert.IsFalse(result2.Succeeded);
        // Assert should fail because no condition is met to run the action.
        Assert.IsFalse(stopwatch.Elapsed >= TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Scenario where defaultAutomation is run and executed before timing out.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AutomationScenario2TestAsync()
    {
        RelayCommand<Customer> command = new((e) =>
        {
            e.Age = 16;
            _defaultAutomation.IsActive = false;
        });

        RelayCommand<Customer> command2 = new((e) =>
        {
            e.Age += 1;
        });

        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(5)));
        _defaultAutomation.Actions.Add(new RepeatPreviousAction(_defaultAutomation.AutomationId, 1));
        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command2, _defaultCustomer));
        _defaultAutomation.Actions.Add(new RepeatPreviousAction<Customer>(_defaultAutomation.AutomationId, RepeatTypes.Until, (e) => e.Age >= 35, 10));

        Stopwatch stopwatch = new();

        stopwatch.Start();
        ActionResult result1 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
        stopwatch.Stop();

        // Assert should succeed because condition is met.
        Assert.IsTrue(result1.Succeeded);
        Assert.AreEqual(27, ((Customer)result1.Result).Age);
        // Assert should succeed because condition is met and delay is applied.
        Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromSeconds(5 * 2));

        stopwatch.Reset();

        stopwatch.Start();
        ActionResult result2 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
        stopwatch.Stop();

        // Assert should fail because condition is not met. Age is below 18.
        Assert.IsFalse(result2.Succeeded);
        // Assert should fail because no condition is met to run the action.
        Assert.IsFalse(stopwatch.Elapsed >= TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Scenario where defaultAutomation is run and executed before timing out.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AutomationScenario3TestAsync()
    {
        RelayCommand<Customer> command = new((e) =>
        {
            e.Age = 16;
        });

        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(3)));

        ActionResult result1 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
        Assert.IsTrue(result1.Succeeded);
        Assert.AreEqual(16, ((Customer)result1.Result).Age);

        ActionResult result2 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
        Assert.IsFalse(result2.Succeeded);
    }

    /// <summary>
    /// Scenario where defaultAutomation trigger is set and handled.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public Task AutomationScenario4TestAsync()
    {
        RelayCommand<Customer> command = new((e) =>
        {
            e.Age = 16;
        });

        _defaultAutomation.ExecutionTimeout = TimeSpan.FromSeconds(10);
        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, TimeSpan.FromSeconds(4)));

        IntegerTrigger trigger = new(
            _defaultAutomation.AutomationId,
            () => new(_defaultCustomer, (IProperty<int>)_defaultCustomer.Properties[nameof(Customer.Age)]),
            65,
            18,
            async (age) =>
            {
                ActionResult result = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer);
                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(21, age);
            });

        _defaultAutomation.Triggers.Add(trigger);
        _defaultCustomer.Age = 17;
        _defaultCustomer.Age = 21;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Scenario where defaultAutomation is cancelled by timeout.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AutomationScenario5TestAsync()
    {
        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(5)
        };

        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(10)));

        await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => _automationService.ExecuteAsync(automation, null));
    }

    /// <summary>
    /// Scenario where automation executes 5 delays.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task AutomationScenario6TestAsync()
    {
        Stopwatch stopwatch = new();
        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };

        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(2)));
        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(2)));
        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(2)));

        stopwatch.Start();
        ActionResult result = await _automationService.ExecuteAsync(automation, null);
        stopwatch.Stop();

        Assert.IsTrue(result.Succeeded);
        // Assert should succeed because condition is met and delay is applied.
        Assert.IsTrue(stopwatch.Elapsed >= TimeSpan.FromSeconds(3 * 2));
    }

    /// <summary>
    /// Scenario where boolean state trigger is executed.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public Task AutomationScenario7TestAsync()
    {
        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };

        BooleanStateTrigger stateTrigger = new(
            automation.AutomationId,
            () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Active)),
            false,
            true,
            async (active) =>
            {
                ActionResult result = await _automationService.ExecuteAsync(automation, _defaultCustomer);
                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(true, active);
            });

        automation.Triggers.Add(stateTrigger);
        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(7)));

        _defaultCustomer.Active = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Scenario where string state trigger is executed.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public Task AutomationScenario8TestAsync()
    {
        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = TimeSpan.FromSeconds(10)
        };

        string name = "Test";

        StringStateTrigger stateTrigger = new(
            automation.AutomationId,
            () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Name)),
            _defaultCustomer.Name,
            name,
            async (newName) =>
            {
                ActionResult result = await _automationService.ExecuteAsync(automation, _defaultCustomer);
                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(name, newName);
            });

        automation.Triggers.Add(stateTrigger);
        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(8)));

        _defaultCustomer.Name = name;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Scenario where boolean state trigger is not executed because they are the same.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public void AutomationScenario9Test()
    {
        Assert.ThrowsException<ArgumentException>(() =>
        {
            BooleanStateTrigger stateTrigger = new(
            new Automation().AutomationId,
            () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Active)),
            false,
            false,
            null);
        });
    }

    /// <summary>
    /// Scenario where event trigger is executed.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public Task AutomationScenario10TestAsync()
    {
        Automation automation = new();
        automation.IsActive = true;
        automation.ExecutionTimeout = TimeSpan.FromSeconds(15);

        EventTrigger<Customer> stateTrigger = new(
            automation.AutomationId,
            _defaultCustomer,
            (s) => _defaultCustomer.Registered += s,
            async (e) =>
            {
                ActionResult result = await _automationService.ExecuteAsync(automation, e);
                Assert.IsTrue(result.Succeeded);
            });

        automation.Triggers.Add(stateTrigger);
        automation.Actions.Add(new DelayAction(automation.AutomationId, TimeSpan.FromSeconds(10)));

        _defaultCustomer.Register();

        return Task.CompletedTask;
    }
}