using NFluent;
using ISynergy.Framework.Core.Linq.Exceptions;
using Xunit;
using System;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Entities;
using ISynergy.Framework.Core.Linq.Parsers;
using System.Linq;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method OfType_WithType.
        /// </summary>
        [Fact]
        public void OfType_WithType()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "e" }, new Boss { Name = "e" }
            }.AsQueryable();

            // Act
            var oftype = qry.OfType<Worker>().ToArray();
            var oftypeDynamic = qry.OfType(typeof(Worker)).ToDynamicArray();

            // Assert
            Check.That(oftypeDynamic.Length).Equals(oftype.Length);
        }

        /// <summary>
        /// Defines the test method OfType_WithString.
        /// </summary>
        [Fact]
        public void OfType_WithString()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "e" }, new Boss { Name = "e" }
            }.AsQueryable();

            // Act
            var oftype = qry.OfType<Worker>().ToArray();
            var oftypeDynamic = qry.OfType(typeof(Worker).FullName).ToDynamicArray();

            // Assert
            Check.That(oftypeDynamic.Length).Equals(oftype.Length);
        }

        /// <summary>
        /// Defines the test method OfType_Dynamic.
        /// </summary>
        [Fact]
        public void OfType_Dynamic()
        {
            // Assign
            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "e" }, new Boss { Name = "e" }
                    }
                }
            }.AsQueryable();

            // Act
            var oftype = qry.Select(c => c.Employees.OfType<Worker>().Where(e => e.Name == "e")).ToArray();
            var oftypeDynamic = qry.Select("Employees.OfType(\"ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Worker\").Where(Name == \"e\")").ToDynamicArray();

            // Assert
            Check.That(oftypeDynamic.Length).Equals(oftype.Length);
        }

        /// <summary>
        /// Defines the test method Is_Dynamic_ActingOnIt.
        /// </summary>
        [Fact]
        public void Is_Dynamic_ActingOnIt()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            int countOfType = qry.Count(c => c is Worker);
            int countOfTypeDynamic = qry.Count("is(\"ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Worker\")");

            // Assert
            Check.That(countOfTypeDynamic).Equals(countOfType);
        }

        /// <summary>
        /// Defines the test method Is_Dynamic_ActingOnIt_WithSimpleName.
        /// </summary>
        [Fact]
        public void Is_Dynamic_ActingOnIt_WithSimpleName()
        {
            // Assign
            var config = new ParsingConfig
            {
                ResolveTypesBySimpleName = true
            };

            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            int countOfType = qry.Count(c => c is Worker);
            int countOfTypeDynamic = qry.Count(config, "is(\"Worker\")");

            // Assert
            Check.That(countOfTypeDynamic).Equals(countOfType);
        }

        /// <summary>
        /// Defines the test method As_Dynamic_ActingOnIt.
        /// </summary>
        [Fact]
        public void As_Dynamic_ActingOnIt()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            int countAsDynamic = qry.Count("As(\"ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Worker\") != null");

            // Assert
            Check.That(countAsDynamic).Equals(1);
        }

        /// <summary>
        /// Defines the test method CastToType_WithType.
        /// </summary>
        [Fact]
        public void CastToType_WithType()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Worker { Name = "2" }
            }.AsQueryable();

            // Act
            var cast = qry.Cast<Worker>().ToArray();
            var castDynamic = qry.Cast(typeof(Worker)).ToDynamicArray();

            // Assert
            Check.That(castDynamic.Length).Equals(cast.Length);
        }

        /// <summary>
        /// Defines the test method CastToType_WithString.
        /// </summary>
        [Fact]
        public void CastToType_WithString()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Worker { Name = "2" }
            }.AsQueryable();

            // Act
            var cast = qry.Cast<Worker>().ToArray();
            var castDynamic = qry.Cast(typeof(Worker).FullName).ToDynamicArray();

            // Assert
            Check.That(castDynamic.Length).Equals(cast.Length);
        }

        /// <summary>
        /// Defines the test method CastToType_Dynamic.
        /// </summary>
        [Fact]
        public void CastToType_Dynamic()
        {
            // Assign
            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "e" }
                    }
                }
            }.AsQueryable();

            // Act
            var cast = qry.Select(c => c.Employees.Cast<Worker>().Where(e => e.Name == "e")).ToArray();
            var castDynamic = qry.Select("Employees.Cast(\"ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Worker\").Where(Name == \"e\")").ToDynamicArray();

            // Assert
            Check.That(cast.Length).Equals(castDynamic.Length);
        }

        /// <summary>
        /// Defines the test method CastToType_Dynamic_ActingOnIt.
        /// </summary>
        [Fact]
        public void CastToType_Dynamic_ActingOnIt()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Worker { Name = "2" }
            }.AsQueryable();

            // Act
            var cast = qry.Select(c => (Worker)c).ToArray();
            var castDynamic = qry.Select("Cast(\"ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Worker\")").ToDynamicArray();

            // Assert
            Check.That(cast.Length).Equals(castDynamic.Length);
        }

        /// <summary>
        /// Defines the test method CastToType_Dynamic_ActingOnIt_Throws.
        /// </summary>
        [Fact]
        public void CastToType_Dynamic_ActingOnIt_Throws()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            Action castDynamic = () => qry.Select("Cast(\"ISynergy.Framework.Core.Linq.Extensions.Tests.Entities.Worker\")").ToDynamicArray();

            // Assert
            Check.ThatCode(castDynamic).Throws<Exception>();
        }

        /// <summary>
        /// Defines the test method OfType_Dynamic_Exceptions.
        /// </summary>
        [Fact]
        public void OfType_Dynamic_Exceptions()
        {
            // Assign
            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "e" }, new Boss { Name = "e" }
                    }
                }
            }.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.Select("Employees.OfType().Where(Name == \"e\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.OfType(true).Where(Name == \"e\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.OfType(\"not-found\").Where(Name == \"e\")"));
        }

        /// <summary>
        /// Defines the test method OfType_Dynamic_ActingOnIt_Exceptions.
        /// </summary>
        [Fact]
        public void OfType_Dynamic_ActingOnIt_Exceptions()
        {
            // Assign
            var qry = new BaseEmployee[]
            {
                new Worker { Name = "1" }, new Boss { Name = "b" }
            }.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.Count("OfType()"));
            Assert.Throws<ParseException>(() => qry.Count("OfType(true)"));
            Assert.Throws<ParseException>(() => qry.Count("OfType(\"not-found\")"));
        }

        /// <summary>
        /// Defines the test method CastToType_Dynamic_Exceptions.
        /// </summary>
        [Fact]
        public void CastToType_Dynamic_Exceptions()
        {
            // Assign
            var qry = new[]
            {
                new CompanyWithBaseEmployees
                {
                    Employees = new BaseEmployee[]
                    {
                        new Worker { Name = "1" }, new Worker { Name = "2" }
                    }
                }
            }.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.Select("Employees.Cast().Where(Name == \"1\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.Cast(true).Where(Name == \"1\")"));
            Assert.Throws<ParseException>(() => qry.Select("Employees.Cast(\"not-found\").Where(Name == \"1\")"));
        }
    }
}
