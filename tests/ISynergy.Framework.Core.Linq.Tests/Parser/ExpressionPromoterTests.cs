using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class ExpressionPromoterTests.
    /// </summary>
    public class ExpressionPromoterTests
    {
        /// <summary>
        /// Class SampleDto.
        /// </summary>
        public class SampleDto
        {
            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            /// <value>The data.</value>
            public Guid data { get; set; }
        }

        /// <summary>
        /// The expression promoter mock
        /// </summary>
        private readonly Mock<IExpressionPromoter> _expressionPromoterMock;
        /// <summary>
        /// The dynamic link custom type provider mock
        /// </summary>
        private readonly Mock<IDynamicLinkCustomTypeProvider> _dynamicLinkCustomTypeProviderMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionPromoterTests"/> class.
        /// </summary>
        public ExpressionPromoterTests()
        {
            _dynamicLinkCustomTypeProviderMock = new Mock<IDynamicLinkCustomTypeProvider>();
            _dynamicLinkCustomTypeProviderMock.Setup(d => d.GetCustomTypes()).Returns(new HashSet<Type>());
            _dynamicLinkCustomTypeProviderMock.Setup(d => d.ResolveType(It.IsAny<string>())).Returns(typeof(SampleDto));

            _expressionPromoterMock = new Mock<IExpressionPromoter>();
            _expressionPromoterMock.Setup(e => e.Promote(It.IsAny<Expression>(), It.IsAny<Type>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Expression.Constant(Guid.NewGuid()));
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_WithCustomExpressionPromoter.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_WithCustomExpressionPromoter()
        {
            // Assign
            var parsingConfig = new ParsingConfig()
            {
                AllowNewToEvaluateAnyType = true,
                CustomTypeProvider = _dynamicLinkCustomTypeProviderMock.Object,
                ExpressionPromoter = _expressionPromoterMock.Object
            };

            // Act
            string query = $"new {typeof(SampleDto).FullName}(@0 as data)";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(parsingConfig, null, query, new object[] { Guid.NewGuid().ToString() });
            Delegate del = expression.Compile();
            SampleDto result = (SampleDto)del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);

            // Verify
            _dynamicLinkCustomTypeProviderMock.Verify(d => d.GetCustomTypes(), Times.Once);
            _dynamicLinkCustomTypeProviderMock.Verify(d => d.ResolveType($"{typeof(SampleDto).FullName}"), Times.Once);

            _expressionPromoterMock.Verify(e => e.Promote(It.IsAny<ConstantExpression>(), typeof(Guid), true, true), Times.Once);
        }
    }
}
