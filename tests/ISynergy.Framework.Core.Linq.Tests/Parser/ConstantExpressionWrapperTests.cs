using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class ConstantExpressionWrapperTests.
    /// </summary>
    [TestClass]
    public class ConstantExpressionWrapperTests
    {
        /// <summary>
        /// The constant expression wrapper
        /// </summary>
        private readonly IConstantExpressionWrapper _constantExpressionWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantExpressionWrapperTests"/> class.
        /// </summary>
        public ConstantExpressionWrapperTests()
        {
            _constantExpressionWrapper = new ConstantExpressionWrapper();
        }

        /// <summary>
        /// Defines the test method ConstantExpressionWrapper_Wrap_ConstantExpression_PrimitiveTypes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="test">The test.</param>
        [DataTestMethod]
        [DataRow(true)]
        [DataRow((int)1)]
        [DataRow((uint)2)]
        [DataRow((short)3)]
        [DataRow((ushort)4)]
        [DataRow(5L)]
        [DataRow(6UL)]
        [DataRow(7.1f)]
        [DataRow(8.1d)]
        [DataRow('c')]
        [DataRow((byte)10)]
        [DataRow((sbyte)11)]
        public void ConstantExpressionWrapper_Wrap_ConstantExpression_PrimitiveTypes<T>(T test)
        {
            // Assign
            var expression = Expression.Constant(test) as Expression;

            // Act
            _constantExpressionWrapper.Wrap(ref expression);

            // Verify
            Check.That(expression).IsNotNull();

            ConstantExpression constantExpression = (expression as MemberExpression).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            T value = wrappedObj.Value;

            Check.That(value).IsEqualTo(test);
        }

        /// <summary>
        /// Defines the test method ConstantExpressionWrapper_Wrap_ConstantExpression_String.
        /// </summary>
        [TestMethod]
        public void ConstantExpressionWrapper_Wrap_ConstantExpression_String()
        {
            // Assign
            string test = "abc";
            var expression = Expression.Constant(test) as Expression;

            // Act
            _constantExpressionWrapper.Wrap(ref expression);

            // Verify
            Check.That(expression).IsNotNull();

            ConstantExpression constantExpression = (expression as MemberExpression).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            string value = wrappedObj.Value;

            Check.That(value).IsEqualTo(test);
        }

        /// <summary>
        /// Defines the test method ConstantExpressionWrapper_Wrap_ConstantExpression_ComplexTypes.
        /// </summary>
        [TestMethod]
        public void ConstantExpressionWrapper_Wrap_ConstantExpression_ComplexTypes()
        {
            // Assign
            var test = DateTime.Now;
            var expression = Expression.Constant(test) as Expression;

            // Act
            _constantExpressionWrapper.Wrap(ref expression);

            // Verify
            Check.That(expression).IsNotNull();

            var constantExpression = (expression as MemberExpression).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            DateTime value = wrappedObj.Value;

            Check.That(value).IsEqualTo(test);
        }
    }
}
