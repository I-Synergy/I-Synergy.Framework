using ISynergy.Framework.Core.Tests.Fixtures;

namespace ISynergy.Framework.Core.Base.Tests;

[TestClass]
public class ObservableValidatedClassTests
{
    [TestMethod]
    public void Dispose_ShouldMarkObjectAsDisposed()
    {
        // Arrange
        var observable = new TestObservableClass();

        // Act
        observable.Dispose();

        // Assert
        Assert.IsTrue(observable.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => observable.EnsureNotDisposed());
    }

    [TestMethod]
    public void Dispose_ShouldClearProperties()
    {
        // Arrange
        var observable = new TestObservableClass();
        observable.TestProperty = "test";

        // Act
        observable.Dispose();

        // Assert
        Assert.AreEqual(0, observable.Properties.Count);
    }

    [TestMethod]
    public void Dispose_ShouldClearErrors()
    {
        // Arrange
        var observable = new TestObservableClass();
        observable.AddValidationError("TestProperty", "Error");

        // Act
        observable.Dispose();

        // Assert
        Assert.AreEqual(0, observable.Errors.Count);
    }

    [TestMethod]
    public void Dispose_MultipleCallsShouldBeHandledGracefully()
    {
        // Arrange
        var observable = new TestObservableClass();

        // Act & Assert - should not throw
        observable.Dispose();
        observable.Dispose();
    }

    [TestMethod]
    public async Task DisposeAsync_ShouldMarkObjectAsDisposed()
    {
        // Arrange
        var observable = new TestObservableClass();

        // Act
        await observable.DisposeAsync();

        // Assert
        Assert.IsTrue(observable.IsAsyncDisposed);
        Assert.Throws<ObjectDisposedException>(() => observable.EnsureNotDisposed());
    }

    [TestMethod]
    public async Task DisposeAsync_ShouldHandleAsyncDisposableProperties()
    {
        // Arrange
        var observable = new TestObservableClass();
        var asyncProperty = new AsyncDisposableProperty("TestProperty");
        observable.Properties.Add("TestProperty", asyncProperty);

        // Act
        await observable.DisposeAsync();

        // Assert
        Assert.IsTrue(asyncProperty.IsAsyncDisposed);
        Assert.AreEqual(0, observable.Properties.Count);
    }

    [TestMethod]
    public async Task DisposeAsync_MultipleCallsShouldBeHandledGracefully()
    {
        // Arrange
        var observable = new TestObservableClass();

        // Act & Assert - should not throw
        await observable.DisposeAsync();
        await observable.DisposeAsync();
    }

    [TestMethod]
    public void PropertyAccess_ShouldThrowAfterDisposal()
    {
        // Arrange
        var observable = new TestObservableClass();
        observable.TestProperty = "test";

        // Act
        observable.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => observable.TestProperty = "new value");
    }

    [TestMethod]
    public void Events_ShouldBeUnsubscribedAfterDisposal()
    {
        // Arrange
        var observable = new TestObservableClass();
        var propertyChangedFired = false;
        observable.PropertyChanged += (s, e) => propertyChangedFired = true;

        // Act
        observable.Dispose();

        // Try to trigger property changed after disposal
        try
        {
            observable.TestProperty = "new value";
        }
        catch (ObjectDisposedException)
        {
            // Expected
        }

        // Assert
        Assert.IsFalse(propertyChangedFired);
    }

    [TestMethod]
    public async Task MixedDisposal_ShouldHandleBothSyncAndAsyncDisposal()
    {
        // Arrange
        var observable = new TestObservableClass();
        var asyncProperty = new AsyncDisposableProperty("TestProperty");
        observable.Properties.Add("TestProperty", asyncProperty);

        // Act - Test both disposal methods
        observable.Dispose();
        await observable.DisposeAsync();

        // Assert
        Assert.IsTrue(observable.IsDisposed);
        Assert.IsTrue(observable.IsAsyncDisposed);
        Assert.AreEqual(0, observable.Properties.Count);
    }
}
