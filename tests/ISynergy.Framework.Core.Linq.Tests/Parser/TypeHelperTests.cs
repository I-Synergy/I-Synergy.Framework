using ISynergy.Framework.Core.Linq.Helpers;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    public class TypeHelperTests
    {
        enum TestEnum
        {
            x = 1
        }

        [Fact]
        public void TypeHelper_ParseEnum_Valid()
        {
            // Assign + Act
            var result = TypeHelper.ParseEnum("x", typeof(TestEnum));

            // Assert
            Check.That(result).Equals(TestEnum.x);
        }

        [Fact]
        public void TypeHelper_ParseEnum_Invalid()
        {
            // Assign + Act
            var result = TypeHelper.ParseEnum("test", typeof(TestEnum));

            // Assert
            Check.That(result).IsNull();
        }

        [Fact]
        public void TypeHelper_IsCompatibleWith_SameTypes_True()
        {
            // Assign + Act
            var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(int));

            // Assert
            Check.That(result).IsTrue();
        }

        [Fact]
        public void TypeHelper_IsCompatibleWith_True()
        {
            // Assign + Act
            var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(long));

            // Assert
            Check.That(result).IsTrue();
        }

        [Fact]
        public void TypeHelper_IsCompatibleWith_False()
        {
            // Assign + Act
            var result = TypeHelper.IsCompatibleWith(typeof(long), typeof(int));

            // Assert
            Check.That(result).IsFalse();
        }
    }
}
