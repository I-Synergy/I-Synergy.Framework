namespace ISynergy.Framework.Core.Base;

/// <summary>
/// Tests for exception handling in PropertyChanged event invocations.
/// </summary>
[TestClass]
public class ObservableClassExceptionHandlingTests
{
    public ObservableClassExceptionHandlingTests()
    {
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void RaisePropertyChanged_WithExceptionInHandler_CallsExceptionHandlerService()
    {
        // Arrange
        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");

        testClass.PropertyChanged += (s, e) => throw expectedException;

        // Act
        Assert.Throws<InvalidOperationException>(() => testClass.TestProperty = "NewValue");
    }

    [TestMethod]
    public void RaisePropertyChanged_WithMultipleHandlers_OneThrows_OtherHandlersStillCalled()
    {
        // Arrange
        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");
        bool handler1Called = false;
        bool handler3Called = false;

        testClass.PropertyChanged += (s, e) => handler1Called = true;
        testClass.PropertyChanged += (s, e) => throw expectedException;
        testClass.PropertyChanged += (s, e) => handler3Called = true;

        // Act
        Assert.Throws<InvalidOperationException>(() => testClass.TestProperty = "NewValue");

        // Assert
        Assert.IsTrue(handler1Called, "First handler should be called");
        Assert.IsTrue(handler3Called, "Third handler should be called even if second throws");
    }

    [TestMethod]
    public void RaisePropertyChanged_WithException_WhenHandlerServiceNotAvailable_LogsToDebug()
    {
        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");

        testClass.PropertyChanged += (s, e) => throw expectedException;

        // Act - should not crash
        Assert.Throws<InvalidOperationException>(() => testClass.TestProperty = "NewValue");

        // Assert - no exception should propagate
        Assert.AreEqual("NewValue", testClass.TestProperty);
    }

    [TestMethod]
    public void RaisePropertyChanged_WithException_WhenHandlerServiceThrows_DoesNotCrash()
    {
        // Arrange
        var handlerException = new InvalidOperationException("Handler service failed");
        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");

        testClass.PropertyChanged += (s, e) => throw expectedException;

        // Act - should not crash even if handler service throws
        Assert.Throws<InvalidOperationException>(() => testClass.TestProperty = "NewValue");

        // Assert
        Assert.AreEqual("NewValue", testClass.TestProperty);
    }

    private class TestObservableClass : ObservableClass
    {
        public string? TestProperty
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }
    }
}

