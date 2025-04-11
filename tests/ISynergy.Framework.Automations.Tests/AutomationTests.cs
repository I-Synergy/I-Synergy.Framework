using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Conditions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Options;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Tests.Fixtures;
using ISynergy.Framework.Automations.Triggers;
using ISynergy.Framework.Core.Abstractions.Base;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics;

namespace ISynergy.Framework.Automations.Tests;

[TestClass()]
public class AutomationTests
{
    private Customer _defaultCustomer;
    private Automation _defaultAutomation;
    private IAutomationService? _automationService;
    private Stopwatch _stopwatch = new Stopwatch();
    private readonly Mock<ILoggerFactory> _mockLoggerFactory = new Mock<ILoggerFactory>();

    // Configuration for environment-aware testing
    private static readonly bool IsRunningInCI = Environment.GetEnvironmentVariable("CI") is not null;
    private static readonly double TimingTolerance = 0.2; // 20% tolerance

    private bool IsWithinTolerance(TimeSpan expected, TimeSpan actual)
    {
        var lowerBound = expected.TotalMilliseconds * (1 - TimingTolerance);
        var upperBound = expected.TotalMilliseconds * (1 + TimingTolerance);
        return actual.TotalMilliseconds >= lowerBound && actual.TotalMilliseconds <= upperBound;
    }

    private TimeSpan GetDelay(int seconds) =>
        IsRunningInCI ? TimeSpan.FromSeconds(seconds / 2.0) : TimeSpan.FromSeconds(seconds);

    public AutomationTests()
    {
        _defaultCustomer = new Customer()
        {
            Name = "Test1",
            Email = "admin@demo.com",
            Age = 18,
            Active = false
        };

        _mockLoggerFactory
           .Setup(x => x.CreateLogger(It.IsAny<string>()))
           .Returns(new Mock<ILogger>().Object);

        _automationService = new AutomationService(
            new Mock<IAutomationManager>().Object,
            new Mock<IOptions<AutomationOptions>>().Object,
            _mockLoggerFactory.Object);

        _defaultAutomation = new Automation { IsActive = true };
    }

