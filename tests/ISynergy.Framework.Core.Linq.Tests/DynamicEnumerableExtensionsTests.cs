using System.Collections.Generic;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class DynamicEnumerableExtensionsTests.
    /// </summary>
    public class DynamicEnumerableExtensionsTests
    {
        /// <summary>
        /// Defines the test method DynamicEnumerableExtensions_ToDynamicArray_int_to_int.
        /// </summary>
        [Fact]
        public void DynamicEnumerableExtensions_ToDynamicArray_int_to_int()
        {
            // Act
            var result = new List<int> { 0, 1 }.ToDynamicList(typeof(int));

            // Assert
            Check.That(result).ContainsExactly(0, 1);
        }

        /// <summary>
        /// Defines the test method DynamicEnumerableExtensions_ToDynamicArray_dynamic_to_int.
        /// </summary>
        [Fact]
        public void DynamicEnumerableExtensions_ToDynamicArray_dynamic_to_int()
        {
            // Act
            var result = new List<dynamic> { 0, 1 }.ToDynamicList(typeof(int));

            // Assert
            Check.That(result).ContainsExactly(0, 1);
        }

        /// <summary>
        /// Defines the test method DynamicEnumerableExtensions_ToDynamicArray_object_to_int.
        /// </summary>
        [Fact]
        public void DynamicEnumerableExtensions_ToDynamicArray_object_to_int()
        {
            // Act
            var result = new List<object> { 0, 1 }.ToDynamicList(typeof(int));

            // Assert
            Check.That(result).ContainsExactly(0, 1);
        }
    }
}
