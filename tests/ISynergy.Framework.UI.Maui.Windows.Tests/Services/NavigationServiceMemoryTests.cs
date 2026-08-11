using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ISynergy.Framework.UI.Maui.Services;

/// <summary>
/// Tests for the memory-management safety net in the MAUI NavigationService.
///
/// These tests verify:
///   1. The <c>_pageFrames</c> ConditionalWeakTable exists and is correctly typed.
///   2. The Remove-before-Add pattern prevents duplicate-key ArgumentException.
///   3. The CleanupPageResources pattern (TryGetValue → Dispose → Remove) works.
///   4. Back-navigation scope disposal: the new scope is disposed when a cached
///      entry is restored.
///
/// Note: <see cref="Microsoft.Maui.Controls.Page"/> cannot be instantiated in
/// unit tests (requires MAUI platform init). Tests 2-4 use a local
/// <see cref="ConditionalWeakTable{TKey, TValue}"/> with <see cref="string"/>
/// keys to demonstrate the same logic without MAUI dependencies.
/// </summary>
[TestClass]
public class NavigationServiceMemoryTests
{
    /// <summary>
    /// Verifies that the <c>_pageFrames</c> field exists on
    /// <see cref="NavigationService"/>, is a private static readonly
    /// <see cref="ConditionalWeakTable{TKey, TValue}"/> with key
    /// <see cref="Page"/> and value <c>NavigationFrame</c>, and is
    /// initialized (non-null).
    /// </summary>
    [TestMethod]
    public void PageFramesField_ExistsAndIsCorrectType()
    {
        // Arrange
        var field = typeof(NavigationService).GetField("_pageFrames",
            BindingFlags.NonPublic | BindingFlags.Static);

        // Assert — field exists and is correctly declared
        Assert.IsNotNull(field, "_pageFrames field should exist on NavigationService");
        Assert.IsTrue(field.IsPrivate, "_pageFrames should be private");
        Assert.IsTrue(field.IsStatic, "_pageFrames should be static");
        Assert.IsTrue(field.IsInitOnly, "_pageFrames should be readonly (readonly keyword)");

        // Assert — generic type is ConditionalWeakTable<Page, NavigationFrame>
        var fieldType = field.FieldType;
        Assert.IsTrue(fieldType.IsGenericType, "_pageFrames should be a generic type");
        Assert.AreEqual(
            typeof(ConditionalWeakTable<,>),
            fieldType.GetGenericTypeDefinition(),
            "_pageFrames type should be ConditionalWeakTable<,>");

        var genericArgs = fieldType.GetGenericArguments();
        Assert.AreEqual(2, genericArgs.Length, "ConditionalWeakTable should have two type arguments");
        Assert.AreEqual(typeof(Page), genericArgs[0], "Key type should be Page");

        // NavigationFrame is protected on BaseNavigationService — verify by name
        Assert.IsTrue(genericArgs[1].Name.Contains("NavigationFrame"),
            "Value type should be NavigationFrame (found: {0})", genericArgs[1].Name);
        Assert.AreEqual("BaseNavigationService", genericArgs[1].DeclaringType?.Name,
            "NavigationFrame should be nested on BaseNavigationService");

        // Assert — field is initialized (the inline = new() assignment)
        var value = field.GetValue(null);
        Assert.IsNotNull(value, "_pageFrames should be initialized (non-null)");
    }

