using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method DefaultIfEmpty.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty()
        {
            // Arrange
            var queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var result = queryable.DefaultIfEmpty();
            var expected = queryable.DefaultIfEmpty();

            // Assert
            Check.That(result).ContainsExactly(expected);
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_Dynamic.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_Dynamic()
        {
            // Arrange
            var queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(u => u.Roles.Where(r => r.Name == "Admin").DefaultIfEmpty().FirstOrDefault()).ToDynamicArray<object>();
            var result = queryable.Select("it.Roles.Where(r => r.Name == \"Admin\").DefaultIfEmpty().FirstOrDefault()").ToDynamicArray<object>();

            // Assert
            Check.That(result).ContainsExactly(expected);
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_Empty.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_Empty()
        {
            // Arrange
            var queryable = User.GenerateSampleModels(1).Where(u => u.Income == -5).AsQueryable();

            // Act
            var result = queryable.DefaultIfEmpty();
            var expected = queryable.DefaultIfEmpty();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(expected).IsNotNull();
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_Empty_Dynamic.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_Empty_Dynamic()
        {
            // Arrange
            var queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(u => u.Roles.Where(r => r.Name == "a").DefaultIfEmpty().FirstOrDefault()).ToDynamicArray<object>();
            var result = queryable.Select("it.Roles.Where(r => r.Name == \"a\").DefaultIfEmpty().FirstOrDefault()").ToDynamicArray<object>();

            // Assert
            Check.That(result).ContainsExactly(expected);
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_WithDefaultValue.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_WithDefaultValue()
        {
            // Arrange
            var user = new User();
            var queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var result = queryable.DefaultIfEmpty(user);
            var expected = queryable.DefaultIfEmpty(user);

            // Assert
            Check.That(result).ContainsExactly(expected);
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_WithDefaultValue_Dynamic.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_WithDefaultValue_Dynamic()
        {
            // Arrange
            var role = new Role { Name = "?" };
            var queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(u => u.Roles.Where(r => r.Name == "Admin").DefaultIfEmpty(role).FirstOrDefault()).ToDynamicArray<object>();
            var result = queryable.Select("it.Roles.Where(r => r.Name == \"Admin\").DefaultIfEmpty(@0).FirstOrDefault()", role).ToDynamicArray<object>();

            // Assert
            Check.That(result).ContainsExactly(expected);
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_Empty_WithDefaultValue.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_Empty_WithDefaultValue()
        {
            // Arrange
            var user = new User { Income = 42 };
            var queryable = User.GenerateSampleModels(1).Where(u => u.Income == -5).AsQueryable();

            // Act
            var result = queryable.DefaultIfEmpty(user);
            var expected = queryable.DefaultIfEmpty(user);

            // Assert
            Check.That(result).ContainsExactly(expected);
        }

        /// <summary>
        /// Defines the test method DefaultIfEmpty_Empty_WithDefaultValue_Dynamic.
        /// </summary>
        [Fact]
        public void DefaultIfEmpty_Empty_WithDefaultValue_Dynamic()
        {
            // Arrange
            var role = new Role { Name = "?" };
            var queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(u => u.Roles.Where(r => r.Name == "a").DefaultIfEmpty(role).FirstOrDefault()).ToDynamicArray<object>();
            var result = queryable.Select("it.Roles.Where(r => r.Name == \"a\").DefaultIfEmpty(@0).FirstOrDefault()", role).ToDynamicArray<object>();

            // Assert
            Check.That(result).ContainsExactly(expected);
        }
    }
}
