using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISynergy.Framework.Payment.Extensions;

namespace ISynergy.Framework.Payment.Tests.Extensions
{
    /// <summary>
    /// Class DictionaryExtensionsTests.
    /// </summary>
    [TestClass]
    public class DictionaryExtensionsTests
    {

        /// <summary>
        /// Defines the test method CanCreateUrlQueryFromDictionary.
        /// </summary>
        [TestMethod]
        public void CanCreateUrlQueryFromDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>()
            {
                {"include", "issuers"},
                {"testmode", "true"}
            };
            var expected = "?include=issuers&testmode=true";

            // Act
            var result = parameters.ToQueryString();

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method CanCreateUrlQueryFromEmptyDictionary.
        /// </summary>
        [TestMethod]
        public void CanCreateUrlQueryFromEmptyDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>();
            var expected = string.Empty;

            // Act
            var result = parameters.ToQueryString();

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method CanAddParameterToDictionaryIfNotEmptyDictionary.
        /// </summary>
        [TestMethod]
        public void CanAddParameterToDictionaryIfNotEmptyDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>();
            var parameterName = "include";
            var parameterValue = "issuers";

            // Act
            parameters.AddValueIfNotNullOrEmpty(parameterName, parameterValue);

            // Assert
            Assert.IsTrue(parameters.Any());
            Assert.AreEqual(parameterValue, parameters[parameterName]);
        }

        /// <summary>
        /// Defines the test method CannotAddParameterToDictionaryIfEmptyDictionary.
        /// </summary>
        [TestMethod]
        public void CannotAddParameterToDictionaryIfEmptyDictionary()
        {
            // Arrange
            var parameters = new Dictionary<string, string>();

            // Act
            parameters.AddValueIfNotNullOrEmpty("include", "");

            // Assert
            Assert.IsFalse(parameters.Any());
        }
    }
}
