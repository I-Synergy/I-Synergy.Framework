using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    /// <summary>
    /// Class CompareUtilityTest.
    /// </summary>
    public class CompareUtilityTest
    {
        /// <summary>
        /// Class EmployeeRecord.
        /// </summary>
        public class EmployeeRecord
        {
            /// <summary>
            /// Gets or sets the employee number.
            /// </summary>
            /// <value>The employee number.</value>
            public int EmployeeNumber { get; set; }
            /// <summary>
            /// Gets or sets the company number.
            /// </summary>
            /// <value>The company number.</value>
            public int CompanyNumber { get; set; }
        }

        /// <summary>
        /// Defines the test method CompareObject.
        /// </summary>
        [Fact]
        public void CompareObject()
        {
            var oOldRecord = new EmployeeRecord
            {
                EmployeeNumber = 1
            };

            var oNewRecord = new EmployeeRecord
            {
                EmployeeNumber = 2,
                CompanyNumber = 3
            };

            var result = CompareUtility.CompareObject(oOldRecord, oNewRecord);
            var assert = new List<string>
            {
                "Property EmployeeNumber was: 1; is: 2",
                "Property CompanyNumber was: 0; is: 3"
            };
            
            Assert.True(assert.All(result.Contains) && assert.Count == result.Count);
        }

        /// <summary>
        /// Defines the test method CompareDecimalValues.
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="operation">The operation.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        [Theory]
        [InlineData(true, "==", 50.99, 50.99)]
        [InlineData(true, "!=", 50.99, 50)]
        [InlineData(true, ">", 1, 0)]
        [InlineData(true, ">=", 50.01, 50)]
        [InlineData(true, "<", 0, 1)]
        [InlineData(true, "<=", 49, 49.99)]
        public void CompareDecimalValues(bool result, string operation, double value1, double value2)
        {
            var assert = CompareUtility.Compare(operation, value1, value2);
            Assert.Equal(result, assert);
        }
    }
}
