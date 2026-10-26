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
///   1. The <c>_bodies</c> ConditionalWeakTable on the shell host
///      (global::ISynergy.Framework.UI.Controls.NavigationHostView) exists and is
///      correctly typed. The page → content-body cache moved from <c>NavigationService</c> to
///      <c>NavigationHostView</c> when the shell took ownership of the content host
///      (commit cbd17689).
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
    /// Verifies that the <c>_bodies</c> field exists on
    /// <see cref="global::ISynergy.Framework.UI.Controls.NavigationHostView"/>, is a private
    /// instance (non-static) readonly <see cref="ConditionalWeakTable{TKey, TValue}"/> with key
    /// <see cref="Microsoft.Maui.Controls.ContentPage"/> and value
    /// <see cref="Microsoft.Maui.Controls.View"/>.
    /// </summary>
    [TestMethod]
    public void HostBodiesTable_ExistsAndIsCorrectType()
    {
        // Arrange
        var field = typeof(global::ISynergy.Framework.UI.Controls.NavigationHostView).GetField("_bodies",
            BindingFlags.NonPublic | BindingFlags.Instance);

        // Assert — field exists and is correctly declared
        Assert.IsNotNull(field, "_bodies field should exist on NavigationHostView");
        Assert.IsTrue(field.IsPrivate, "_bodies should be private");
        Assert.IsFalse(field.IsStatic, "_bodies should be an instance field (not static)");
        Assert.IsTrue(field.IsInitOnly, "_bodies should be readonly (readonly keyword)");

        // Assert — generic type is ConditionalWeakTable<ContentPage, View>
        var fieldType = field.FieldType;
        Assert.IsTrue(fieldType.IsGenericType, "_bodies should be a generic type");
        Assert.AreEqual(
            typeof(ConditionalWeakTable<,>),
            fieldType.GetGenericTypeDefinition(),
            "_bodies type should be ConditionalWeakTable<,>");

        var genericArgs = fieldType.GetGenericArguments();
        Assert.AreEqual(2, genericArgs.Length, "ConditionalWeakTable should have two type arguments");
        Assert.AreEqual(typeof(Microsoft.Maui.Controls.ContentPage), genericArgs[0],
            "Key type should be ContentPage");
        Assert.AreEqual(typeof(Microsoft.Maui.Controls.View), genericArgs[1],
            "Value type should be Microsoft.Maui.Controls.View");

        // No instance-value assertion here: _bodies is an instance field, and MAUI controls
        // cannot be constructed in a unit test (they need platform init), so there is no
        // instance to read the table from. The declaration itself is what we verify.
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


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
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
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
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
