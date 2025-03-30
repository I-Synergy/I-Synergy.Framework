using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    [TestClass]
    public class ExceptionExtensionsTests
    {
        [TestMethod]
        public void ToMessage_ReturnsCorrectLogMessage()
        {
            string? logMessage = null;

            // Arrange
            try
            {
                throw new Exception("Test Exception");
            }
            catch (Exception ex)
            {
                logMessage = ex.ToMessage(Environment.StackTrace);

                Assert.IsTrue(logMessage.Contains(ex.Message));
                Assert.IsNotNull(Environment.StackTrace);
                Assert.IsTrue(logMessage.Contains(Environment.StackTrace));
            }
        }

        [TestMethod]
        public void ToMessage_ReturnsLogMessageWithoutEnvironmentStackTrace()
        {
            // Arrange
            var exception = new Exception("Test Exception");
            var environmentStackTrace = "";

            // Act
            var logMessage = exception.ToMessage(environmentStackTrace);

            // Assert
            Assert.IsTrue(logMessage.Contains(exception.Message));
            Assert.IsTrue(string.IsNullOrEmpty(environmentStackTrace));
            Assert.IsTrue(logMessage.Contains(environmentStackTrace));
        }

        [TestMethod]
        public void ToMessage_ReturnsLogMessageWithNullEnvironmentStackTrace()
        {
            // Arrange
            var exception = new Exception("Test Exception");
            string? environmentStackTrace = null;

            // Act
            var logMessage = exception.ToMessage(environmentStackTrace!);

            // Assert
            Assert.IsTrue(logMessage.Contains(exception.Message));
            Assert.IsTrue(string.IsNullOrEmpty(environmentStackTrace));
        }
    }
}
