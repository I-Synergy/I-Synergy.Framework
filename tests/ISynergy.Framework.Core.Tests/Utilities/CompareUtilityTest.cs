using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ISynergy.Framework.Core.Utilities.Tests
{
    public class CompareUtilityTest
    {
        public class EmployeeRecord
        {
            public int EmployeeNumber { get; set; }
            public int CompanyNumber { get; set; }
        }

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
