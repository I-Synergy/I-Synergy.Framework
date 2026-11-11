using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Actions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Services;
using ISynergy.Framework.Automations.Tests.Fixtures;
using Moq;

namespace ISynergy.Framework.Automations.Tests;

/// <summary>
/// Tests for the repeat bookkeeping / queue building extracted into <see cref="ActionQueueBuilder"/>.
/// </summary>
[TestClass]
public class ActionQueueBuilderTests
{
    private Mock<IActionExecutorFactory> _factory = null!;
    private Mock<IActionExecutor> _executor = null!;
    private Automation _automation = null!;
    private CancellationTokenSource _cancellationTokenSource = null!;
    private ActionQueueBuilder _builder = null!;
    private Customer _value = null!;

    [TestInitialize]
    public void Initialize()
    {
        _factory = new Mock<IActionExecutorFactory>();
        _executor = new Mock<IActionExecutor>();

        // Default: every action resolves to an executor that produces a completed task.
        _executor
            .Setup(e => e.CreateTaskAsync(
                It.IsAny<IAction>(),
                It.IsAny<object>(),
                It.IsAny<CancellationTokenSource>(),
                It.IsAny<IAutomationService>()))
            .Returns(() => () => Task.CompletedTask);

        _factory
            .Setup(f => f.GetExecutor(It.IsAny<IAction>()))
            .Returns(_executor.Object);

        _automation = new Automation();
        _value = new Customer { Name = "Test", Email = "test@demo.com", Age = 18 };
        _cancellationTokenSource = new CancellationTokenSource();
        _builder = new ActionQueueBuilder(_factory.Object, Mock.Of<IAutomationService>());
    }

    [TestCleanup]
    public void Cleanup()
    {
        _automation?.Dispose();
        _value?.Dispose();
        _cancellationTokenSource?.Dispose();
    }

    private DelayAction NewDelay() => new(_automation.AutomationId, TimeSpan.FromMilliseconds(1));

    private async Task<int> BuildAndCount()
    {
        var queue = await _builder.BuildQueueAsync(_automation, _value, _cancellationTokenSource);
        Assert.IsTrue(queue.IsAddingCompleted, "Queue must be marked complete after building.");
        return queue.Count;
    }

    [TestMethod]
    public async Task BuildQueueAsync_WithNoActions_ProducesEmptyQueue()
    {
        Assert.AreEqual(0, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_EnqueuesOneTaskPerAction()
    {
        _automation.Actions.Add(NewDelay());
        _automation.Actions.Add(NewDelay());
        _automation.Actions.Add(NewDelay());

        Assert.AreEqual(3, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_SkipsActionsWithoutExecutor()
    {
        _automation.Actions.Add(NewDelay());

        _factory
            .Setup(f => f.GetExecutor(It.IsAny<IAction>()))
            .Returns((object?)null);

        Assert.AreEqual(0, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_SkipsActionsWhoseExecutorReturnsNullTask()
    {
        _automation.Actions.Add(NewDelay());

        _executor
            .Setup(e => e.CreateTaskAsync(
                It.IsAny<IAction>(),
                It.IsAny<object>(),
                It.IsAny<CancellationTokenSource>(),
                It.IsAny<IAutomationService>()))
            .Returns((Func<Task>?)null);

        Assert.AreEqual(0, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_RepeatPreviousAction_RewindsOnce()
    {
        // action, RepeatPrevious(count=1) -> the action is queued twice.
        _automation.Actions.Add(NewDelay());
        _automation.Actions.Add(new RepeatPreviousAction(_automation.AutomationId, 1));

        Assert.AreEqual(2, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_RepeatPreviousAction_WithZeroCount_IsIgnored()
    {
        // A RepeatPreviousAction with Count == 0 is not treated as a repeat (Count > 0 required),
        // so it is handled as a regular action.
        _automation.Actions.Add(NewDelay());
        _automation.Actions.Add(new RepeatPreviousAction(_automation.AutomationId, 0));

        Assert.AreEqual(2, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_RepeatUntil_ConditionMet_DoesNotRepeat()
    {
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromMilliseconds(1)));
        _automation.Actions.Add(new RepeatPreviousAction<Customer>(
            _automation.AutomationId,
            RepeatTypes.Until,
            _ => true,
            countCircuitBreaker: 10));

        Assert.AreEqual(1, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_RepeatUntil_ConditionNotMet_RepeatsThenStopsAtCircuitBreaker()
    {
        // Until with a never-true condition: repeat until the circuit breaker (2) is hit,
        // meaning the previous action is queued 1 (initial) + 2 (rewinds) = 3 times.
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromMilliseconds(1)));
        _automation.Actions.Add(new RepeatPreviousAction<Customer>(
            _automation.AutomationId,
            RepeatTypes.Until,
            _ => false,
            countCircuitBreaker: 2));

        Assert.AreEqual(3, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_RepeatWhile_ConditionFalse_DoesNotRepeat()
    {
        // While with a false condition stops immediately: previous action queued once.
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromMilliseconds(1)));
        _automation.Actions.Add(new RepeatPreviousAction<Customer>(
            _automation.AutomationId,
            RepeatTypes.While,
            _ => false,
            countCircuitBreaker: 10));

        Assert.AreEqual(1, await BuildAndCount());
    }

    [TestMethod]
    public async Task BuildQueueAsync_RepeatWhile_ConditionTrue_RepeatsUntilCircuitBreaker()
    {
        // While with a always-true condition repeats until the circuit breaker (1) is hit:
        // previous action queued 1 (initial) + 1 (rewind) = 2 times.
        _automation.Actions.Add(new DelayAction(_automation.AutomationId, TimeSpan.FromMilliseconds(1)));
        _automation.Actions.Add(new RepeatPreviousAction<Customer>(
            _automation.AutomationId,
            RepeatTypes.While,
            _ => true,
            countCircuitBreaker: 1));

        Assert.AreEqual(2, await BuildAndCount());
    }
}
