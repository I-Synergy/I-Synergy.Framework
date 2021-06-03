using System;
using System.Collections.Generic;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method GroupJoin_1.
        /// </summary>
        [TestMethod]
        public void GroupJoin_1()
        {
            //Arrange
            Person magnus = new Person { Name = "Hedlund, Magnus" };
            Person terry = new Person { Name = "Adams, Terry" };
            Person charlotte = new Person { Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person,
                pet => pet.Owner,
                (person, pets) => new { OwnerName = person.Name, Pets = pets, NumberOfPets = pets.Count() });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it",
                "Owner",
                "new(outer.Name as OwnerName, inner as Pets, inner.Count() as NumberOfPets)");

            //Assert
            var realResult = realQuery.ToArray();

            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.AreEqual(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.AreEqual(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.AreEqual(realResult[i].NumberOfPets, dynamicResult[i].GetDynamicPropertyValue<int>("NumberOfPets"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.AreEqual(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        /// <summary>
        /// Defines the test method GroupJoin_2.
        /// </summary>
        [TestMethod]
        public void GroupJoin_2()
        {
            //Arrange
            Person magnus = new Person { Name = "Hedlund, Magnus" };
            Person terry = new Person { Name = "Adams, Terry" };
            Person charlotte = new Person { Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person.Id,
                pet => pet.OwnerId,
                (person, pets) => new { OwnerName = person.Name, Pets = pets, NumberOfPets = pets.Count() });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it.Id",
                "OwnerId",
                "new(outer.Name as OwnerName, inner as Pets, inner.Count() as NumberOfPets)");

            //Assert
            var realResult = realQuery.ToArray();

            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.AreEqual(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.AreEqual(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.AreEqual(realResult[i].NumberOfPets, dynamicResult[i].GetDynamicPropertyValue<int>("NumberOfPets"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.AreEqual(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        /// <summary>
        /// Class Employee.
        /// </summary>
        public class Employee
        {
            // [Key]
            /// <summary>
            /// Gets or sets the employee identifier.
            /// </summary>
            /// <value>The employee identifier.</value>
            public int EmployeeId { get; set; }
            /// <summary>
            /// Gets or sets the first name.
            /// </summary>
            /// <value>The first name.</value>
            public string FirstName { get; set; }
            /// <summary>
            /// Gets or sets the last name.
            /// </summary>
            /// <value>The last name.</value>
            public string LastName { get; set; }
            /// <summary>
            /// Gets or sets the age.
            /// </summary>
            /// <value>The age.</value>
            public int Age { get; set; }

            /// <summary>
            /// Gets or sets the paychecks.
            /// </summary>
            /// <value>The paychecks.</value>
            public ICollection<Paycheck> Paychecks { get; set; } = new List<Paycheck>();
        }

        /// <summary>
        /// Class Paycheck.
        /// </summary>
        public class Paycheck
        {
            // [Key]
            /// <summary>
            /// Gets or sets the paycheck identifier.
            /// </summary>
            /// <value>The paycheck identifier.</value>
            public int PaycheckId { get; set; }
            /// <summary>
            /// Gets or sets the hourly wage.
            /// </summary>
            /// <value>The hourly wage.</value>
            public decimal HourlyWage { get; set; }
            /// <summary>
            /// Gets or sets the hours worked.
            /// </summary>
            /// <value>The hours worked.</value>
            public int HoursWorked { get; set; }
            /// <summary>
            /// Gets or sets the date created.
            /// </summary>
            /// <value>The date created.</value>
            public DateTimeOffset DateCreated { get; set; }

            // [ForeignKey("EmployeeId")]
            /// <summary>
            /// Gets or sets the employee.
            /// </summary>
            /// <value>The employee.</value>
            public Employee Employee { get; set; }
            /// <summary>
            /// Gets or sets the employee identifier.
            /// </summary>
            /// <value>The employee identifier.</value>
            public int EmployeeId { get; set; }
        }

        /// <summary>
        /// Defines the test method GroupJoin_3.
        /// </summary>
        [TestMethod]
        public void GroupJoin_3()
        {
            // Arrange
            var employees = new List<Employee>();
            var paychecks = new List<Paycheck>();

            // Act
            var realQuery = employees.AsQueryable().GroupJoin(
                paychecks,
                employee => employee.EmployeeId,
                paycheck => paycheck.EmployeeId,
                (emp, paycheckList) => new { Name = emp.FirstName + " " + emp.LastName, NumberOfPaychecks = paycheckList.Count() });

            var dynamicQuery = employees.AsQueryable().GroupJoin(
                paychecks,
                "it.EmployeeId",
                "EmployeeId",
                "new(outer.FirstName + \" \" + outer.LastName as Name, inner.Count() as NumberOfPaychecks)");

            // Assert
            var realResult = realQuery.ToArray();
            Check.That(realResult).IsNotNull();

            var dynamicResult = dynamicQuery.ToDynamicArray();
            Check.That(dynamicResult).IsNotNull();
        }

        /// <summary>
        /// Defines the test method GroupJoinOnNullableType_RightNullable.
        /// </summary>
        [TestMethod]
        public void GroupJoinOnNullableType_RightNullable()
        {
            //Arrange
            Person magnus = new Person { Id = 1, Name = "Hedlund, Magnus" };
            Person terry = new Person { Id = 2, Name = "Adams, Terry" };
            Person charlotte = new Person { Id = 3, Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", NullableOwnerId = terry.Id };
            Pet boots = new Pet { Name = "Boots", NullableOwnerId = terry.Id };
            Pet whiskers = new Pet { Name = "Whiskers", NullableOwnerId = charlotte.Id };
            Pet daisy = new Pet { Name = "Daisy", NullableOwnerId = magnus.Id };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person.Id,
                pet => pet.NullableOwnerId,
                (person, pets) => new { OwnerName = person.Name, Pets = pets });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it.Id",
                "NullableOwnerId",
                "new(outer.Name as OwnerName, inner as Pets)");

            //Assert
            var realResult = realQuery.ToArray();
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.AreEqual(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.AreEqual(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.AreEqual(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        /// <summary>
        /// Defines the test method GroupJoinOnNullableType_LeftNullable.
        /// </summary>
        [TestMethod]
        public void GroupJoinOnNullableType_LeftNullable()
        {
            //Arrange
            Person magnus = new Person { NullableId = 1, Name = "Hedlund, Magnus" };
            Person terry = new Person { NullableId = 2, Name = "Adams, Terry" };
            Person charlotte = new Person { NullableId = 3, Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", OwnerId = terry.Id };
            Pet boots = new Pet { Name = "Boots", OwnerId = terry.Id };
            Pet whiskers = new Pet { Name = "Whiskers", OwnerId = charlotte.Id };
            Pet daisy = new Pet { Name = "Daisy", OwnerId = magnus.Id };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person.NullableId,
                pet => pet.OwnerId,
                (person, pets) => new { OwnerName = person.Name, Pets = pets });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it.NullableId",
                "OwnerId",
                "new(outer.Name as OwnerName, inner as Pets)");

            //Assert
            var realResult = realQuery.ToArray();
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.AreEqual(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.AreEqual(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.AreEqual(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        /// <summary>
        /// Defines the test method GroupJoinOnNullableType_NotSameTypesThrowsException.
        /// </summary>
        [TestMethod]
        public void GroupJoinOnNullableType_NotSameTypesThrowsException()
        {
            var person = new Person { Id = 1, Name = "Hedlund, Magnus" };
            var people = new List<Person> { person };
            var pets = new List<Pet> { new Pet { Name = "Daisy", OwnerId = person.Id } };

            Check.ThatCode(() =>
                people.AsQueryable()
                    .GroupJoin(
                        pets,
                        "it.Id",
                        "Name", // This is wrong
                        "new(outer.Name as OwnerName, inner as Pets)")).Throws<ParseException>();
        }
    }
}
