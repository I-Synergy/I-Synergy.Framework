using System.Collections.Generic;
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
        /// Defines the test method Any.
        /// </summary>
        [TestMethod]
        public void Any()
        {
            //Arrange
            IQueryable testListFull = User.GenerateSampleModels(100).AsQueryable();
            IQueryable testListOne = User.GenerateSampleModels(1).AsQueryable();
            IQueryable testListNone = User.GenerateSampleModels(0).AsQueryable();

            //Act
            var resultFull = testListFull.Any();
            var resultOne = testListOne.Any();
            var resultNone = testListNone.Any();

            //Assert
            Assert.IsTrue(resultFull);
            Assert.IsTrue(resultOne);
            Assert.IsFalse(resultNone);
        }

        /// <summary>
        /// Defines the test method Any_Predicate.
        /// </summary>
        [TestMethod]
        public void Any_Predicate()
        {
            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            bool expected = queryable.Any(u => u.Income > 50);
            bool result = queryable.Any("Income > 50");

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method Any_Predicate_WithArgs.
        /// </summary>
        [TestMethod]
        public void Any_Predicate_WithArgs()
        {
            const int value = 50;

            //Arrange
            var queryable = User.GenerateSampleModels(100).AsQueryable();

            //Act
            bool expected = queryable.Any(u => u.Income > value);
            bool result = queryable.Any("Income > @0", value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method Any_Dynamic_Select.
        /// </summary>
        //[TestMethod]
        public void Any_Dynamic_Select()
        {
            // Arrange
            IQueryable<User> queryable = User.GenerateSampleModels(1).AsQueryable();

            // Act
            var expected = queryable.Select(x => x.Roles.Any()).ToArray();
            var result = queryable.Select("Roles.Any()").ToDynamicArray<bool>();

            // Assert
            Assert.AreEqual(expected, result);
        }

        /// <summary>
        /// Defines the test method Any_Dynamic_Where.
        /// </summary>
        //[TestMethod]
        public void Any_Dynamic_Where()
        {
            const string search = "e";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.Any(r => r.Name.Contains(search))).ToArray();
            var result = queryable.Where("Roles.Any(Name.Contains(@0))", search).ToArray();

            Assert.AreEqual(expected, result);
        }

        // https://dynamiclinq.codeplex.com/discussions/654313
        /// <summary>
        /// Defines the test method Any_Dynamic_Where_Nested.
        /// </summary>
        [TestMethod]
        public void Any_Dynamic_Where_Nested()
        {
            const string search = "a";

            // Arrange
            var testList = User.GenerateSampleModels(10);
            var queryable = testList.AsQueryable();

            // Act
            var expected = queryable.Where(u => u.Roles.Any(r => r.Permissions.Any(p => p.Name.Contains(search)))).ToArray();
            var result = queryable.Where("Roles.Any(Permissions.Any(Name.Contains(@0)))", search).ToArray();

            Assert.AreEqual(expected, result);
        }

        // http://stackoverflow.com/questions/30846189/nested-any-in-is-not-working-in-dynamic-linq
        /// <summary>
        /// Defines the test method Any_Dynamic_Where_Nested2.
        /// </summary>
        //[TestMethod]
        public void Any_Dynamic_Where_Nested2()
        {
            // arrange
            var list = new List<A>
            {
                new A {Bs = new List<B> {new B {A = new A(), Cs = new List<C> {new C {B = new B()}}}}}
            };
            var queryable = list.AsQueryable();

            // act : 1
            var result1 = queryable.Where("(Name = \"\") && (Bs.Any(Cs.Any()))").ToList();
            var expected1 = queryable.Where(a => a.Name == "" && a.Bs.Any(b => b.Cs.Any()));
            Assert.AreEqual(expected1, result1);

            // act : 2
            var result2 = queryable.Where("(Bs.Any(Cs.Any())) && (Name = \"\")").ToList();
            var expected2 = queryable.Where(a => a.Bs.Any(b => b.Cs.Any() && a.Name == ""));
            Assert.AreEqual(expected2, result2);
        }

        /// <summary>
        /// Class A.
        /// </summary>
        class A
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the bs.
            /// </summary>
            /// <value>The bs.</value>
            public IList<B> Bs
            {
                get { return bs; }
                set { bs = value; }
            }
            /// <summary>
            /// The bs
            /// </summary>
            private IList<B> bs = new List<B>(0);
        }

        /// <summary>
        /// Class B.
        /// </summary>
        class B
        {
            /// <summary>
            /// Gets or sets a.
            /// </summary>
            /// <value>a.</value>
            public A A { get; set; }
            /// <summary>
            /// Gets or sets the cs.
            /// </summary>
            /// <value>The cs.</value>
            public IList<C> Cs
            {
                get { return cs; }
                set { cs = value; }
            }
            /// <summary>
            /// The cs
            /// </summary>
            private IList<C> cs = new List<C>(0);
        }

        /// <summary>
        /// Class C.
        /// </summary>
        class C
        {
            /// <summary>
            /// Gets or sets the b.
            /// </summary>
            /// <value>The b.</value>
            public B B { get; set; }
        }
    }
}
