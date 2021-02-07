using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class OperatorTests.
    /// </summary>
    public class OperatorTests
    {
        /// <summary>
        /// Defines the test method Operator_NullPropagation.
        /// </summary>
        [Fact]
        public void Operator_NullPropagation()
        {
            // Arrange
            var users = new[]
            {
                new User { Income = 1, Profile = new UserProfile { FirstName = "F1" } },
                new User { Income = 2, Profile = new UserProfile { FirstName = "F2" } },
                new User { Income = 3, Profile = null }
            }.AsQueryable();

            // Act
            Check.ThatCode(() => users.Select("Profile?.Age")).Throws<NotSupportedException>();
        }

        /// <summary>
        /// Defines the test method Operator_Multiplication_Single_Float_ParseException.
        /// </summary>
        [Fact]
        public void Operator_Multiplication_Single_Float_ParseException()
        {
            //Arrange
            var models = new[] { new SimpleValuesModel() }.AsQueryable();

            //Act + Assert
            Assert.Throws<ParseException>(() => models.Select("FloatValue * DecimalValue"));
        }

        /// <summary>
        /// Defines the test method Operator_Multiplication_Single_Float_Cast.
        /// </summary>
        [Fact]
        public void Operator_Multiplication_Single_Float_Cast()
        {
            //Arrange
            var models = new[] { new SimpleValuesModel { FloatValue = 2, DecimalValue = 3 } }.AsQueryable();

            //Act
            var result = models.Select("Decimal(FloatValue) * DecimalValue").First();

            //Assert
            Assert.Equal(6.0m, result);
        }
    }
}
