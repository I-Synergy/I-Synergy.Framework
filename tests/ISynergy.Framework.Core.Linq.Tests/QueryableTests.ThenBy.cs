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
        /// Defines the test method ThenBy_Dynamic.
        /// </summary>
        //[TestMethod]
        public void ThenBy_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
			var ordered = qry.OrderBy("Id");
            var thenByUserName = ordered.ThenBy("UserName");
            var thenByComplex1 = ordered.ThenBy("Profile.Age, Income");

            //Assert
            Assert.AreEqual(testList.OrderBy(x => x.Id).ThenBy(x => x.UserName).ToArray(), thenByUserName.ToArray());
            Assert.AreEqual(testList.OrderBy(x => x.Id).ThenBy(x => x.Profile.Age).ThenBy(x => x.Income).ToArray(), thenByComplex1.ToArray());
        }

        /// <summary>
        /// Defines the test method ThenBy_Dynamic_AsStringExpression.
        /// </summary>
        //[TestMethod]
        public void ThenBy_Dynamic_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var expected = qry.SelectMany(x => x.Roles.OrderBy(y => y.Name).ThenBy(y => y.Id)).Select(x => x.Name);
            var orderById = qry.SelectMany("Roles.OrderBy(Name).ThenBy(Id)").Select("Name");

            //Assert
            Assert.AreEqual(expected.ToArray(), orderById.Cast<string>().ToArray());
        }

        /// <summary>
        /// Defines the test method ThenBy_Dynamic_Exceptions.
        /// </summary>
        [TestMethod]
        public void ThenBy_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            var ordered = qry.OrderBy("Id");
            Assert.ThrowsException<ParseException>(() => ordered.ThenBy("Bad=3"));
            Assert.ThrowsException<ParseException>(() => ordered.Where("Id=123"));

            Assert.ThrowsException<ArgumentNullException>(() => DynamicQueryExtensions.ThenBy(null, "Id"));
            Assert.ThrowsException<ArgumentNullException>(() => ordered.ThenBy(null));
            Assert.ThrowsException<ArgumentNullException>(() => ordered.ThenBy(""));
            Assert.ThrowsException<ParseException>(() => ordered.ThenBy(" "));
        }
    }
}
