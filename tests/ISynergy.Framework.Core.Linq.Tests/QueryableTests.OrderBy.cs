using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method OrderBy_Dynamic.
        /// </summary>
        //[TestMethod]
        public void OrderBy_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var orderById = qry.OrderBy("Id");
            var orderByAge = qry.OrderBy("Profile.Age");
            var orderByComplex1 = qry.OrderBy("Profile.Age, Id");

            //Assert
            Assert.AreEqual(testList.OrderBy(x => x.Id).ToArray(), orderById.ToArray());
            Assert.AreEqual(testList.OrderBy(x => x.Profile.Age).ToArray(), orderByAge.ToArray());
            Assert.AreEqual(testList.OrderBy(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex1.ToArray());
        }

        /// <summary>
        /// Defines the test method OrderBy_Dynamic_AsStringExpression.
        /// </summary>
        //[TestMethod]
        public void OrderBy_Dynamic_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var expected = qry.SelectMany(x => x.Roles.OrderBy(y => y.Name)).Select(x => x.Name);
            var orderById = qry.SelectMany("Roles.OrderBy(Name)").Select("Name");

            //Assert
            Assert.AreEqual(expected.ToArray(), orderById.Cast<string>().ToArray());
        }

        /// <summary>
        /// Defines the test method OrderBy_Dynamic_Exceptions.
        /// </summary>
        [TestMethod]
        public void OrderBy_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.ThrowsException<ParseException>(() => qry.OrderBy("Bad=3"));
            Assert.ThrowsException<ParseException>(() => qry.Where("Id=123"));

            Assert.ThrowsException<ArgumentNullException>(() => DynamicQueryExtensions.OrderBy(null, "Id"));
            Assert.ThrowsException<ArgumentNullException>(() => qry.OrderBy(null));
            Assert.ThrowsException<ArgumentNullException>(() => qry.OrderBy(""));
            Assert.ThrowsException<ParseException>(() => qry.OrderBy(" "));
        }
    }
}
