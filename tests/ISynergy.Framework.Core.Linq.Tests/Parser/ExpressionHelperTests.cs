using ISynergy.Framework.Core.Linq.Helpers;
using ISynergy.Framework.Core.Linq.Parsers;
using NFluent;
using System;
using System.Linq.Expressions;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class ExpressionHelperTests.
    /// </summary>
    public class ExpressionHelperTests
    {
        /// <summary>
        /// The expression helper
        /// </summary>
        private readonly ExpressionHelper _expressionHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionHelperTests"/> class.
        /// </summary>
        public ExpressionHelperTests()
        {
            _expressionHelper = new ExpressionHelper(ParsingConfig.Default);
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_WrapConstantExpression_false.
        /// </summary>
        [Fact]
        public void ExpressionHelper_WrapConstantExpression_false()
        {
            // Assign
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = false
            };
            var expressionHelper = new ExpressionHelper(config);

            string value = "test";
            Expression expression = Expression.Constant(value);

            // Act
            expressionHelper.WrapConstantExpression(ref expression);

            // Assert
            Check.That(expression).IsInstanceOf<ConstantExpression>();
            Check.That(expression.ToString()).Equals("\"test\"");
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_WrapConstantExpression_true.
        /// </summary>
        [Fact]
        public void ExpressionHelper_WrapConstantExpression_true()
        {
            // Assign
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = true
            };
            var expressionHelper = new ExpressionHelper(config);

            string value = "test";
            Expression expression = Expression.Constant(value);

            // Act
            expressionHelper.WrapConstantExpression(ref expression);
            expressionHelper.WrapConstantExpression(ref expression);

            // Assert
            Check.That(expression.GetType().FullName).Equals("System.Linq.Expressions.PropertyExpression");
            Check.That(expression.ToString()).Equals("value(ISynergy.Framework.Core.Linq.Parsers.WrappedValue`1[System.String]).Value");
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid.
        /// </summary>
        [Fact]
        public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid()
        {
            // Assign
            string guidAsString = Guid.NewGuid().ToString();

            // Act
            Expression result = _expressionHelper.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

            // Assert
            Check.That(result).IsInstanceOf<ConstantExpression>();
            Check.That(result.ToString()).Equals(guidAsString);
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid_Invalid.
        /// </summary>
        [Fact]
        public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid_Invalid()
        {
            // Assign
            string guidAsString = "x";

            // Act
            Expression result = _expressionHelper.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

            // Assert
            Check.That(result).IsNull();
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NestedNonNullable.
        /// </summary>
        [Fact]
        public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NestedNonNullable()
        {
            // Assign
            Expression<Func<Item, int>> expression = x => x.Relation1.Relation2.Id;

            // Act
            bool result = _expressionHelper.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

            // Assert
            Check.That(result).IsTrue();
            Check.That(generatedExpression.ToString()).IsEqualTo("(((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null))");
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NestedNullable_AddSelfFalse.
        /// </summary>
        [Fact]
        public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NestedNullable_AddSelfFalse()
        {
            // Assign
            Expression<Func<Item, int?>> expression = x => x.Relation1.Relation2.IdNullable;

            // Act
            bool result = _expressionHelper.TryGenerateAndAlsoNotNullExpression(expression, false, out Expression generatedExpression);

            // Assert
            Check.That(result).IsTrue();
            Check.That(generatedExpression.ToString()).IsEqualTo("(((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null))");
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NestedNullable_AddSelfTrue.
        /// </summary>
        [Fact]
        public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NestedNullable_AddSelfTrue()
        {
            // Assign
            Expression<Func<Item, int?>> expression = x => x.Relation1.Relation2.IdNullable;

            // Act
            bool result = _expressionHelper.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

            // Assert
            Check.That(result).IsTrue();
            Check.That(generatedExpression.ToString()).IsEqualTo("((((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null)) AndAlso (x => x.Relation1.Relation2.IdNullable != null))");
        }

        /// <summary>
        /// Defines the test method ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NonNullable.
        /// </summary>
        [Fact]
        public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_NonNullable()
        {
            // Assign
            Expression<Func<Item, int>> expression = x => x.Id;

            // Act
            bool result = _expressionHelper.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

            // Assert
            Check.That(result).IsFalse();
            Check.That(generatedExpression.ToString()).IsEqualTo("x => x.Id");
        }

        /// <summary>
        /// Class Item.
        /// </summary>
        class Item
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the relation1.
            /// </summary>
            /// <value>The relation1.</value>
            public Relation1 Relation1 { get; set; }
        }

        /// <summary>
        /// Class Relation1.
        /// </summary>
        class Relation1
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the relation2.
            /// </summary>
            /// <value>The relation2.</value>
            public Relation2 Relation2 { get; set; }
        }

        /// <summary>
        /// Class Relation2.
        /// </summary>
        class Relation2
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the identifier nullable.
            /// </summary>
            /// <value>The identifier nullable.</value>
            public int? IdNullable { get; set; }
        }
    }
}
