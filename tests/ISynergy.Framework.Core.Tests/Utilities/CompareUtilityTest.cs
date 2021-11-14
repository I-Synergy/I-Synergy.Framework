using ISynergy.Framework.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Tests.Utilities
{
    /// <summary>
    /// Class CompareUtilityTest.
    /// </summary>
    [TestClass]
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
        [TestMethod]
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

            Assert.IsTrue(assert.All(result.Contains) && assert.Count == result.Count);
        }

        /// <summary>
        /// Defines the test method CompareDecimalValues.
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="operation">The operation.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        [DataTestMethod]
        [DataRow(true, "==", 50.99, 50.99)]
        [DataRow(true, "!=", 50.99, 50)]
        [DataRow(true, ">", 1, 0)]
        [DataRow(true, ">=", 50.01, 50)]
        [DataRow(true, "<", 0, 1)]
        [DataRow(true, "<=", 49, 49.99)]
        public void CompareDecimalValues(bool result, string operation, double value1, double value2)
        {
            var assert = CompareUtility.Compare(operation, value1, value2);
            Assert.AreEqual(result, assert);
        }
    }
}
