using System.Linq;
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
        /// Defines the test method OrderByDescending_Dynamic.
        /// </summary>
        //[TestMethod]
        public void OrderByDescending_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var orderByIdDesc = qry.OrderBy("Id DESC");
            var orderByAgeDesc = qry.OrderBy("Profile.Age DESC");
            var orderByComplex2 = qry.OrderBy("Profile.Age DESC, Id");

            //Assert
            Assert.AreEqual(testList.OrderByDescending(x => x.Id).ToArray(), orderByIdDesc.ToArray());
            Assert.AreEqual(testList.OrderByDescending(x => x.Profile.Age).ToArray(), orderByAgeDesc.ToArray());
            Assert.AreEqual(testList.OrderByDescending(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex2.ToArray());
        }

        /// <summary>
        /// Defines the test method OrderByDescending_Dynamic_AsStringExpression.
        /// </summary>
        //[TestMethod]
        public void OrderByDescending_Dynamic_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var expectedDesc = qry.SelectMany(x => x.Roles.OrderByDescending(y => y.Name)).Select(x => x.Name);
            var orderByIdDesc = qry.SelectMany("Roles.OrderByDescending(Name)").Select("Name");

            //Assert
            Assert.AreEqual(expectedDesc.ToArray(), orderByIdDesc.Cast<string>().ToArray());
        }
    }
}
