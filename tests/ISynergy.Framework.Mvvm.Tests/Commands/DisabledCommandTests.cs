using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mvvm.Commands.Tests;

[TestClass]
public class DisabledCommandTests
{
    [TestMethod]
    public void Instance_ReturnsSameInstance()
    {
        // Act
        var instance1 = DisabledCommand.Instance;
        var instance2 = DisabledCommand.Instance;

        // Assert
        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void CanExecute_AlwaysReturnsFalse()
    {
        // Arrange
        var command = DisabledCommand.Instance;

        // Act & Assert
        Assert.IsFalse(command.CanExecute(null));
        Assert.IsFalse(command.CanExecute("some parameter"));
        Assert.IsFalse(command.CanExecute(42));
    }

    [TestMethod]
    public void Execute_DoesNothing()
    {
        // Arrange
        var command = DisabledCommand.Instance;

        // Act & Assert - should not throw
        command.Execute(null);
        command.Execute("some parameter");
        command.Execute(42);
    }

    [TestMethod]
    public void CanExecuteChanged_NeverRaises()
    {
        // Arrange
        var command = DisabledCommand.Instance;
        bool eventRaised = false;

        // Act
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        // Assert
        Assert.IsFalse(eventRaised);
    }

    [TestMethod]
    public void CanExecuteChanged_MultipleSubscriptions_HandlesSafely()
    {
        // Arrange
        var command = DisabledCommand.Instance;
        EventHandler handler1 = (s, e) => { };
        EventHandler handler2 = (s, e) => { };

        // Act & Assert - should not throw
        command.CanExecuteChanged += handler1;
        command.CanExecuteChanged += handler2;
        command.CanExecuteChanged -= handler1;
        command.CanExecuteChanged -= handler2;
    }
}
