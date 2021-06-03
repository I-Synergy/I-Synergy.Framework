using System.Linq.Expressions;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class ExpressionParserTests.
    /// </summary>
    [TestClass]
    public class ExpressionParserTests
    {
        /// <summary>
        /// Defines the test method Parse_ParseComparisonOperator.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        [DataTestMethod]
        [DataRow("it == 1", "(x == 1)")]
        [DataRow("it eq 1", "(x == 1)")]
        [DataRow("it equal 1", "(x == 1)")]
        [DataRow("it != 1", "(x != 1)")]
        [DataRow("it ne 1", "(x != 1)")]
        [DataRow("it neq 1", "(x != 1)")]
        [DataRow("it notequal 1", "(x != 1)")]
        [DataRow("it lt 1", "(x < 1)")]
        [DataRow("it LessThan 1", "(x < 1)")]
        [DataRow("it le 1", "(x <= 1)")]
        [DataRow("it LessThanEqual 1", "(x <= 1)")]
        [DataRow("it gt 1", "(x > 1)")]
        [DataRow("it GreaterThan 1", "(x > 1)")]
        [DataRow("it ge 1", "(x >= 1)")]
        [DataRow("it GreaterThanEqual 1", "(x >= 1)")]
        public void Parse_ParseComparisonOperator(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { Expression.Parameter(typeof(int), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }

        /// <summary>
        /// Defines the test method Parse_ParseOrOperator.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        [DataTestMethod]
        [DataRow("it || true", "(x OrElse True)")]
        [DataRow("it or true", "(x OrElse True)")]
        [DataRow("it OrElse true", "(x OrElse True)")]
        public void Parse_ParseOrOperator(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { Expression.Parameter(typeof(bool), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }

        /// <summary>
        /// Defines the test method Parse_ParseAndOperator.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        [DataTestMethod]
        [DataRow("it && true", "(x AndAlso True)")]
        [DataRow("it and true", "(x AndAlso True)")]
        [DataRow("it AndAlso true", "(x AndAlso True)")]
        public void Parse_ParseAndOperator(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { Expression.Parameter(typeof(bool), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }
    }
}