    [TestInitialize]
    public void InitializeTest()
    {
        _defaultAutomation.Conditions.Clear();
        _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => !string.IsNullOrEmpty(e.Name)));
        _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => !string.IsNullOrEmpty(e.Email)));
        _defaultAutomation.Conditions.Add(new Condition<Customer>(_defaultAutomation.AutomationId, (e) => e.Age >= 18));
    }

    [TestCleanup]
    public void CleanTest()
    {
        _defaultAutomation?.Dispose();
        _defaultCustomer?.Dispose();
        _automationService = null;
    }

    [TestMethod()]
    public void AutomationDefaultInActiveTest()
    {
        Automation defaultAutomation = new();
        Assert.IsFalse(defaultAutomation.IsActive);
    }

    [TestMethod()]
    public void AutomationActiveTest()
    {
        Automation defaultAutomation = new() { IsActive = true };
        Assert.IsTrue(defaultAutomation.IsActive);
    }

    [TestMethod]
    public async Task AutomationScenario1TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(1);

        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, expectedDelay));

        _stopwatch.Start();
        ActionResult result1 = await _automationService!.ExecuteAsync(_defaultAutomation, _defaultCustomer, cancellationTokenSource);
        _stopwatch.Stop();

        Assert.IsTrue(result1.Succeeded);
        Assert.IsTrue(IsWithinTolerance(expectedDelay, _stopwatch.Elapsed));

        _stopwatch.Reset();

        Customer customer2 = new() { Name = "Test2", Age = 16 };

        _stopwatch.Start();
        ActionResult result2 = await _automationService!.ExecuteAsync(_defaultAutomation, customer2, cancellationTokenSource);
        _stopwatch.Stop();

        Assert.IsFalse(result2.Succeeded);
        Assert.IsFalse(_stopwatch.Elapsed >= expectedDelay);

        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public async Task AutomationScenario2TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(5);

        RelayCommand<Customer> command = new((e) =>
        {
            e!.Age = 16;
            _defaultAutomation.IsActive = false;
        });

        RelayCommand<Customer> command2 = new((e) => e!.Age += 1);

        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, expectedDelay));
        _defaultAutomation.Actions.Add(new RepeatPreviousAction(_defaultAutomation.AutomationId, 1));
        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command2, _defaultCustomer));
        _defaultAutomation.Actions.Add(new RepeatPreviousAction<Customer>(_defaultAutomation.AutomationId, RepeatTypes.Until, (e) => e.Age >= 35, 10));

        _stopwatch.Start();
        ActionResult result1 = await _automationService!.ExecuteAsync(_defaultAutomation, _defaultCustomer, cancellationTokenSource);
        _stopwatch.Stop();

        Assert.IsTrue(result1.Succeeded);
        Assert.AreEqual(27, ((Customer)result1.Result!).Age);
        Assert.IsTrue(IsWithinTolerance(expectedDelay * 2, _stopwatch.Elapsed));

        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public async Task AutomationScenario3TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(3);

        RelayCommand<Customer> command = new((e) => e!.Age = 16);

        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, expectedDelay));

        ActionResult result1 = await _automationService!.ExecuteAsync(_defaultAutomation, _defaultCustomer, cancellationTokenSource);
        Assert.IsTrue(result1.Succeeded);
        Assert.AreEqual(16, ((Customer)result1.Result!).Age);

        ActionResult result2 = await _automationService.ExecuteAsync(_defaultAutomation, _defaultCustomer, cancellationTokenSource);
        Assert.IsFalse(result2.Succeeded);

        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public Task AutomationScenario4TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(4);

        RelayCommand<Customer> command = new((e) => e!.Age = 16);

        _defaultAutomation.ExecutionTimeout = GetDelay(10);
        _defaultAutomation.Actions.Add(new CommandAction(_defaultAutomation.AutomationId, command, _defaultCustomer));
        _defaultAutomation.Actions.Add(new DelayAction(_defaultAutomation.AutomationId, expectedDelay));

        IntegerTrigger trigger = new(
            _defaultAutomation.AutomationId,
            () => new(_defaultCustomer, (IProperty<int>)_defaultCustomer.Properties[nameof(Customer.Age)]),
            65,
            18,
            async (age) =>
            {
                ActionResult result = await _automationService!.ExecuteAsync(_defaultAutomation, _defaultCustomer, cancellationTokenSource);
                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(21, age);
            });

        _defaultAutomation.Triggers.Add(trigger);
        _defaultCustomer.Age = 17;
        _defaultCustomer.Age = 21;

        cancellationTokenSource.Dispose();
        return Task.CompletedTask;
    }

    [TestMethod]
    public async Task AutomationScenario5TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var executionTimeout = GetDelay(5);
        var actionDelay = GetDelay(10);

        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = executionTimeout
        };

        automation.Actions.Add(new DelayAction(automation.AutomationId, actionDelay));

        _stopwatch.Start();
        await Assert.ThrowsExceptionAsync<TaskCanceledException>(() =>
            _automationService!.ExecuteAsync(automation, default!, cancellationTokenSource));
        _stopwatch.Stop();

        Assert.IsTrue(IsWithinTolerance(executionTimeout, _stopwatch.Elapsed));
        Assert.IsTrue(_stopwatch.Elapsed < actionDelay);

        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public async Task AutomationScenario6TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(2);

        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = GetDelay(10)
        };

        automation.Actions.Add(new DelayAction(automation.AutomationId, expectedDelay));
        automation.Actions.Add(new DelayAction(automation.AutomationId, expectedDelay));
        automation.Actions.Add(new DelayAction(automation.AutomationId, expectedDelay));

        _stopwatch.Start();
        ActionResult result = await _automationService!.ExecuteAsync(automation, default!, cancellationTokenSource);
        _stopwatch.Stop();

        Assert.IsTrue(result.Succeeded);
        Assert.IsTrue(IsWithinTolerance(expectedDelay * 3, _stopwatch.Elapsed));

        cancellationTokenSource.Dispose();
    }

    [TestMethod]
    public Task AutomationScenario7TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(7);

        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = GetDelay(10)
        };

        BooleanStateTrigger stateTrigger = new(
            automation.AutomationId,
            () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Active)!),
            false,
            true,
            async (active) =>
            {
                ActionResult result = await _automationService!.ExecuteAsync(automation, _defaultCustomer, cancellationTokenSource);
                Assert.IsTrue(result.Succeeded);
                Assert.IsTrue(active);
            });

        automation.Triggers.Add(stateTrigger);
        automation.Actions.Add(new DelayAction(automation.AutomationId, expectedDelay));

        _defaultCustomer.Active = true;

        cancellationTokenSource.Dispose();
        return Task.CompletedTask;
    }

    [TestMethod]
    public Task AutomationScenario8TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var expectedDelay = GetDelay(8);

        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = GetDelay(10)
        };

        string name = "Test";

        StringStateTrigger stateTrigger = new(
            automation.AutomationId,
            () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Name)!),
            _defaultCustomer.Name,
            name,
            async (newName) =>
            {
                ActionResult result = await _automationService!.ExecuteAsync(automation, _defaultCustomer, cancellationTokenSource);
                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(name, newName);
            });

        automation.Triggers.Add(stateTrigger);
        automation.Actions.Add(new DelayAction(automation.AutomationId, expectedDelay));

        _defaultCustomer.Name = name;

        cancellationTokenSource.Dispose();
        return Task.CompletedTask;
    }

    [TestMethod]
    public void AutomationScenario9Test()
    {
        Assert.ThrowsException<ArgumentException>(() =>
        {
            BooleanStateTrigger stateTrigger = new(
                new Automation().AutomationId,
                () => new(_defaultCustomer, _defaultCustomer.GetProperty(x => x.Active)!),
                false,
                false,
                default!);
        });
    }

    [TestMethod]
    public Task AutomationScenario10TestAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        Automation automation = new()
        {
            IsActive = true,
            ExecutionTimeout = GetDelay(15)
        };

        EventTrigger<Customer> stateTrigger = new(
            automation.AutomationId,
            _defaultCustomer,
            (s) => _defaultCustomer.Registered += s,
            async (e) =>
            {
                ActionResult result = await _automationService!.ExecuteAsync(automation, e, cancellationTokenSource);
                Assert.IsTrue(result.Succeeded);
            });

        automation.Triggers.Add(stateTrigger);
        automation.Actions.Add(new DelayAction(automation.AutomationId, GetDelay(10)));

        _defaultCustomer.Register();

        cancellationTokenSource.Dispose();
        return Task.CompletedTask;
    }
}
