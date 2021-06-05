using System;
using System.Collections.Generic;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using ISynergy.Framework.Core.Linq.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method Where_Dynamic.
        /// </summary>
        [TestMethod]
        public void Where_Dynamic()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            // Act
            var userById = qry.Where("Id=@0", testList[10].Id);
            var userByUserName = qry.Where("UserName=\"User5\"");
            var nullProfileCount = qry.Where("Profile=null");
            var userByFirstName = qry.Where("Profile!=null && Profile.FirstName=@0", testList[1].Profile.FirstName);

            // Assert
            Assert.AreEqual(testList[10], userById.Single());
            Assert.AreEqual(testList[5], userByUserName.Single());
            Assert.AreEqual(testList.Count(x => x.Profile == null), nullProfileCount.Count());
            Assert.AreEqual(testList[1], userByFirstName.Single());
        }

        /// <summary>
        /// Defines the test method Where_Dynamic_CheckCastToObject.
        /// </summary>
        [TestMethod]
        public void Where_Dynamic_CheckCastToObject()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            // Act
            string dynamicExpression = qry.Where("Profile == null").Expression.ToDebugView();
            string expresion = qry.Where(var1 => var1.Profile == null).Expression.ToDebugView();

            // Assert
            NFluent.Check.That(dynamicExpression).Equals(expresion);
        }

        /// <summary>
        /// Defines the test method Where_Dynamic_DateTimeIsParsedAsUTC.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="hours">The hours.</param>
        [TestMethod]
        [DataRow("Fri, 10 May 2019 11:03:17 GMT", 11)]
        [DataRow("Fri, 10 May 2019 11:03:17 -07:00", 18)]
        public void Where_Dynamic_DateTimeIsParsedAsUTC(string time, int hours)
        {
            // Arrange
            var queryable = new List<Example> {
                new Example
                {
                    TimeNull = new DateTime(2019, 5, 10, hours, 3, 17, DateTimeKind.Utc)
                }
            }.AsQueryable();

            // Act
            var parsingConfig = new ParsingConfig
            {
                DateTimeIsParsedAsUTC = true
            };
            var result = queryable.Where(parsingConfig, $"it.TimeNull >= \"{time}\"");

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        /// <summary>
        /// https://github.com/StefH/System.Linq.Dynamic.Core/issues/19
        /// </summary>
        [TestMethod]
        public void Where_Dynamic_DateTime_NotEquals_Null()
        {
            //Arrange
            IQueryable<Entities.Post> queryable = new[] { new Entities.Post() }.AsQueryable();

            //Act
            var expected = queryable.Where(p => p.PostDate != null).ToArray();
            var result1 = queryable.Where("PostDate != null").ToArray();
            var result2 = queryable.Where("null != PostDate").ToArray();

            //Assert
            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        /// <summary>
        /// Defines the test method Where_Dynamic_DateTime_Equals_Null.
        /// </summary>
        //[TestMethod]
        public void Where_Dynamic_DateTime_Equals_Null()
        {
            //Arrange
            IQueryable<Entities.Post> queryable = new[] { new Entities.Post() }.AsQueryable();

            //Act
            var expected = queryable.Where(p => p.PostDate == null).ToArray();
            var result1 = queryable.Where("PostDate == null").ToArray();
            var result2 = queryable.Where("null == PostDate").ToArray();

            //Assert
            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        /// <summary>
        /// Defines the test method Where_Dynamic_Exceptions.
        /// </summary>
        [TestMethod]
        public void Where_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.ThrowsException<InvalidOperationException>(() => qry.Where("Id"));
            Assert.ThrowsException<ParseException>(() => qry.Where("Bad=3"));
            Assert.ThrowsException<ParseException>(() => qry.Where("Id=123"));

            Assert.ThrowsException<ArgumentNullException>(() => DynamicQueryExtensions.Where(null, "Id=1"));
            Assert.ThrowsException<ArgumentNullException>(() => qry.Where(null));
            Assert.ThrowsException<ArgumentNullException>(() => qry.Where(""));
            Assert.ThrowsException<ParseException>(() => qry.Where(" "));
        }

        /// <summary>
        /// Defines the test method Where_Dynamic_StringQuoted.
        /// </summary>
        //[TestMethod]
        public void Where_Dynamic_StringQuoted()
        {
            //Arrange
            var testList = User.GenerateSampleModels(2, allowNullableProfiles: true);
            testList[0].UserName = @"This \""is\"" a test.";
            var qry = testList.AsQueryable();

            //Act
            var result1a = qry.Where(@"UserName == ""This \""is\"" a test.""").ToArray();
            var result1b = qry.Where("UserName == \"This \\\"is\\\" a test.\"").ToArray();
            var result2 = qry.Where("UserName == @0", @"This \""is\"" a test.").ToArray();
            var expected = qry.Where(x => x.UserName == @"This \""is\"" a test.").ToArray();

            //Assert
            Assert.IsTrue(expected.Count() == 1);
            Assert.AreEqual(expected, result1a);
            Assert.AreEqual(expected, result1b);
            Assert.AreEqual(expected, result2);
        }

        /// <summary>
        /// Defines the test method Where_Dynamic_SelectNewObjects.
        /// </summary>
        //[TestMethod]
        public void Where_Dynamic_SelectNewObjects()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            var expectedResult = testList.Where(x => x.Income > 4000).Select(x => new { Id = x.Id, Income = x.Income + 1111 });
            var dynamicList = qry.Where("Income > @0", 4000).ToDynamicList();

            var newUsers = dynamicList.Select(x => new { Id = x.Id, Income = x.Income + 1111 });
            Assert.AreEqual(newUsers.Cast<object>().ToList(), expectedResult);
        }
    }
}
