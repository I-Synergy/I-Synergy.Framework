using NFluent;
using ISynergy.Framework.Core.Linq.Exceptions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method GroupBy_Dynamic.
        /// </summary>
        [TestMethod]
        public void GroupBy_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var byAgeReturnUserName = qry.GroupBy("Profile.Age", "UserName");
            var byAgeReturnAll = qry.GroupBy("Profile.Age");

            //Assert
            Assert.AreEqual(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnUserName.Count());
            Assert.AreEqual(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnAll.Count());
        }

        // https://github.com/StefH/System.Linq.Dynamic.Core/issues/75
        /// <summary>
        /// Defines the test method GroupBy_Dynamic_Issue75.
        /// </summary>
        [TestMethod]
        public void GroupBy_Dynamic_Issue75()
        {
            var testList = User.GenerateSampleModels(100);

            var resultDynamic = testList.AsQueryable().GroupBy("Profile.Age").Select("new (it.key as PropertyKey)");
            var result = testList.GroupBy(e => e.Profile.Age).Select(e => new { PropertyKey = e.Key }).AsQueryable();

            // I think this should be true, but it isn't. dynamicResult add System.Object Item [System.String] property.
            PropertyInfo[] properties = result.ElementType.GetTypeInfo().GetProperties();
            PropertyInfo[] propertiesDynamic = resultDynamic.ElementType.GetTypeInfo().GetProperties();

            Check.That(propertiesDynamic.Length).IsStrictlyGreaterThan(properties.Length);
        }

        /// <summary>
        /// Defines the test method GroupBy_Dynamic_Exceptions.
        /// </summary>
        [TestMethod]
        public void GroupBy_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.ThrowsException<ParseException>(() => qry.GroupBy("Bad"));
            Assert.ThrowsException<ParseException>(() => qry.GroupBy("Id, UserName"));
            Assert.ThrowsException<ParseException>(() => qry.GroupBy("new Id, UserName"));
            Assert.ThrowsException<ParseException>(() => qry.GroupBy("new (Id, UserName"));
            Assert.ThrowsException<ParseException>(() => qry.GroupBy("new (Id, UserName, Bad)"));

            Assert.ThrowsException<ArgumentNullException>(() => DynamicQueryExtensions.GroupBy((IQueryable<string>)null, "Id"));
            Assert.ThrowsException<ArgumentNullException>(() => qry.GroupBy(null));
            Assert.ThrowsException<ArgumentNullException>(() => qry.GroupBy(""));
            Assert.ThrowsException<ParseException>(() => qry.GroupBy(" "));

            Assert.ThrowsException<ArgumentNullException>(() => qry.GroupBy("Id", (string)null));
            Assert.ThrowsException<ArgumentNullException>(() => qry.GroupBy("Id", ""));
            Assert.ThrowsException<ParseException>(() => qry.GroupBy("Id", " "));
        }
    }
}
