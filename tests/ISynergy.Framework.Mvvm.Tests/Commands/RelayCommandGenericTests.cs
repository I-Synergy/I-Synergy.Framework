﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Locators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class RelayCommandGenericTests
{
    private Mock<IExceptionHandlerService> _mockExceptionHandler;
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    public RelayCommandGenericTests()
    {
        // Setup mocks
        _mockExceptionHandler = new Mock<IExceptionHandlerService>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceScope = new Mock<IServiceScope>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();

        // Setup service scope factory
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_mockServiceScopeFactory.Object);

        // Setup service scope
        _mockServiceScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(_mockServiceScope.Object);

        _mockServiceScope
            .Setup(x => x.ServiceProvider)
            .Returns(_mockServiceProvider.Object);

        // Setup exception handler service
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IExceptionHandlerService)))
            .Returns(_mockExceptionHandler.Object);

        // Initialize ServiceLocator with mock service provider
        ServiceLocator.SetLocatorProvider(_mockServiceProvider.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        ServiceLocator.Default.Dispose();
    }

    [TestMethod]
    public void Execute_WithParameter_PassesParameterCorrectly()
    {
        // Arrange
        string? receivedParameter = null;
        var command = new RelayCommand<string>(param => receivedParameter = param);

        // Act
        command.Execute("test");

        // Assert
        Assert.AreEqual("test", receivedParameter);
    }

    [TestMethod]
    public void CanExecute_WithGenericPredicate_ReturnsExpectedResult()
    {
        // Arrange
        var command = new RelayCommand<int>(
            param => { },
            param => param > 0
        );

        // Act & Assert
        Assert.IsFalse(command.CanExecute(-1));
        Assert.IsTrue(command.CanExecute(1));
    }

    [TestMethod]
    public void Execute_WithNullableParameter_HandlesNullCorrectly()
    {
        // Arrange
        string? receivedParameter = "not null";
        var command = new RelayCommand<string?>(param => receivedParameter = param);

        // Act
        command.Execute(null);

        // Assert
        Assert.IsNull(receivedParameter);
    }

    [TestMethod]
    public void Execute_WithValueType_HandlesDefaultValueCorrectly()
    {
        // Arrange
        int receivedParameter = -1;
        var command = new RelayCommand<int>(param => receivedParameter = param);

        // Act
        command.Execute(default);

        // Assert
        Assert.AreEqual(0, receivedParameter);
    }

    [TestMethod]
    public void Execute_WithInvalidParameterType_ThrowsArgumentException()
    {
        // Arrange
        var command = new RelayCommand<int>(_ => { });

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            command.Execute("invalid parameter type"));
    }

    [TestMethod]
    public void Execute_WithException_HandlesExceptionCorrectly()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand<string>(_ => throw expectedException);

        // Act
        command.Execute("test");

        // Assert
        _mockExceptionHandler.Verify(
            x => x.HandleExceptionAsync(expectedException),
            Times.Once);
    }

    [TestMethod]
    public void CanExecute_WithNullForValueType_ConsultsPredicate()
    {
        // Arrange
        var commandWithoutPredicate = new RelayCommand<int>(_ => { });
        var commandWithTruePredicate = new RelayCommand<int>(_ => { }, _ => true);
        var commandWithFalsePredicate = new RelayCommand<int>(_ => { }, _ => false);

        // Act & Assert
        // Without an explicit predicate, should return true (default behavior)
        Assert.IsTrue(commandWithoutPredicate.CanExecute(null));

        // With a predicate that returns true, should return true
        Assert.IsTrue(commandWithTruePredicate.CanExecute(null));

        // With a predicate that returns false, should return false
        Assert.IsFalse(commandWithFalsePredicate.CanExecute(null));
    }

    [TestMethod]
    public void CanExecute_WithBoxedValueType_HandlesCorrectly()
    {
        // Arrange
        var command = new RelayCommand<int>(
            param => { },
            param => param > 0
        );

        // Act & Assert
        object boxedValue = 42;
        Assert.IsTrue(command.CanExecute(boxedValue));
    }

    [TestMethod]
    public void TryGetCommandArgument_WithValidParameters_ReturnsTrue()
    {
        // Test various scenarios
        Assert.IsTrue(RelayCommand<string>.TryGetCommandArgument(null, out string? result1));
        Assert.IsNull(result1);

        Assert.IsTrue(RelayCommand<int>.TryGetCommandArgument(42, out int result2));
        Assert.AreEqual(42, result2);

        Assert.IsTrue(RelayCommand<object>.TryGetCommandArgument(new object(), out object? result3));
        Assert.IsNotNull(result3);
    }

    [TestMethod]
    public void ThrowArgumentExceptionForInvalidCommandArgument_ThrowsCorrectException()
    {
        // Act & Assert
        var ex1 = Assert.ThrowsException<ArgumentException>(() =>
            RelayCommand<int>.ThrowArgumentExceptionForInvalidCommandArgument(null));
        Assert.IsTrue(ex1.Message.Contains("must not be null"));

        var ex2 = Assert.ThrowsException<ArgumentException>(() =>
            RelayCommand<int>.ThrowArgumentExceptionForInvalidCommandArgument("invalid"));
        Assert.IsTrue(ex2.Message.Contains("cannot be of type"));
    }

    [TestMethod]
    public void Dispose_MultipleTimes_HandlesCorrectly()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { });

        // Act & Assert - should not throw
        command.Dispose();
        command.Dispose(); // Second dispose should be safe
    }

    [TestMethod]
    public void Execute_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { });
        command.Dispose();

        // Act & Assert
        Assert.ThrowsException<ObjectDisposedException>(() => command.Execute("test"));
    }

    [TestMethod]
    public void CanExecute_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { });
        command.Dispose();

        // Act & Assert
        Assert.ThrowsException<ObjectDisposedException>(() => command.CanExecute("test"));
    }

    [TestMethod]
    public void Execute_WithComplexType_PassesReferenceCorrectly()
    {
        // Arrange
        var testObject = new TestClass { Value = "test" };
        TestClass? receivedObject = null;
        var command = new RelayCommand<TestClass>(param => receivedObject = param);

        // Act
        command.Execute(testObject);

        // Assert
        Assert.AreSame(testObject, receivedObject);
    }

    [TestMethod]
    public void Constructor_WithNullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new RelayCommand<string>(null!));
    }

    [TestMethod]
    public void Constructor_WithNullPredicate_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new RelayCommand<string>(_ => { }, null!));
    }


    private class TestClass
    {
        public string? Value { get; set; }
    }
}