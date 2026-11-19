using ISynergy.Framework.UI.Services;
using System.Reflection;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Tests for the two helpers extracted out of <see cref="NavigationService"/>'s root-replacement
/// paths. A <see cref="Page"/> cannot be constructed in a unit test (its static constructor needs
/// MAUI platform initialization), so the platform-bound parts stay in the thin overload and the
/// decision-making is exercised here.
/// </summary>
/// <remarks>
/// The helpers are internal, not public: they exist to be testable, not to be API. The test
/// project reaches them by reflection, the same way the other navigation tests reach their
/// private helpers.
/// </remarks>
[TestClass]
[OSCondition(OperatingSystems.Windows)]
public class NavigationServiceRootReplacementTests
{
    private static readonly Type ServiceType = typeof(NavigationService);

    private static readonly MethodInfo Decide =
        ServiceType.GetMethod("DecideRootReplacement", BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new InvalidOperationException("DecideRootReplacement should exist");

    private static readonly Type ReplacementType =
        ServiceType.GetNestedType("RootReplacement", BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("RootReplacement should exist");

    private static readonly object Unchanged = Enum.Parse(ReplacementType, "Unchanged");
    private static readonly object Replace = Enum.Parse(ReplacementType, "Replace");

    private static object DecideRoot(object? currentRoot, object incoming) =>
        Decide.Invoke(null, [currentRoot, incoming])!;

    private static MethodInfo Reapply(int parameterCount) =>
        ServiceType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(method => method.Name == "ReapplyWindowContentAsync"
                              && method.GetParameters().Length == parameterCount);

    private static Task ReapplyWindowContentAsync(bool hasHandler, Action<Action> dispatch, Action update) =>
        (Task)Reapply(3).Invoke(null, [hasHandler, dispatch, update])!;

    // -----------------------------------------------------------------------
    // DecideRootReplacement
    // -----------------------------------------------------------------------

    [TestMethod]
    public void DecideRootReplacement_WhenPageIsAlreadyTheRoot_ReportsUnchanged()
    {
        var page = new object();

        Assert.AreEqual(Unchanged, DecideRoot(page, page));
    }

    [TestMethod]
    public void DecideRootReplacement_WhenPageDiffersFromTheRoot_ReportsReplace()
    {
        Assert.AreEqual(Replace, DecideRoot(new object(), new object()));
    }

    [TestMethod]
    public void DecideRootReplacement_WhenThereIsNoCurrentRoot_ReportsReplace()
    {
        Assert.AreEqual(Replace, DecideRoot(null, new object()));
    }

    /// <summary>
    /// Equal-but-distinct instances must not be treated as the same root: the guard exists to skip
    /// re-installing the very same page, not an equivalent one.
    /// </summary>
    [TestMethod]
    public void DecideRootReplacement_WhenInstancesAreEqualButDistinct_ReportsReplace()
    {
        Assert.AreEqual(Replace, DecideRoot("root", new string("root")));
    }

    // -----------------------------------------------------------------------
    // ReapplyWindowContentAsync
    // -----------------------------------------------------------------------

    [TestMethod]
    public async Task ReapplyWindowContentAsync_WithoutHandler_CompletesWithoutDispatching()
    {
        var dispatched = false;
        var updated = false;

        await ReapplyWindowContentAsync(
            hasHandler: false,
            _ => dispatched = true,
            () => updated = true);

        Assert.IsFalse(dispatched, "an unattached window needs no dispatcher turn");
        Assert.IsFalse(updated, "an unattached window has no content mapping to re-apply");
    }

    [TestMethod]
    public async Task ReapplyWindowContentAsync_WithHandler_DefersTheUpdateToTheDispatcher()
    {
        Action? deferred = null;
        var updated = false;

        var task = ReapplyWindowContentAsync(
            hasHandler: true,
            action => deferred = action,
            () => updated = true);

        Assert.IsNotNull(deferred, "the update must be scheduled, not run inline");
        Assert.IsFalse(updated, "the update must not run before the dispatcher turn");
        Assert.IsFalse(task.IsCompleted, "the task must not complete before the update runs");

        deferred();

        await task;
        Assert.IsTrue(updated);
    }

    [TestMethod]
    public async Task ReapplyWindowContentAsync_WhenTheUpdateThrows_StillCompletes()
    {
        Action? deferred = null;
        var task = ReapplyWindowContentAsync(
            hasHandler: true,
            action => deferred = action,
            () => throw new InvalidOperationException("handler detached mid-flight"));

        Assert.IsNotNull(deferred);

        // The exception surfaces from the dispatcher callback, which is where MAUI would observe
        // it; what matters here is that the awaiting caller is never left hanging.
        Assert.ThrowsExactly<InvalidOperationException>(deferred);

        await task;
        Assert.IsTrue(task.IsCompletedSuccessfully, "completion must be signalled even when the update throws");
    }

    [TestMethod]
    public void ReapplyWindowContentAsync_HasExpectedSignatures()
    {
        var testable = Reapply(3);

        Assert.AreEqual(typeof(Task), testable.ReturnType);
        Assert.AreEqual(typeof(bool), testable.GetParameters()[0].ParameterType);
        Assert.AreEqual(typeof(Action<Action>), testable.GetParameters()[1].ParameterType);
        Assert.AreEqual(typeof(Action), testable.GetParameters()[2].ParameterType);
    }
}