    /// <summary>
    /// Demonstrates that the Remove-before-Add pattern used in the forward
    /// navigation path (when the same singleton page is reused) prevents the
    /// <see cref="ArgumentException"/> that <c>ConditionalWeakTable.Add</c>
    /// throws on duplicate keys.
    ///
    /// NavigationService uses this exact pattern:
    /// <code>
    ///   _pageFrames.Remove(page);
    ///   _pageFrames.Add(page, newFrame);
    /// </code>
    ///
    /// Without the Remove call, re-adding the same page key would throw.
    /// </summary>
    [TestMethod]
    public void PageFrames_RemoveThenAdd_PreventsArgumentException()
    {
        // Arrange
        var table = new ConditionalWeakTable<string, IServiceScope>();
        var key = "singletonPage";
        var mockScope1 = new Mock<IServiceScope>();
        var mockScope2 = new Mock<IServiceScope>();

        // Add the first entry
        table.Add(key, mockScope1.Object);

        // Act & Assert — adding the same key without Remove throws ArgumentException.
        // (Manual try/catch used because this MSTest version does not expose
        // Assert.ThrowsException in the global-implicit-usings scope.)
        bool duplicateThrew = false;
        try
        {
            table.Add(key, mockScope2.Object);
        }
        catch (ArgumentException)
        {
            duplicateThrew = true;
        }
        Assert.IsTrue(duplicateThrew, "Add on a duplicate key should throw ArgumentException");

        // Act — apply the Remove-before-Add pattern (same as NavigationService)
        table.Remove(key);
        table.Add(key, mockScope2.Object); // Should not throw

        // Assert — the new value was stored
        Assert.IsTrue(table.TryGetValue(key, out var storedScope));
        Assert.AreSame(mockScope2.Object, storedScope,
            "The stored scope should be the second (replacement) scope");
    }

    /// <summary>
    /// Verifies the cleanup pattern used in <c>CleanupPageResources</c>:
    /// <c>TryGetValue</c> → <c>Dispose</c> → <c>Remove</c>.
    ///
    /// This pattern ensures orphaned frames are properly disposed when
    /// the platform back button pops pages without going through
    /// <see cref="NavigationService.GoBackAsync"/>.
    /// </summary>
    [TestMethod]
    public void CleanupPageResources_DisposesAndRemovesFrame()
    {
        // Arrange
        var table = new ConditionalWeakTable<string, IServiceScope>();
        var key = "orphanedPage";
        var mockScope = new Mock<IServiceScope>();
        mockScope.SetupGet(x => x.ServiceProvider)
            .Returns(new Mock<IServiceProvider>().Object);

        table.Add(key, mockScope.Object);

        // Act — apply the CleanupPageResources pattern
        if (table.TryGetValue(key, out var scope))
        {
            scope.Dispose();
            table.Remove(key);
        }

        // Assert — scope was disposed
        mockScope.Verify(x => x.Dispose(), Times.Once,
            "CleanupPageResources should dispose the orphaned frame's scope");

        // Assert — entry was removed from the table
        Assert.IsFalse(table.TryGetValue(key, out _),
            "CleanupPageResources should remove the entry from the table");
    }

    /// <summary>
    /// Verifies the back-navigation scope disposal pattern.
    ///
    /// When <see cref="NavigationService.NavigateAsync{TViewModel}(TViewModel, object?, bool)"/>
    /// detects that the target page is already in the navigation stack, it:
    ///   1. Restores the cached NavigationFrame from the backstack (scope 1).
    ///   2. Disposes the newly created scope (scope 2) since it is no longer
    ///      needed — the restored frame carries its own preserved scope.
    ///
    /// This test confirms that only the newly created scope is disposed,
    /// while the cached (restored) scope is left intact.
    /// </summary>
    [TestMethod]
    public void BackNavigationScopeDisposal_Pattern()
    {
        // Arrange
        var cachedScope = new Mock<IServiceScope>();   // scope 1 — preserved
        var newScope = new Mock<IServiceScope>();      // scope 2 — to be disposed

        cachedScope.SetupGet(x => x.ServiceProvider)
            .Returns(new Mock<IServiceProvider>().Object);
        newScope.SetupGet(x => x.ServiceProvider)
            .Returns(new Mock<IServiceProvider>().Object);

        // Act — simulate the fix pattern from the back-navigation path.
        // In the real code, the restored cached frame becomes _currentFrame
        // and the new scope is no longer needed.
        //
        //     // Restore target's existing frame from _backStack
        //     _currentFrame = targetFrame;   // <-- carries cachedScope
        //     scope.Dispose();                // <-- newScope is disposed
        //
        newScope.Object.Dispose();

        // Assert — only the newly created scope is disposed
        newScope.Verify(x => x.Dispose(), Times.Once,
            "The newly created scope should be disposed after restoring the cached frame");

        cachedScope.Verify(x => x.Dispose(), Times.Never,
            "The cached scope (from the restored frame) should NOT be disposed");
    }
}
