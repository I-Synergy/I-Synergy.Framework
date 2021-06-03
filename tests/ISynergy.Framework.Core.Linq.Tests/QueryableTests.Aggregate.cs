using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method Aggregate_Average.
        /// </summary>
        [TestMethod]
        public void Aggregate_Average()
        {
            // Arrange
            var queryable = new[]
            {
                new AggregateTest { Double = 50, Float = 1.0f, Int = 42, NullableDouble = 400, NullableFloat = 100f, NullableInt = 60 },
                new AggregateTest { Double = 0, Float = 0.0f, Int = 0, NullableDouble = 0, NullableFloat = 0, NullableInt = 0 }
            }.AsQueryable();

            // Act
            var resultNullableFloat = queryable.Aggregate("Average", "NullableFloat");
            var resultFloat = queryable.Aggregate("Average", "Float");
            var resultDouble = queryable.Aggregate("Average", "Double");
            var resultNullableDouble = queryable.Aggregate("Average", "NullableDouble");
            var resultInt = queryable.Aggregate("Average", "Int");
            var resultNullableInt = queryable.Aggregate("Average", "NullableInt");

            // Assert
            Assert.AreEqual(50f, resultNullableFloat);
            Assert.AreEqual(200.0, resultNullableDouble);
            Assert.AreEqual(25.0, resultDouble);
            Assert.AreEqual(0.5f, resultFloat);
            Assert.AreEqual(21.0, resultInt);
            Assert.AreEqual(30.0, resultNullableInt);
        }

        /// <summary>
        /// Defines the test method Aggregate_Min.
        /// </summary>
        [TestMethod]
        public void Aggregate_Min()
        {
            // Arrange
            var queryable = new[]
            {
                new AggregateTest { Double = 50, Float = 1.0f, Int = 42, NullableDouble = 400, NullableFloat = 100f, NullableInt = 60 },
                new AggregateTest { Double = 51, Float = 2.0f, Int = 90, NullableDouble = 800, NullableFloat = 101f, NullableInt = 61 }
            }.AsQueryable();

            // Act
            var resultDouble = queryable.Aggregate("Min", "Double");
            var resultFloat = queryable.Aggregate("Min", "Float");
            var resultInt = queryable.Aggregate("Min", "Int");
            var resultNullableDouble = queryable.Aggregate("Min", "NullableDouble");
            var resultNullableFloat = queryable.Aggregate("Min", "NullableFloat");
            var resultNullableInt = queryable.Aggregate("Min", "NullableInt");

            // Assert
            Assert.AreEqual(50.0, resultDouble);
            Assert.AreEqual(1.0f, resultFloat);
            Assert.AreEqual(42, resultInt);
            Assert.AreEqual(400.0, resultNullableDouble);
            Assert.AreEqual(100f, resultNullableFloat);
            Assert.AreEqual(60, resultNullableInt);
        }

        /// <summary>
        /// Class AggregateTest.
        /// </summary>
        public class AggregateTest
        {
            /// <summary>
            /// Gets or sets the double.
            /// </summary>
            /// <value>The double.</value>
            public double Double { get; set; }

            /// <summary>
            /// Gets or sets the nullable double.
            /// </summary>
            /// <value>The nullable double.</value>
            public double? NullableDouble { get; set; }

            /// <summary>
            /// Gets or sets the float.
            /// </summary>
            /// <value>The float.</value>
            public float Float { get; set; }

            /// <summary>
            /// Gets or sets the nullable float.
            /// </summary>
            /// <value>The nullable float.</value>
            public float? NullableFloat { get; set; }

            /// <summary>
            /// Gets or sets the int.
            /// </summary>
            /// <value>The int.</value>
            public int Int { get; set; }

            /// <summary>
            /// Gets or sets the nullable int.
            /// </summary>
            /// <value>The nullable int.</value>
            public int? NullableInt { get; set; }
        }
    }
}
