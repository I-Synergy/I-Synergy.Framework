using ISynergy.Framework.Mvvm.Abstractions.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class CancelCommandTests
{
    [TestMethod]
    public void CanExecute_WhenCommandCanBeCanceled_ReturnsTrue()
    {
        // Arrange
        var mockAsyncCommand = new Mock<IAsyncRelayCommand>();
        mockAsyncCommand.Setup(x => x.CanBeCanceled).Returns(true);
        var cancelCommand = new CancelCommand(mockAsyncCommand.Object);

        // Act
        bool canExecute = cancelCommand.CanExecute(null);

        // Assert
        Assert.IsTrue(canExecute);
    }

    [TestMethod]
    public void CanExecute_WhenCommandCannotBeCanceled_ReturnsFalse()
    {
        // Arrange
        var mockAsyncCommand = new Mock<IAsyncRelayCommand>();
        mockAsyncCommand.Setup(x => x.CanBeCanceled).Returns(false);
        var cancelCommand = new CancelCommand(mockAsyncCommand.Object);

        // Act
        bool canExecute = cancelCommand.CanExecute(null);

        // Assert
        Assert.IsFalse(canExecute);
    }

    [TestMethod]
    public void Execute_CallsCancelOnWrappedCommand()
    {
        // Arrange
        var mockAsyncCommand = new Mock<IAsyncRelayCommand>();
        var cancelCommand = new CancelCommand(mockAsyncCommand.Object);

        // Act
        cancelCommand.Execute(null);

        // Assert
        mockAsyncCommand.Verify(x => x.Cancel(), Times.Once);
    }

    [TestMethod]
    public void CanExecuteChanged_ProperlyPropagates()
    {
        // Arrange
        var mockAsyncCommand = new Mock<IAsyncRelayCommand>();
        var cancelCommand = new CancelCommand(mockAsyncCommand.Object);
        bool eventRaised = false;
        cancelCommand.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        mockAsyncCommand.Raise(x => x.PropertyChanged += null,
            new System.ComponentModel.PropertyChangedEventArgs(nameof(IAsyncRelayCommand.CanBeCanceled)));

        // Assert
        Assert.IsTrue(eventRaised);
    }

    [TestMethod]
    public void CanExecuteChanged_OtherPropertyChanges_DoesNotRaise()
    {
        // Arrange
        var mockAsyncCommand = new Mock<IAsyncRelayCommand>();
        var cancelCommand = new CancelCommand(mockAsyncCommand.Object);
        bool eventRaised = false;
        cancelCommand.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act
        mockAsyncCommand.Raise(x => x.PropertyChanged += null,
            new System.ComponentModel.PropertyChangedEventArgs("SomeOtherProperty"));

        // Assert
        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void Dispose_RemovesEventHandler()
    {
        // Arrange
        var mockAsyncCommand = new Mock<IAsyncRelayCommand>();
        var cancelCommand = new CancelCommand(mockAsyncCommand.Object);

        // Act
        cancelCommand.Dispose();

        // Raise property changed after disposal
        mockAsyncCommand.Raise(x => x.PropertyChanged += null,
            new System.ComponentModel.PropertyChangedEventArgs(nameof(IAsyncRelayCommand.CanBeCanceled)));

        // Assert - if event handler wasn't removed, this would throw
        mockAsyncCommand.VerifyAdd(x => x.PropertyChanged += It.IsAny<System.ComponentModel.PropertyChangedEventHandler>(), Times.Once);
        mockAsyncCommand.VerifyRemove(x => x.PropertyChanged -= It.IsAny<System.ComponentModel.PropertyChangedEventHandler>(), Times.Once);
    }
}
