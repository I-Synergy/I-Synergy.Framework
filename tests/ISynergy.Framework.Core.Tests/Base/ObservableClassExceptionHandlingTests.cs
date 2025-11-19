using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Base;
using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel;

namespace ISynergy.Framework.Core.Base.Tests;

/// <summary>
/// Tests for exception handling in PropertyChanged event invocations.
/// </summary>
[TestClass]
public class ObservableClassExceptionHandlingTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IExceptionHandlerService> _mockExceptionHandler;

    public ObservableClassExceptionHandlingTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();

        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        ServiceLocator.SetLocatorProvider(_mockServiceProvider.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public void RaisePropertyChanged_WithExceptionInHandler_CallsExceptionHandlerService()
    {
        // Arrange
        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");
        
        testClass.PropertyChanged += (s, e) => throw expectedException;

        // Act
        testClass.TestProperty = "NewValue";

        // Assert
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
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
        testClass.TestProperty = "NewValue";

        // Assert
        Assert.IsTrue(handler1Called, "First handler should be called");
        Assert.IsTrue(handler3Called, "Third handler should be called even if second throws");
        _mockExceptionHandler.Verify(x => x.HandleException(It.Is<Exception>(e => e == expectedException)), Times.Once);
    }

    [TestMethod]
    public void RaisePropertyChanged_WithException_WhenHandlerServiceNotAvailable_LogsToDebug()
    {
        // Arrange
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns((IExceptionHandlerService?)null);

        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");
        
        testClass.PropertyChanged += (s, e) => throw expectedException;

        // Act - should not crash
        testClass.TestProperty = "NewValue";

        // Assert - no exception should propagate
        Assert.AreEqual("NewValue", testClass.TestProperty);
    }

    [TestMethod]
    public void RaisePropertyChanged_WithException_WhenHandlerServiceThrows_DoesNotCrash()
    {
        // Arrange
        var handlerException = new InvalidOperationException("Handler service failed");
        _mockExceptionHandler
            .Setup(x => x.HandleException(It.IsAny<Exception>()))
            .Throws(handlerException);

        var testClass = new TestObservableClass();
        var expectedException = new InvalidOperationException("Handler exception");
        
        testClass.PropertyChanged += (s, e) => throw expectedException;

        // Act - should not crash even if handler service throws
        testClass.TestProperty = "NewValue";

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

