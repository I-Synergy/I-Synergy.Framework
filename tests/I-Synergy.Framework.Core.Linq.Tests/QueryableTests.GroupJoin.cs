﻿using System;
using System.Collections.Generic;
using System.Linq;
using ISynergy.Framework.Core.Linq.Exceptions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
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

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.Equal(realResult[i].NumberOfPets, dynamicResult[i].GetDynamicPropertyValue<int>("NumberOfPets"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        [Fact]
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

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.Equal(realResult[i].NumberOfPets, dynamicResult[i].GetDynamicPropertyValue<int>("NumberOfPets"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        public class Employee
        {
            // [Key]
            public int EmployeeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }

            public ICollection<Paycheck> Paychecks { get; set; } = new List<Paycheck>();
        }

        public class Paycheck
        {
            // [Key]
            public int PaycheckId { get; set; }
            public decimal HourlyWage { get; set; }
            public int HoursWorked { get; set; }
            public DateTimeOffset DateCreated { get; set; }

            // [ForeignKey("EmployeeId")]
            public Employee Employee { get; set; }
            public int EmployeeId { get; set; }
        }

        [Fact]
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

        [Fact]
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

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        [Fact]
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

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        [Fact]
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
