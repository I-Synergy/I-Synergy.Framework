using System.Collections;
using System.Collections.Generic;
using ISynergy.Framework.Core.Linq.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using ISynergy.Framework.Core.Linq.Parsers;
using Microsoft.AspNetCore.Identity;
using QueryInterceptor.Core;
using Linq.PropertyTranslator.Core;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Class Example.
        /// </summary>
        public class Example
        {
            /// <summary>
            /// Gets or sets the time.
            /// </summary>
            /// <value>The time.</value>
            public DateTime Time { get; set; }
            /// <summary>
            /// Gets or sets the time null.
            /// </summary>
            /// <value>The time null.</value>
            public DateTime? TimeNull { get; set; }
            /// <summary>
            /// Gets or sets the dow null.
            /// </summary>
            /// <value>The dow null.</value>
            public DayOfWeek? DOWNull { get; set; }
            /// <summary>
            /// Gets or sets the dow.
            /// </summary>
            /// <value>The dow.</value>
            public DayOfWeek DOW { get; set; }
            /// <summary>
            /// Gets or sets the sec.
            /// </summary>
            /// <value>The sec.</value>
            public int Sec { get; set; }
            /// <summary>
            /// Gets or sets the sec null.
            /// </summary>
            /// <value>The sec null.</value>
            public int? SecNull { get; set; }

            /// <summary>
            /// Class NestedDto.
            /// </summary>
            public class NestedDto
            {
                /// <summary>
                /// Gets or sets the name.
                /// </summary>
                /// <value>The name.</value>
                public string Name { get; set; }

                /// <summary>
                /// Class NestedDto2.
                /// </summary>
                public class NestedDto2
                {
                    /// <summary>
                    /// Gets or sets the name2.
                    /// </summary>
                    /// <value>The name2.</value>
                    public string Name2 { get; set; }
                }
            }
        }

        /// <summary>
        /// Class ExampleWithConstructor.
        /// </summary>
        public class ExampleWithConstructor
        {
            /// <summary>
            /// Gets or sets the time.
            /// </summary>
            /// <value>The time.</value>
            public DateTime Time { get; set; }
            /// <summary>
            /// Gets or sets the dow null.
            /// </summary>
            /// <value>The dow null.</value>
            public DayOfWeek? DOWNull { get; set; }
            /// <summary>
            /// Gets or sets the dow.
            /// </summary>
            /// <value>The dow.</value>
            public DayOfWeek DOW { get; set; }
            /// <summary>
            /// Gets or sets the sec.
            /// </summary>
            /// <value>The sec.</value>
            public int Sec { get; set; }
            /// <summary>
            /// Gets or sets the sec null.
            /// </summary>
            /// <value>The sec null.</value>
            public int? SecNull { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ExampleWithConstructor"/> class.
            /// </summary>
            /// <param name="t">The t.</param>
            /// <param name="dn">The dn.</param>
            /// <param name="d">The d.</param>
            /// <param name="s">The s.</param>
            /// <param name="sn">The sn.</param>
            public ExampleWithConstructor(DateTime t, DayOfWeek? dn, DayOfWeek d, int s, int? sn)
            {
                Time = t;
                DOWNull = dn;
                DOW = d;
                Sec = s;
                SecNull = sn;
            }
        }

        /// <summary>
        /// Cannot work with property which in base class. https://github.com/StefH/System.Linq.Dynamic.Core/issues/23
        /// </summary>
        //[TestMethod]
        public void Select_Dynamic_PropertyInBaseClass()
        {
            var queryable = new[] { new IdentityUser("a"), new IdentityUser("b") }.AsQueryable();

            var expected = queryable.Select(i => i.Id);
            var dynamic = queryable.Select<string>("Id");

            Assert.AreEqual(expected.ToArray(), dynamic.ToArray());
        }

        /// <summary>
        /// Defines the test method Select_Dynamic1.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic1()
        {
            // Assign
            var qry = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var userRoles1 = qry.Select("new (Roles.Select(Id) as RoleIds)");
            var userRoles2 = qry.Select("new (Roles.Select(it.Id) as RoleIds)");

            // Assert
            Check.That(userRoles1.Count()).IsEqualTo(1);
            Check.That(userRoles2.Count()).IsEqualTo(1);
        }

        /// <summary>
        /// Defines the test method Select_Dynamic2.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic2()
        {
            // Assign
            var qry = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var userRoles = qry.Select("new (Roles.Select(it).ToArray() as Rolez)");

            // Assert
            Check.That(userRoles.Count()).IsEqualTo(1);
        }

        /// <summary>
        /// Defines the test method Select_Dynamic3.
        /// </summary>
        //[TestMethod]
        public void Select_Dynamic3()
        {
            //Arrange
            List<int> range = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            IEnumerable rangeResult = range.AsQueryable().Select("it * it");
            var userNames = qry.Select("UserName");
            var userFirstName = qry.Select("new (UserName, Profile.FirstName as MyFirstName)");
            var userRoles = qry.Select("new (UserName, Roles.Select(Id) AS RoleIds)");

            //Assert
            Assert.AreEqual(range.Select(x => x * x).ToArray(), rangeResult.Cast<int>().ToArray());

#if NET35
            Assert.AreEqual(testList.Select(x => x.UserName).ToArray(), userNames.AsEnumerable().Cast<string>().ToArray());
            Assert.AreEqual(
                testList.Select(x => "{ UserName = " + x.UserName + ", MyFirstName = " + x.Profile.FirstName + " }").ToArray(),
                userFirstName.Cast<object>().Select(x => x.ToString()).ToArray());
            Assert.AreEqual(testList[0].Roles.Select(x => x.Id).ToArray(), Enumerable.ToArray(userRoles.First().GetDynamicProperty<IEnumerable<Guid>>("RoleIds")));
#else
            Assert.AreEqual(testList.Select(x => x.UserName).ToArray(), userNames.Cast<string>().ToArray());
            Assert.AreEqual(
                testList.Select(x => "{ UserName = " + x.UserName + ", MyFirstName = " + x.Profile.FirstName + " }").ToArray(),
                userFirstName.AsEnumerable().Select(x => x.ToString()).Cast<string>().ToArray());
            Assert.AreEqual(testList[0].Roles.Select(x => x.Id).ToArray(), Enumerable.ToArray(userRoles.First().RoleIds));
#endif
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_Add_Integers.
        /// </summary>
        //[TestMethod]
        public void Select_Dynamic_Add_Integers()
        {
            // Arrange
            var range = new List<int> { 1, 2 };

            // Act
            IEnumerable rangeResult = range.AsQueryable().Select("it + 1");

            // Assert
            Assert.AreEqual(range.Select(x => x + 1).ToArray(), rangeResult.Cast<int>().ToArray());
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_Add_Strings.
        /// </summary>
        //[TestMethod]
        public void Select_Dynamic_Add_Strings()
        {
            // Arrange
            var range = new List<string> { "a", "b" };

            // Act
            IEnumerable rangeResult = range.AsQueryable().Select("it + \"c\"");

            // Assert
            Assert.AreEqual(range.Select(x => x + "c").ToArray(), rangeResult.Cast<string>().ToArray());
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_WithIncludes.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_WithIncludes()
        {
            // Arrange
            var qry = new List<Entities.Employee>().AsQueryable();

            // Act
            string includesX =
                ", it.Company as TEntity__Company, it.Company.MainCompany as TEntity__Company_MainCompany, it.Country as TEntity__Country, it.Function as TEntity__Function, it.SubFunction as TEntity__SubFunction";
            string select = $"new (\"__Key__\" as __Key__, it AS TEntity__{includesX})";

            var userNames = qry.Select(select).ToDynamicList();

            // Assert
            Assert.IsNotNull(userNames);
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_WithPropertyVisitorAndQueryInterceptor.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_WithPropertyVisitorAndQueryInterceptor()
        {
            var testList = new List<Entities.Employee>
            {
                new Entities.Employee {FirstName = "first", LastName = "last"}
            };
            var qry = testList.AsEnumerable().AsQueryable().InterceptWith(new PropertyVisitor());

            var dynamicSelect = qry.Select("new (FirstName, LastName, FullName)").ToDynamicList();
            Assert.IsNotNull(dynamicSelect);
            Assert.IsTrue(dynamicSelect.Count == 1);

            var firstEmployee = dynamicSelect.FirstOrDefault();
            Assert.IsNotNull(firstEmployee);

            Assert.AreEqual("first last", firstEmployee.FullName);
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_TResult.
        /// </summary>
        //[TestMethod]
        public void Select_Dynamic_TResult()
        {
            //Arrange
            List<int> range = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            IEnumerable rangeResult = range.AsQueryable().Select<int>("it * it").ToList();
            var userNames = qry.Select<string>("UserName").ToList();
            var userProfiles = qry.Select<UserProfile>("Profile").ToList();

            //Assert
            Assert.AreEqual(range.Select(x => x * x).ToList(), rangeResult);
            Assert.AreEqual(testList.Select(x => x.UserName).ToList(), userNames);
            Assert.AreEqual(testList.Select(x => x.Profile).ToList(), userProfiles);
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_IntoType.
        /// </summary>
        //[TestMethod]
        public void Select_Dynamic_IntoType()
        {
            //Arrange
            List<int> range = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testList = User.GenerateSampleModels(10);
            var qry = testList.AsQueryable();

            //Act
            IEnumerable rangeResult = range.AsQueryable().Select(typeof(int), "it * it");
            var userNames = qry.Select(typeof(string), "UserName");
            var userProfiles = qry.Select(typeof(UserProfile), "Profile");

            //Assert
            Assert.AreEqual(range.Select(x => x * x).Cast<object>().ToList(), rangeResult.ToDynamicList());
            Assert.AreEqual(testList.Select(x => x.UserName).Cast<object>().ToList(), userNames.ToDynamicList());
            Assert.AreEqual(testList.Select(x => x.Profile).Cast<object>().ToList(), userProfiles.ToDynamicList());
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_IntoTypeWithNullableProperties1.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_IntoTypeWithNullableProperties1()
        {
            // Arrange
            var dates = Enumerable.Repeat(0, 7)
                    .Select((d, i) => new DateTime(2000, 1, 1).AddDays(i).AddSeconds(i))
                    .AsQueryable();
            var config = new ParsingConfig { SupportEnumerationsFromSystemNamespace = false };

            // Act
            IQueryable<Example> result = dates
                .Select(d => new Example { Time = d, DOWNull = d.DayOfWeek, DOW = d.DayOfWeek, Sec = d.Second, SecNull = d.Second });
            IQueryable<Example> resultDynamic = dates
                .Select<Example>(config, "new (it as Time, DayOfWeek as DOWNull, DayOfWeek as DOW, Second as Sec, int?(Second) as SecNull)");

            // Assert
            Check.That(resultDynamic.First()).Equals(result.First());
            Check.That(resultDynamic.Last()).Equals(result.Last());
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_IntoTypeWithNullableProperties2.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_IntoTypeWithNullableProperties2()
        {
            // Arrange
            var dates = Enumerable.Repeat(0, 7)
                    .Select((d, i) => new DateTime(2000, 1, 1).AddDays(i).AddSeconds(i))
                    .AsQueryable();
            var config = new ParsingConfig { SupportEnumerationsFromSystemNamespace = false };

            // Act
            IQueryable<ExampleWithConstructor> result = dates
                .Select(d => new ExampleWithConstructor(d, d.DayOfWeek, d.DayOfWeek, d.Second, d.Second));
            IQueryable<ExampleWithConstructor> resultDynamic = dates
                .Select<ExampleWithConstructor>(config, "new (it as Time, DayOfWeek as DOWNull, DayOfWeek as DOW, Second as Sec, int?(Second) as SecNull)");

            // Assert
            Check.That(resultDynamic.First()).Equals(result.First());
            Check.That(resultDynamic.Last()).Equals(result.Last());
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_IntoKnownNestedType.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_IntoKnownNestedType()
        {
            var config = new ParsingConfig { AllowNewToEvaluateAnyType = true };
#if NETCOREAPP
            // config.CustomTypeProvider = new NetStandardCustomTypeProvider();
#endif
            // Assign
            var queryable = new List<string>() { "name1", "name2" }.AsQueryable();

            // Act
            var projectedData = queryable.Select<Example.NestedDto>(config, $"new {typeof(Example.NestedDto).FullName}(~ as Name)");

            // Assert
            Check.That(projectedData.First().Name).Equals("name1");
            Check.That(projectedData.Last().Name).Equals("name2");
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_IntoKnownNestedTypeSecondLevel.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_IntoKnownNestedTypeSecondLevel()
        {
            var config = new ParsingConfig { AllowNewToEvaluateAnyType = true };
#if NETCOREAPP
            // config.CustomTypeProvider = new NetStandardCustomTypeProvider();
#endif

            // Assign
            var queryable = new List<string>() { "name1", "name2" }.AsQueryable();

            // Act
            var projectedData = queryable.Select<Example.NestedDto.NestedDto2>(config, $"new {typeof(Example.NestedDto.NestedDto2).FullName}(~ as Name2)");

            // Assert
            Check.That(projectedData.First().Name2).Equals("name1");
            Check.That(projectedData.Last().Name2).Equals("name2");
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_RenameParameterExpression_Is_False.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_RenameParameterExpression_Is_False()
        {
            // Arrange
            var config = new ParsingConfig
            {
                RenameParameterExpression = false
            };
            var queryable = new int[0].AsQueryable();

            // Act
            string result = queryable.Select<int>(config, "it * it").ToString();

            // Assert
            Check.That(result).Equals("System.Int32[].Select(Param_0 => (Param_0 * Param_0))");
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_RenameParameterExpression_Is_True.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_RenameParameterExpression_Is_True()
        {
            // Arrange
            var config = new ParsingConfig
            {
                RenameParameterExpression = true
            };
            var queryable = new int[0].AsQueryable();

            // Act
            string result = queryable.Select<int>(config, "it * it").ToString();

            // Assert
            Check.That(result).Equals("System.Int32[].Select(it => (it * it))");
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_ReservedKeyword.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_ReservedKeyword()
        {
            // Arrange
            var queryable = new[] { new { Id = 1, Guid = "a" } }.AsQueryable();

            // Act
            var result = queryable.Select("new (Id, @Guid, 42 as Answer)").ToDynamicArray();

            // Assert
            Check.That(result).IsNotNull();
        }

        /// <summary>
        /// Defines the test method Select_Dynamic_Exceptions.
        /// </summary>
        [TestMethod]
        public void Select_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.ThrowsException<ParseException>(() => qry.Select("Bad"));
            Assert.ThrowsException<ParseException>(() => qry.Select("Id, UserName"));
            Assert.ThrowsException<ParseException>(() => qry.Select("new Id, UserName"));
            Assert.ThrowsException<ParseException>(() => qry.Select("new (Id, UserName"));
            Assert.ThrowsException<ParseException>(() => qry.Select("new (Id, UserName, Bad)"));

            Assert.ThrowsException<ArgumentNullException>(() => DynamicQueryExtensions.Select(null, "Id"));
            Assert.ThrowsException<ArgumentNullException>(() => qry.Select(null));
            Assert.ThrowsException<ArgumentNullException>(() => qry.Select(""));
            Assert.ThrowsException<ParseException>(() => qry.Select(" "));
        }
    }
}
