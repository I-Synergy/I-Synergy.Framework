using ISynergy.Framework.Tests.Base;
using System.Collections.Generic;
using Xunit;

namespace ISynergy.Utilities
{
    public class CompareUtilityTest : UnitTest
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

            List<string> result = CompareUtility.CompareObject(oOldRecord, oNewRecord);

            Assert.Equal(new List<string>
            {
                "Property EmployeeNumber was: 1; is: 2",
                "Property CompanyNumber was: 0; is: 3"
            }, result);
        }

        [Theory]
        [InlineData(true, "==", 50.99, 50.99)]
        [InlineData(true, "!=", 50.99, 50)]
        [InlineData(true, ">", 1, 0)]
        [InlineData(true, ">=", 50.01, 50)]
        [InlineData(true, "<", 0, 1)]
        [InlineData(true, "<=", 49, 49.99)]
        public void CompareDecimalValues(bool result, string operation, decimal value1, decimal value2)
        {
            bool assert = CompareUtility.Compare(operation, value1, value2);
            Assert.Equal(result, assert);
        }
    }
}
