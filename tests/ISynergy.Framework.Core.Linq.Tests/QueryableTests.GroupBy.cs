using NFluent;
using ISynergy.Framework.Core.Linq.Exceptions;
using System.Reflection;
using Xunit;
using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void GroupBy_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var byAgeReturnUserName = qry.GroupBy("Profile.Age", "UserName");
            var byAgeReturnAll = qry.GroupBy("Profile.Age");

            //Assert
            Assert.Equal(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnUserName.Count());
            Assert.Equal(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnAll.Count());
        }

        // https://github.com/StefH/System.Linq.Dynamic.Core/issues/75
        [Fact]
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

        [Fact]
        public void GroupBy_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<ParseException>(() => qry.GroupBy("Bad"));
            Assert.Throws<ParseException>(() => qry.GroupBy("Id, UserName"));
            Assert.Throws<ParseException>(() => qry.GroupBy("new Id, UserName"));
            Assert.Throws<ParseException>(() => qry.GroupBy("new (Id, UserName"));
            Assert.Throws<ParseException>(() => qry.GroupBy("new (Id, UserName, Bad)"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryExtensions.GroupBy((IQueryable<string>)null, "Id"));
            Assert.Throws<ArgumentNullException>(() => qry.GroupBy(null));
            Assert.Throws<ArgumentNullException>(() => qry.GroupBy(""));
            Assert.Throws<ParseException>(() => qry.GroupBy(" "));

            Assert.Throws<ArgumentNullException>(() => qry.GroupBy("Id", (string)null));
            Assert.Throws<ArgumentNullException>(() => qry.GroupBy("Id", ""));
            Assert.Throws<ParseException>(() => qry.GroupBy("Id", " "));
        }
    }
}
