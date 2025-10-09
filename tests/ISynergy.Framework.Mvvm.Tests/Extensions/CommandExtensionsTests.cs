using ISynergy.Framework.Mvvm.Abstractions.Commands;
using ISynergy.Framework.Mvvm.Commands;
using Moq;
using System.Windows.Input;

namespace ISynergy.Framework.Mvvm.Extensions.Tests;

[TestClass]
public class CommandExtensionsTests
{
    [TestMethod]
    public void CreateCancelCommand_WithNull_ThrowsArgumentNullException()
    {
        // Act
        Assert.Throws<ArgumentNullException>(() => CommandExtensions.CreateCancelCommand(null!));
    }

    [TestMethod]
    public void CreateCancelCommand_WithCancellableCommand_ReturnsCommand()
    {
        // Arrange
        var mockCommand = new Mock<IAsyncRelayCommand>();

        // Act
        ICommand cancelCommand = mockCommand.Object.CreateCancelCommand();

        // Assert
        Assert.IsNotNull(cancelCommand);
    }

    [TestMethod]
    public void CreateCancelCommand_WithMultipleCalls_ReturnsDifferentInstances()
    {
        // Arrange
        var mockCommand = new Mock<IAsyncRelayCommand>();

        // Act
        var cancelCommand1 = mockCommand.Object.CreateCancelCommand();
        var cancelCommand2 = mockCommand.Object.CreateCancelCommand();

        // Assert
        Assert.AreNotSame(cancelCommand1, cancelCommand2);
    }

    [TestMethod]
    public void CreateCancelCommand_WithCancellableCommand_ProperlyRelaysCanExecute()
    {
        // Arrange
        var mockCommand = new Mock<IAsyncRelayCommand>();
        mockCommand.Setup(x => x.CanBeCanceled).Returns(true);

        // Act
        var cancelCommand = mockCommand.Object.CreateCancelCommand();
        var canExecute = cancelCommand.CanExecute(null);

        // Assert
        Assert.IsTrue(canExecute);
        mockCommand.Verify(x => x.CanBeCanceled, Times.Once);
    }

    [TestMethod]
    public async Task CreateCancelCommand_Execute_CallsCancelOnCommand()
    {
        // Arrange
        var tcs = new TaskCompletionSource<bool>();
        var command = new AsyncRelayCommand(async (CancellationToken token) =>
        {
            try
            {
                await Task.Delay(1000, token);
                tcs.SetResult(false);
            }
            catch (OperationCanceledException)
            {
                tcs.SetResult(true);
            }
        });

        // Start the command execution
        var task = command.ExecuteAsync(null);

        // Create and execute the cancel command
        var cancelCommand = command.CreateCancelCommand();

        // Act
        cancelCommand.Execute(null);

        // Wait for the result
        var wasCancelled = await tcs.Task;

        // Assert
        Assert.IsTrue(wasCancelled);
        Assert.IsTrue(command.IsCancellationRequested);
    }

    [TestMethod]
    public void CreateCancelCommand_WithDisposedCommand_DisposesCorrectly()
    {
        // Arrange
        var mockCommand = new Mock<IAsyncRelayCommand>();

        // Act
        var cancelCommand = (IDisposable)mockCommand.Object.CreateCancelCommand();
        cancelCommand.Dispose();

        // No Assert needed - just verifying no exceptions are thrown
    }

    [TestMethod]
    public void CreateCancelCommand_WithPropertyChangedNotification_ProperlyPropagates()
    {
        // Arrange
        var mockCommand = new Mock<IAsyncRelayCommand>();
        var cancelCommand = mockCommand.Object.CreateCancelCommand();
        bool eventRaised = false;
        cancelCommand.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        mockCommand.Raise(m => m.PropertyChanged += null,
            new System.ComponentModel.PropertyChangedEventArgs(nameof(IAsyncRelayCommand.CanBeCanceled)));

        // Assert
        Assert.IsTrue(eventRaised);
    }
}
