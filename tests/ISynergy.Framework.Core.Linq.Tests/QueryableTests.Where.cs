﻿using System;
using System.Collections.Generic;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using ISynergy.Framework.Core.Linq.Parsers;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
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
            Assert.Equal(testList[10], userById.Single());
            Assert.Equal(testList[5], userByUserName.Single());
            Assert.Equal(testList.Count(x => x.Profile == null), nullProfileCount.Count());
            Assert.Equal(testList[1], userByFirstName.Single());
        }

        [Fact]
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

        [Theory]
        [InlineData("Fri, 10 May 2019 11:03:17 GMT", 11)]
        [InlineData("Fri, 10 May 2019 11:03:17 -07:00", 18)]
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
            Assert.Equal(1, result.Count());
        }

        /// <summary>
        /// https://github.com/StefH/System.Linq.Dynamic.Core/issues/19
        /// </summary>
        [Fact]
        public void Where_Dynamic_DateTime_NotEquals_Null()
        {
            //Arrange
            IQueryable<Entities.Post> queryable = new[] { new Entities.Post() }.AsQueryable();

            //Act
            var expected = queryable.Where(p => p.PostDate != null).ToArray();
            var result1 = queryable.Where("PostDate != null").ToArray();
            var result2 = queryable.Where("null != PostDate").ToArray();

            //Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        public void Where_Dynamic_DateTime_Equals_Null()
        {
            //Arrange
            IQueryable<Entities.Post> queryable = new[] { new Entities.Post() }.AsQueryable();

            //Act
            var expected = queryable.Where(p => p.PostDate == null).ToArray();
            var result1 = queryable.Where("PostDate == null").ToArray();
            var result2 = queryable.Where("null == PostDate").ToArray();

            //Assert
            Assert.Equal(expected, result1);
            Assert.Equal(expected, result2);
        }

        [Fact]
        public void Where_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<InvalidOperationException>(() => qry.Where("Id"));
            Assert.Throws<ParseException>(() => qry.Where("Bad=3"));
            Assert.Throws<ParseException>(() => qry.Where("Id=123"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryExtensions.Where(null, "Id=1"));
            Assert.Throws<ArgumentNullException>(() => qry.Where(null));
            Assert.Throws<ArgumentNullException>(() => qry.Where(""));
            Assert.Throws<ParseException>(() => qry.Where(" "));
        }

        [Fact]
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
            Assert.Single(expected);
            Assert.Equal(expected, result1a);
            Assert.Equal(expected, result1b);
            Assert.Equal(expected, result2);
        }

        [Fact]
        public void Where_Dynamic_SelectNewObjects()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            var expectedResult = testList.Where(x => x.Income > 4000).Select(x => new { Id = x.Id, Income = x.Income + 1111 });
            var dynamicList = qry.Where("Income > @0", 4000).ToDynamicList();

            var newUsers = dynamicList.Select(x => new { Id = x.Id, Income = x.Income + 1111 });
            Assert.Equal(newUsers.Cast<object>().ToList(), expectedResult);
        }
    }
}
