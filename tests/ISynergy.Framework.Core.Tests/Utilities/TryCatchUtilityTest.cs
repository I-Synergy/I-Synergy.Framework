using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Utilities.Tests;

[TestClass]
public class TryCatchUtilityTest
{
    [TestMethod]
    public void IgnoreAllErrors_ShouldReturnTrue_WhenNoExceptionThrown()
    {
        bool operationExecuted = false;

        void Operation()
        {
            operationExecuted = true;
        }

        bool result = TryCatchUtility.IgnoreAllErrors(Operation);

        Assert.IsTrue(result);
        Assert.IsTrue(operationExecuted);
    }

    [TestMethod]
    public void IgnoreErrors_ShouldReturnFalse_WhenExceptionThrown()
    {
        void Operation()
        {
            throw new ArgumentException("Test Exception");
        }

        bool result = TryCatchUtility.IgnoreErrors<ArgumentException>(Operation);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IgnoreErrors_ShouldReturnException_WhenExceptionThrown()
    {
        void Operation()
        {
            throw new ArgumentException("Test Exception");
        }

        Assert.Throws<ArgumentException>(() =>
        {
            bool result = TryCatchUtility.IgnoreErrors<NotImplementedException>(Operation);
        });
    }

    [TestMethod]
    public void IgnoreAllErrors_WithValidOperation_ReturnsOperationResult()
    {
        // Arrange
        int expected = 42;
        Func<int> operation = () => expected;

        // Act
        int result = TryCatchUtility.IgnoreAllErrors(operation);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void IgnoreErrors_WithInvalidOperation_ReturnsDefaultValue()
    {
        // Arrange
        int defaultValue = 0;
        Func<int> operation = () => throw new InvalidOperationException();

        // Act
        int result = TryCatchUtility.IgnoreErrors<int, InvalidOperationException>(operation, defaultValue);

        // Assert
        Assert.AreEqual(defaultValue, result);
    }

    [TestMethod]
    public void IgnoreErrors_ShouldReturnException_ReturnsDefaultValue()
    {
        // Arrange
        int defaultValue = 0;
        Func<int> operation = () => throw new InvalidOperationException();

        Assert.Throws<InvalidOperationException>(() =>
        {
            int result = TryCatchUtility.IgnoreErrors<int, NotImplementedException>(operation, defaultValue);
        });
    }
}
