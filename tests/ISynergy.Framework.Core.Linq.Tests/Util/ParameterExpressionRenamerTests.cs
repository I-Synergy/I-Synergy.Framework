using NFluent;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Util
{
    /// <summary>
    /// Class ParameterExpressionRenamerTests.
    /// </summary>
    [TestClass]
    public class ParameterExpressionRenamerTests
    {
        /// <summary>
        /// Defines the test method ParameterExpressionRenamer_Rename_ToNewName.
        /// </summary>
        /// <param name="newName">The new name.</param>
        /// <param name="resultString">The result string.</param>
        [DataTestMethod]
        [DataRow("test", "(test + 42)")]
        public void ParameterExpressionRenamer_Rename_ToNewName(string newName, string resultString)
        {
            // Assign
            var expression = Expression.Add(Expression.Parameter(typeof(int), ""), Expression.Constant(42));
            var sut = new ParameterExpressionRenamer(newName);

            // Act
            string result = sut.Rename(expression, out ParameterExpression parameterExpression).ToString();

            // Assert
            Check.That(result).IsEqualTo(resultString);
            Check.That(parameterExpression.Name).IsEqualTo(newName);
        }

        /// <summary>
        /// Defines the test method ParameterExpressionRenamer_Rename_OldNameInNewName.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="resultString">The result string.</param>
        [DataTestMethod]
        [DataRow("", "test", "(test + 42)")]
        [DataRow("x", "test", "(test + 42)")]
        public void ParameterExpressionRenamer_Rename_OldNameInNewName(string oldName, string newName, string resultString)
        {
            // Assign
            var expression = Expression.Add(Expression.Parameter(typeof(int), oldName), Expression.Constant(42));
            var sut = new ParameterExpressionRenamer(oldName, newName);

            // Act
            string result = sut.Rename(expression, out ParameterExpression parameterExpression).ToString();

            // Assert
            Check.That(result).IsEqualTo(resultString);
            Check.That(parameterExpression.Name).IsEqualTo(newName);
        }

        /// <summary>
        /// Defines the test method ParameterExpressionRenamer_Rename_NoParameterExpressionPresent.
        /// </summary>
        [TestMethod]
        public void ParameterExpressionRenamer_Rename_NoParameterExpressionPresent()
        {
            // Assign
            var expression = Expression.Add(Expression.Constant(1), Expression.Constant(2));
            var sut = new ParameterExpressionRenamer("test");

            // Act
            string result = sut.Rename(expression, out ParameterExpression parameterExpression).ToString();

            // Assert
            Check.That(result).IsEqualTo("(1 + 2)");
            Check.That(parameterExpression).IsNull();
        }
    }
}
