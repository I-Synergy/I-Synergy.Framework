using ISynergy.Framework.Mvvm.Commands.Base;
using ISynergy.Framework.Mvvm.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class RelayCommandTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;

    public RelayCommandTests()
    {
        // Setup mocks
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
    }

    [TestCleanup]
    public void Cleanup()
    {
    }

    [TestMethod]
    public void Execute_SimpleCommand_ExecutesSuccessfully()
    {
        // Arrange
        bool wasExecuted = false;
        var command = new RelayCommand(() => wasExecuted = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.IsTrue(wasExecuted);
    }

    [TestMethod]
    public void CanExecute_WithPredicate_ReturnsExpectedResult()
    {
        // Arrange
        bool canExecuteValue = false;
        var command = new RelayCommand(
            () => { },
            () => canExecuteValue
        );

        // Act & Assert
        Assert.IsFalse(command.CanExecute(null));

        canExecuteValue = true;
        Assert.IsTrue(command.CanExecute(null));
    }

    [TestMethod]
    public void Execute_WithException_DoesNotThrowAndRaisesCommandExceptionDispatcher()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        var command = new RelayCommand(() => throw expectedException);

        CommandExceptionEventArgs? raisedArgs = null;
        object? raisedSender = null;
        void Handler(object? sender, CommandExceptionEventArgs e)
        {
            raisedSender = sender;
            raisedArgs = e;
        }

        CommandExceptionDispatcher.UnhandledCommandException += Handler;
        try
        {
            // Act - a synchronous throw must never escape Execute(): on some platforms (e.g.
            // Mac Catalyst) an unhandled exception on the calling thread aborts the process
            // unconditionally, bypassing the framework's global exception handling.
            command.Execute(null);
        }
        finally
        {
            CommandExceptionDispatcher.UnhandledCommandException -= Handler;
        }

        // Assert
        Assert.AreSame(command, raisedSender);
        Assert.IsNotNull(raisedArgs);
        Assert.AreSame(expectedException, raisedArgs!.Exception);
    }

    [TestMethod]
    public void Execute_WithNestedException_RaisesCommandExceptionDispatcherWithOuterException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception");
        var outerException = new Exception("Outer exception", innerException);
        var command = new RelayCommand(() => throw outerException);

        Exception? raisedException = null;
        void Handler(object? sender, CommandExceptionEventArgs e) => raisedException = e.Exception;

        CommandExceptionDispatcher.UnhandledCommandException += Handler;
        try
        {
            // Act
            command.Execute(null);
        }
        finally
        {
            CommandExceptionDispatcher.UnhandledCommandException -= Handler;
        }

        // Assert
        Assert.AreSame(outerException, raisedException);
    }

    [TestMethod]
    public void NotifyCanExecuteChanged_TriggersEvent()
    {
        // Arrange
        var command = new RelayCommand(() => { });
        bool wasNotified = false;
        command.CanExecuteChanged += (s, e) => wasNotified = true;

        // Act
        command.NotifyCanExecuteChanged();

        // Assert
        Assert.IsTrue(wasNotified);
    }

    [TestMethod]
    public void Dispose_MultipleTimes_HandlesCorrectly()
    {
        // Arrange
        var command = new RelayCommand(() => { });

        // Act
        command.Dispose();
        command.Dispose(); // Second dispose should be safe

        // Assert: command is disposed — further calls throw ObjectDisposedException
        Assert.Throws<ObjectDisposedException>(() => command.CanExecute(null));
    }

    [TestMethod]
    public void Execute_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var command = new RelayCommand(() => { });
        command.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => command.Execute(null));
    }

    [TestMethod]
    public void CanExecute_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var command = new RelayCommand(() => { });
        command.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => command.CanExecute(null));
    }

    [TestMethod]
    public void Constructor_WithNullAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
    }

    [TestMethod]
    public void Constructor_WithNullPredicate_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RelayCommand(() => { }, null!));
    }

}
