using ISynergy.Framework.Core.Tests.Fixtures;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Base.Tests;

[TestClass]
public class ObservableValidatedClassEventTests
{
    [TestMethod]
    public void PropertyChanged_ShouldFireWhenPropertyChanges()
    {
        // Arrange
        var observable = new TestObservableClass();
        var eventFired = false;
        var propertyNameFromEvent = string.Empty;

        observable.PropertyChanged += (s, e) =>
        {
            eventFired = true;
            propertyNameFromEvent = e.PropertyName;
        };

        // Act
        observable.TestProperty = "New Value";

        // Assert
        Assert.IsTrue(eventFired);
        Assert.AreEqual(nameof(TestObservableClass.TestProperty), propertyNameFromEvent);
    }

    [TestMethod]
    public void PropertyChanged_ShouldNotFireAfterDisposal()
    {
        // Arrange
        var observable = new TestObservableClass();
        var eventFired = false;
        observable.PropertyChanged += (s, e) => eventFired = true;

        // Act
        observable.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => observable.TestProperty = "New Value");
        Assert.IsFalse(eventFired);
    }

    [TestMethod]
    public void ErrorsChanged_ShouldFireWhenErrorsChange()
    {
        // Arrange
        var observable = new TestObservableClass();
        var eventFired = false;
        var propertyNameFromEvent = string.Empty;

        observable.ErrorsChanged += (s, e) =>
        {
            eventFired = true;
            propertyNameFromEvent = e.PropertyName;
        };

        // Act
        observable.AddError("TestProperty");

        // Assert
        Assert.IsTrue(eventFired);
        Assert.AreEqual(nameof(observable.Errors), propertyNameFromEvent);
    }

    [TestMethod]
    public void ErrorsChanged_ShouldNotFireAfterDisposal()
    {
        // Arrange
        var observable = new TestObservableClass();
        var eventFired = false;
        observable.ErrorsChanged += (s, e) => eventFired = true;

        // Act
        observable.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => observable.AddError("TestProperty"));
        Assert.IsFalse(eventFired);
    }

    [TestMethod]
    public void MultipleEventHandlers_ShouldAllBeUnsubscribed()
    {
        // Arrange
        var observable = new TestObservableClass();
        var handler1Called = false;
        var handler2Called = false;
        var handler3Called = false;

        PropertyChangedEventHandler handler1 = (s, e) => handler1Called = true;
        PropertyChangedEventHandler handler2 = (s, e) => handler2Called = true;
        PropertyChangedEventHandler handler3 = (s, e) => handler3Called = true;

        observable.PropertyChanged += handler1;
        observable.PropertyChanged += handler2;
        observable.PropertyChanged += handler3;

        // Act
        observable.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => observable.TriggerPropertyChanged("TestProperty"));
        Assert.IsFalse(handler1Called);
        Assert.IsFalse(handler2Called);
        Assert.IsFalse(handler3Called);
    }

    [TestMethod]
    public async Task AsyncDisposal_ShouldUnsubscribeAllEvents()
    {
        // Arrange
        var observable = new TestObservableClass();
        var propertyChangedCalled = false;
        var errorsChangedCalled = false;

        observable.PropertyChanged += (s, e) => propertyChangedCalled = true;
        observable.ErrorsChanged += (s, e) => errorsChangedCalled = true;

        // Act
        await observable.DisposeAsync();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => observable.TriggerPropertyChanged("TestProperty"));
        Assert.Throws<ObjectDisposedException>(() => observable.TriggerErrorsChanged("TestProperty"));
        Assert.IsFalse(propertyChangedCalled);
        Assert.IsFalse(errorsChangedCalled);
    }

    [TestMethod]
    public void SubscribeAfterDisposal_ShouldThrowObjectDisposedException()
    {
        // Arrange
        var observable = new TestObservableClass();
        observable.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() =>
            observable.PropertyChanged += (s, e) => { });
        Assert.Throws<ObjectDisposedException>(() =>
            observable.ErrorsChanged += (s, e) => { });
    }

    [TestMethod]
    public void UnsubscribeAfterDisposal_ShouldNotThrow()
    {
        // Arrange
        var observable = new TestObservableClass();
        PropertyChangedEventHandler handler = (s, e) => { };
        EventHandler<DataErrorsChangedEventArgs> errorHandler = (s, e) => { };

        observable.PropertyChanged += handler;
        observable.ErrorsChanged += errorHandler;

        // Act
        observable.Dispose();

        // Assert - Should not throw
        observable.PropertyChanged -= handler;
        observable.ErrorsChanged -= errorHandler;
    }

    [TestMethod]
    public void DisposalOrder_ShouldHandleEventsDuringDisposal()
    {
        // Arrange
        var observable = new TestObservableClass();
        var disposalOrder = new List<string>();

        observable.PropertyChanged += (s, e) => disposalOrder.Add("PropertyChanged");
        observable.ErrorsChanged += (s, e) => disposalOrder.Add("ErrorsChanged");

        // Act
        observable.Dispose();

        // Verify events are cleaned up
        Assert.Throws<ObjectDisposedException>(() => observable.TriggerPropertyChanged("TestProperty"));
        Assert.Throws<ObjectDisposedException>(() => observable.TriggerErrorsChanged("TestProperty"));

        // No events should have been added to disposalOrder during disposal
        Assert.AreEqual(0, disposalOrder.Count);
    }
}
