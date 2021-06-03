using ISynergy.Framework.Core.Linq.Helpers;
using NFluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class TypeHelperTests.
    /// </summary>
    [TestClass]
    public class TypeHelperTests
    {
        /// <summary>
        /// Enum TestEnum
        /// </summary>
        enum TestEnum
        {
            /// <summary>
            /// The x
            /// </summary>
            x = 1
        }

        /// <summary>
        /// Defines the test method TypeHelper_ParseEnum_Valid.
        /// </summary>
        [TestMethod]
        public void TypeHelper_ParseEnum_Valid()
        {
            // Assign + Act
            var result = TypeHelper.ParseEnum("x", typeof(TestEnum));

            // Assert
            Check.That(result).Equals(TestEnum.x);
        }

        /// <summary>
        /// Defines the test method TypeHelper_ParseEnum_Invalid.
        /// </summary>
        [TestMethod]
        public void TypeHelper_ParseEnum_Invalid()
        {
            // Assign + Act
            var result = TypeHelper.ParseEnum("test", typeof(TestEnum));

            // Assert
            Check.That(result).IsNull();
        }

        /// <summary>
        /// Defines the test method TypeHelper_IsCompatibleWith_SameTypes_True.
        /// </summary>
        [TestMethod]
        public void TypeHelper_IsCompatibleWith_SameTypes_True()
        {
            // Assign + Act
            var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(int));

            // Assert
            Check.That(result).IsTrue();
        }

        /// <summary>
        /// Defines the test method TypeHelper_IsCompatibleWith_True.
        /// </summary>
        [TestMethod]
        public void TypeHelper_IsCompatibleWith_True()
        {
            // Assign + Act
            var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(long));

            // Assert
            Check.That(result).IsTrue();
        }

        /// <summary>
        /// Defines the test method TypeHelper_IsCompatibleWith_False.
        /// </summary>
        [TestMethod]
        public void TypeHelper_IsCompatibleWith_False()
        {
            // Assign + Act
            var result = TypeHelper.IsCompatibleWith(typeof(long), typeof(int));

            // Assert
            Check.That(result).IsFalse();
        }
    }
}
