using NFluent;
using System.Collections.Generic;
using ISynergy.Framework.Core.Linq.Exceptions;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using ISynergy.Framework.Core.Linq.Attributes;
using System;
using ISynergy.Framework.Core.Linq.Providers;
using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Parsers;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using System.Linq;
using Newtonsoft.Json;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class DynamicExpressionParserTests.
    /// </summary>
    public class DynamicExpressionParserTests
    {
        /// <summary>
        /// Class MyClass.
        /// </summary>
        private class MyClass
        {
            /// <summary>
            /// Fooes this instance.
            /// </summary>
            /// <returns>System.Int32.</returns>
            public int Foo()
            {
                return 42;
            }
        }

        /// <summary>
        /// Class ComplexParseLambda1Result.
        /// </summary>
        private class ComplexParseLambda1Result
        {
            /// <summary>
            /// The age
            /// </summary>
            public int? Age;
            /// <summary>
            /// The total income
            /// </summary>
            public int TotalIncome;
            /// <summary>
            /// The name
            /// </summary>
            public string Name = string.Empty;
        }

        /// <summary>
        /// Class ComplexParseLambda3Result.
        /// </summary>
        [DynamicLinqType]
        public class ComplexParseLambda3Result
        {
            /// <summary>
            /// Gets or sets the age.
            /// </summary>
            /// <value>The age.</value>
            public int? Age { get; set; }
            /// <summary>
            /// Gets or sets the total income.
            /// </summary>
            /// <value>The total income.</value>
            public int TotalIncome { get; set; }
        }

        /// <summary>
        /// Class CustomClassWithStaticMethod.
        /// </summary>
        public class CustomClassWithStaticMethod
        {
            /// <summary>
            /// Gets the age.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <returns>System.Int32.</returns>
            public static int GetAge(int x) => x;
        }

        /// <summary>
        /// Class CustomTextClass.
        /// </summary>
        public class CustomTextClass
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomTextClass"/> class.
            /// </summary>
            /// <param name="origin">The origin.</param>
            public CustomTextClass(string origin)
            {
                Origin = origin;
            }

            /// <summary>
            /// Gets the origin.
            /// </summary>
            /// <value>The origin.</value>
            public string Origin { get; }

            /// <summary>
            /// Performs an implicit conversion from <see cref="CustomTextClass"/> to <see cref="System.String"/>.
            /// </summary>
            /// <param name="customTextValue">The custom text value.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator string(CustomTextClass customTextValue)
            {
                return customTextValue?.Origin;
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="CustomTextClass"/>.
            /// </summary>
            /// <param name="origin">The origin.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator CustomTextClass(string origin)
            {
                return new CustomTextClass(origin);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Origin;
            }
        }

        /// <summary>
        /// Class CustomClassWithOneWayImplicitConversion.
        /// </summary>
        public class CustomClassWithOneWayImplicitConversion
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomClassWithOneWayImplicitConversion"/> class.
            /// </summary>
            /// <param name="origin">The origin.</param>
            public CustomClassWithOneWayImplicitConversion(string origin)
            {
                Origin = origin;
            }

            /// <summary>
            /// Gets the origin.
            /// </summary>
            /// <value>The origin.</value>
            public string Origin { get; }

            /// <summary>
            /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="CustomClassWithOneWayImplicitConversion"/>.
            /// </summary>
            /// <param name="origin">The origin.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator CustomClassWithOneWayImplicitConversion(string origin)
            {
                return new CustomClassWithOneWayImplicitConversion(origin);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Origin;
            }
        }

        /// <summary>
        /// Class CustomClassWithReversedImplicitConversion.
        /// </summary>
        public class CustomClassWithReversedImplicitConversion
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomClassWithReversedImplicitConversion"/> class.
            /// </summary>
            /// <param name="origin">The origin.</param>
            public CustomClassWithReversedImplicitConversion(string origin)
            {
                Origin = origin;
            }

            /// <summary>
            /// Gets the origin.
            /// </summary>
            /// <value>The origin.</value>
            public string Origin { get; }

            /// <summary>
            /// Performs an implicit conversion from <see cref="CustomClassWithReversedImplicitConversion"/> to <see cref="System.String"/>.
            /// </summary>
            /// <param name="origin">The origin.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator string(CustomClassWithReversedImplicitConversion origin)
            {
                return origin.ToString();
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Origin;
            }
        }

        /// <summary>
        /// Class CustomClassWithValueTypeImplicitConversion.
        /// </summary>
        public class CustomClassWithValueTypeImplicitConversion
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomClassWithValueTypeImplicitConversion"/> class.
            /// </summary>
            /// <param name="origin">The origin.</param>
            public CustomClassWithValueTypeImplicitConversion(int origin)
            {
                Origin = origin;
            }

            /// <summary>
            /// Gets the origin.
            /// </summary>
            /// <value>The origin.</value>
            public int Origin { get; }

            /// <summary>
            /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="CustomClassWithValueTypeImplicitConversion"/>.
            /// </summary>
            /// <param name="origin">The origin.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator CustomClassWithValueTypeImplicitConversion(int origin)
            {
                return new CustomClassWithValueTypeImplicitConversion(origin);
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Origin.ToString();
            }
        }

        /// <summary>
        /// Class CustomClassWithReversedValueTypeImplicitConversion.
        /// </summary>
        public class CustomClassWithReversedValueTypeImplicitConversion
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomClassWithReversedValueTypeImplicitConversion"/> class.
            /// </summary>
            /// <param name="origin">The origin.</param>
            public CustomClassWithReversedValueTypeImplicitConversion(int origin)
            {
                Origin = origin;
            }

            /// <summary>
            /// Gets the origin.
            /// </summary>
            /// <value>The origin.</value>
            public int Origin { get; }

            /// <summary>
            /// Performs an implicit conversion from <see cref="CustomClassWithReversedValueTypeImplicitConversion"/> to <see cref="System.Int32"/>.
            /// </summary>
            /// <param name="origin">The origin.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator int(CustomClassWithReversedValueTypeImplicitConversion origin)
            {
                return origin.Origin;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Origin.ToString();
            }
        }

        /// <summary>
        /// Class TestImplicitConversionContainer.
        /// </summary>
        public class TestImplicitConversionContainer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestImplicitConversionContainer"/> class.
            /// </summary>
            /// <param name="oneWay">The one way.</param>
            /// <param name="reversed">The reversed.</param>
            /// <param name="valueType">Type of the value.</param>
            /// <param name="reversedValueType">Type of the reversed value.</param>
            public TestImplicitConversionContainer(
                CustomClassWithOneWayImplicitConversion oneWay,
                CustomClassWithReversedImplicitConversion reversed,
                CustomClassWithValueTypeImplicitConversion valueType,
                CustomClassWithReversedValueTypeImplicitConversion reversedValueType)
            {
                OneWay = oneWay;
                Reversed = reversed;
                ValueType = valueType;
                ReversedValueType = reversedValueType;
            }

            /// <summary>
            /// Gets the one way.
            /// </summary>
            /// <value>The one way.</value>
            public CustomClassWithOneWayImplicitConversion OneWay { get; }

            /// <summary>
            /// Gets the reversed.
            /// </summary>
            /// <value>The reversed.</value>
            public CustomClassWithReversedImplicitConversion Reversed { get; }

            /// <summary>
            /// Gets the type of the value.
            /// </summary>
            /// <value>The type of the value.</value>
            public CustomClassWithValueTypeImplicitConversion ValueType { get; }

            /// <summary>
            /// Gets the type of the reversed value.
            /// </summary>
            /// <value>The type of the reversed value.</value>
            public CustomClassWithReversedValueTypeImplicitConversion ReversedValueType { get; }
        }

        /// <summary>
        /// Class TextHolder.
        /// </summary>
        public class TextHolder
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TextHolder"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="note">The note.</param>
            public TextHolder(string name, CustomTextClass note)
            {
                Name = name;
                Note = note;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; }

            /// <summary>
            /// Gets the note.
            /// </summary>
            /// <value>The note.</value>
            public CustomTextClass Note { get; }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return Name + " (" + Note + ")";
            }
        }

        /// <summary>
        /// Class StaticHelper.
        /// </summary>
        public static class StaticHelper
        {
            /// <summary>
            /// Gets the unique identifier.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>System.Nullable&lt;Guid&gt;.</returns>
            public static Guid? GetGuid(string name)
            {
                return Guid.NewGuid();
            }
        }

        /// <summary>
        /// Class TestCustomTypeProvider.
        /// Implements the <see cref="ISynergy.Framework.Core.Linq.Providers.AbstractDynamicLinqCustomTypeProvider" />
        /// Implements the <see cref="ISynergy.Framework.Core.Linq.Abstractions.IDynamicLinkCustomTypeProvider" />
        /// </summary>
        /// <seealso cref="ISynergy.Framework.Core.Linq.Providers.AbstractDynamicLinqCustomTypeProvider" />
        /// <seealso cref="ISynergy.Framework.Core.Linq.Abstractions.IDynamicLinkCustomTypeProvider" />
        public class TestCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
        {
            /// <summary>
            /// The custom types
            /// </summary>
            private HashSet<Type> _customTypes;

            /// <summary>
            /// Returns a list of custom types that System.Linq.Dynamic.Core will understand.
            /// </summary>
            /// <returns>A <see cref="T:System.Collections.Generic.HashSet`1" /> list of custom types.</returns>
            public virtual HashSet<Type> GetCustomTypes()
            {
                if (_customTypes != null)
                {
                    return _customTypes;
                }

                _customTypes =
                    new HashSet<Type>(
                        FindTypesMarkedWithDynamicLinqTypeAttribute(new[] { GetType().GetTypeInfo().Assembly }))
                    {
                        typeof(CustomClassWithStaticMethod),
                        typeof(StaticHelper)
                    };
                return _customTypes;
            }

            /// <summary>
            /// Resolve any type by fullname which is registered in the current application domain.
            /// </summary>
            /// <param name="typeName">The typename to resolve.</param>
            /// <returns>A resolved <see cref="T:System.Type" /> or null when not found.</returns>
            public Type ResolveType(string typeName)
            {
                return Type.GetType(typeName);
            }

            /// <summary>
            /// Resolves the name of the type by simple.
            /// </summary>
            /// <param name="typeName">Name of the type.</param>
            /// <returns>Type.</returns>
            public Type ResolveTypeBySimpleName(string typeName)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                return ResolveTypeBySimpleName(assemblies, typeName);
            }
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_true.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_true()
        {
            // Assign
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = true
            };

            // Act
            var expression = DynamicExpressionParser.ParseLambda<string, bool>(config, true, "s => s == \"x\"");

            // Assert
            dynamic constantExpression = ((MemberExpression)(expression.Body as BinaryExpression).Right).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            var propertyInfo = wrappedObj.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
            string value = propertyInfo.GetValue(wrappedObj) as string;

            Check.That(value).IsEqualTo("x");
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_WithStructWithEquality.
        /// </summary>
        /// <param name="query">The query.</param>
        [Theory]
        [InlineData("Where(x => x.SnowflakeId == {0})")]
        [InlineData("Where(x => x.SnowflakeId = {0})")]
        public void DynamicExpressionParser_ParseLambda_WithStructWithEquality(string query)
        {
            // Assign
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            // Act
            ulong expectedX = (ulong)long.MaxValue + 3;

            query = string.Format(query, expectedX);
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(qry.GetType(), null, query);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke(qry) as IEnumerable<dynamic>;

            var expected = qry.Where(gg => gg.SnowflakeId == new SnowflakeId(expectedX)).ToList();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Count);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_false.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_false()
        {
            // Assign
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = false
            };

            // Act
            var expression = DynamicExpressionParser.ParseLambda<string, bool>(config, true, "s => s == \"x\"");

            // Assert
            dynamic constantExpression = (ConstantExpression)(expression.Body as BinaryExpression).Right;
            string value = constantExpression.Value;

            Check.That(value).IsEqualTo("x");
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_ToList.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_ToList()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            // Act
            string query = "OrderBy(gg => gg.Income).ToList()";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(qry.GetType(), null, query);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke(qry) as IEnumerable<dynamic>;

            var expected = qry.OrderBy(gg => gg.Income).ToList();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Count);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Complex_1.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Complex_1()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            // Act
            string query =
                "Users.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new (j.Key.Age, j.Sum(k => k.Income) As TotalIncome))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke() as IEnumerable<dynamic>;

            var expected = qry.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age)
                .Select(j => new { j.Key.Age, TotalIncome = j.Sum(k => k.Income) })
                .Select(c => new ComplexParseLambda1Result { Age = c.Age, TotalIncome = c.TotalIncome }).Cast<dynamic>()
                .ToArray();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Length);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Complex_2.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Complex_2()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            // Act
            string query =
                "GroupBy(x => new { x.Profile.Age }, it).OrderBy(gg => gg.Key.Age).Select(j => new (j.Key.Age, j.Sum(k => k.Income) As TotalIncome))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(qry.GetType(), null, query);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke(qry) as IEnumerable<dynamic>;

            var expected = qry.GroupBy(x => new { x.Profile.Age }, x => x).OrderBy(gg => gg.Key.Age)
                .Select(j => new { j.Key.Age, TotalIncome = j.Sum(k => k.Income) })
                .Select(c => new ComplexParseLambda1Result { Age = c.Age, TotalIncome = c.TotalIncome }).Cast<dynamic>()
                .ToArray();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Length);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Complex_3.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Complex_3()
        {
            var config = new ParsingConfig
            {
                CustomTypeProvider = new TestCustomTypeProvider()
            };

            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            // Act
            string stringExpression =
                "Users.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new ISynergy.Framework.Core.Linq.Extensions.Tests.DynamicExpressionParserTests+ComplexParseLambda3Result{j.Key.Age, j.Sum(k => k.Income) As TotalIncome})";
            LambdaExpression expression =
                DynamicExpressionParser.ParseLambda(config, null, stringExpression, externals);
            Delegate del = expression.Compile();
            IEnumerable<dynamic> result = del.DynamicInvoke() as IEnumerable<dynamic>;

            var expected = qry.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age)
                .Select(j => new ComplexParseLambda3Result { Age = j.Key.Age, TotalIncome = j.Sum(k => k.Income) })
                .Cast<dynamic>().ToArray();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result).HasSize(expected.Length);
            Check.That(result.ToArray()[0]).Equals(expected[0]);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Select_1.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Select_1()
        {
            // Arrange
            var testList = User.GenerateSampleModels(51);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            // Act
            string query = "Users.Select(j => new User(j.Income As Income))";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Select_2.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Select_2()
        {
            // Arrange
            var testList = User.GenerateSampleModels(5);
            var qry = testList.AsQueryable();

            var externals = new Dictionary<string, object>
            {
                {"Users", qry}
            };

            // Act
            string query = "Users.Select(j => j)";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(null, query, externals);
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);
        }

        // https://github.com/StefH/System.Linq.Dynamic.Core/issues/58
        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_4_Issue58.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_4_Issue58()
        {
            var expressionParams = new[]
            {
                Expression.Parameter(typeof(MyClass), "myObj")
            };

            var myClassInstance = new MyClass();
            var invokersMerge = new List<object> { myClassInstance };

            LambdaExpression expression =
                DynamicExpressionParser.ParseLambda(false, expressionParams, null, "myObj.Foo()");
            Delegate del = expression.Compile();
            object result = del.DynamicInvoke(invokersMerge.ToArray());

            Check.That(result).Equals(42);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_DuplicateParameterNames_ThrowsException.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_DuplicateParameterNames_ThrowsException()
        {
            // Arrange
            var parameters = new[]
            {
                Expression.Parameter(typeof(int), "x"),
                Expression.Parameter(typeof(int), "x")
            };

            // Act and Assert
            Check.ThatCode(() => DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42"))
                .Throws<ParseException>()
                .WithMessage("The identifier 'x' was defined more than once");
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_EmptyParameterList.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_EmptyParameterList()
        {
            // Arrange
            var pEmpty = new ParameterExpression[] { };

            // Act
            var @delegate = DynamicExpressionParser.ParseLambda(pEmpty, null, "1+2").Compile();
            int? result = @delegate.DynamicInvoke() as int?;

            // Assert
            Check.That(result).Equals(3);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_ParameterName.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_ParameterName()
        {
            // Arrange
            var parameters = new[]
            {
                Expression.Parameter(typeof(int), "x")
            };

            // Assert
            var expressionX = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "x == 42");
            var expressionIT = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42");

            // Assert
            Assert.Equal(typeof(bool), expressionX.Body.Type);
            Assert.Equal(typeof(bool), expressionIT.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_ParameterName_Empty.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_ParameterName_Empty()
        {
            // Arrange
            var parameters = new[]
            {
                Expression.Parameter(typeof(int), "")
            };

            // Assert
            var expression = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42");

            // Assert
            Assert.Equal(typeof(bool), expression.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_ParameterName_Null.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_ParameterName_Null()
        {
            // Arrange
            var parameters = new[]
            {
                Expression.Parameter(typeof(int), null)
            };

            // Assert
            var expression = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42");

            // Assert
            Assert.Equal(typeof(bool), expression.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_ParameterExpressionMethodCall_ReturnsIntExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_ParameterExpressionMethodCall_ReturnsIntExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(true,
                new[] { Expression.Parameter(typeof(int), "x") },
                typeof(int),
                "x + 1");
            Assert.Equal(typeof(int), expression.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_RealNumbers.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_RealNumbers()
        {
            var parameters = new ParameterExpression[0];

            var result1 = DynamicExpressionParser.ParseLambda(parameters, typeof(double), "0.10");
            var result2 = DynamicExpressionParser.ParseLambda(parameters, typeof(double), "0.10d");
            var result3 = DynamicExpressionParser.ParseLambda(parameters, typeof(float), "0.10f");
            var result4 = DynamicExpressionParser.ParseLambda(parameters, typeof(decimal), "0.10m");

            // Assert
            Assert.Equal(0.10d, result1.Compile().DynamicInvoke());
            Assert.Equal(0.10d, result2.Compile().DynamicInvoke());
            Assert.Equal(0.10f, result3.Compile().DynamicInvoke());
            Assert.Equal(0.10m, result4.Compile().DynamicInvoke());
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteral_ReturnsBooleanLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteral_ReturnsBooleanLambdaExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(bool), "Property1 == \"test\"");
            Assert.Equal(typeof(bool), expression.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteralEmpty_ReturnsBooleanLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteralEmpty_ReturnsBooleanLambdaExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(bool), "Property1 == \"\"");
            Assert.Equal(typeof(bool), expression.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Config_StringLiteralEmpty_ReturnsBooleanLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Config_StringLiteralEmpty_ReturnsBooleanLambdaExpression()
        {
            var config = new ParsingConfig();
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(bool), "Property1 == \"\"");
            Assert.Equal(typeof(bool), expression.Body.Type);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteralEmbeddedQuote_ReturnsBooleanLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteralEmbeddedQuote_ReturnsBooleanLambdaExpression()
        {
            string expectedRightValue = "\"test \\\"string\"";

            // Act
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(bool),
                string.Format("Property1 == {0}", expectedRightValue));

            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(bool), expression.Body.Type);
            Assert.Equal(expectedRightValue, rightValue);
        }

        /// <summary>
        /// @see https://github.com/StefH/System.Linq.Dynamic.Core/issues/294
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_MultipleLambdas()
        {
            var users = new[]
            {
                new { name = "Juan", age = 25 },
                new { name = "Juan", age = 25 },
                new { name = "David", age = 12 },
                new { name = "Juan", age = 25 },
                new { name = "Juan", age = 4 },
                new { name = "Pedro", age = 2 },
                new { name = "Juan", age = 25 }
            }.ToList();

            IQueryable query;

            // One lambda
            string res1 = "[{\"Key\":{\"name\":\"Juan\"},\"nativeAggregates\":{\"ageSum\":104},\"Grouping\":[{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":4},{\"name\":\"Juan\",\"age\":25}]},{\"Key\":{\"name\":\"David\"},\"nativeAggregates\":{\"ageSum\":12},\"Grouping\":[{\"name\":\"David\",\"age\":12}]},{\"Key\":{\"name\":\"Pedro\"},\"nativeAggregates\":{\"ageSum\":2},\"Grouping\":[{\"name\":\"Pedro\",\"age\":2}]}]";
            query = users.AsQueryable();
            query = query.GroupBy("new(name as name)", "it");
            query = query.Select("new (it.Key as Key, new(it.Sum(x => x.age) as ageSum) as nativeAggregates, it as Grouping)");
            Assert.Equal(res1, JsonConvert.SerializeObject(query));

            // Multiple lambdas
            string res2 = "[{\"Key\":{\"name\":\"Juan\"},\"nativeAggregates\":{\"ageSum\":0,\"ageSum2\":104},\"Grouping\":[{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":4},{\"name\":\"Juan\",\"age\":25}]},{\"Key\":{\"name\":\"David\"},\"nativeAggregates\":{\"ageSum\":0,\"ageSum2\":12},\"Grouping\":[{\"name\":\"David\",\"age\":12}]},{\"Key\":{\"name\":\"Pedro\"},\"nativeAggregates\":{\"ageSum\":0,\"ageSum2\":2},\"Grouping\":[{\"name\":\"Pedro\",\"age\":2}]}]";
            
            query = users.AsQueryable();
            query = query.GroupBy("new(name as name)", "it");
            query = query.Select("new (it.Key as Key, new(it.Sum(x => x.age > 25 ? 1 : 0) as ageSum, it.Sum(x => x.age) as ageSum2) as nativeAggregates, it as Grouping)");
            
            Assert.Equal(res2, JsonConvert.SerializeObject(query));
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteralStartEmbeddedQuote_ReturnsBooleanLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteralStartEmbeddedQuote_ReturnsBooleanLambdaExpression()
        {
            // Assign
            string expectedRightValue = "\"\\\"test\"";

            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(bool),
                string.Format("Property1 == {0}", expectedRightValue));

            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(bool), expression.Body.Type);
            Assert.Equal(expectedRightValue, rightValue);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteral_MissingClosingQuote.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteral_MissingClosingQuote()
        {
            string expectedRightValue = "\"test\\\"";

            Assert.Throws<ParseException>(() => DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(bool),
                string.Format("Property1 == {0}", expectedRightValue)));
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteralEscapedBackslash_ReturnsBooleanLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteralEscapedBackslash_ReturnsBooleanLambdaExpression()
        {
            // Assign
            string expectedRightValue = "\"test\\string\"";

            // Act
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(bool),
                string.Format("Property1 == {0}", expectedRightValue));

            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(Boolean), expression.Body.Type);
            Assert.Equal(expectedRightValue, rightValue);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteral_Backslash.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteral_Backslash()
        {
            string expectedLeftValue = "Property1.IndexOf(\"\\\\\")";
            string expectedRightValue = "0";
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(Boolean),
                string.Format("{0} >= {1}", expectedLeftValue, expectedRightValue));

            string leftValue = ((BinaryExpression)expression.Body).Left.ToString();
            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(Boolean), expression.Body.Type);
            Assert.Equal(expectedLeftValue, leftValue);
            Assert.Equal(expectedRightValue, rightValue);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_StringLiteral_QuotationMark.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_StringLiteral_QuotationMark()
        {
            string expectedLeftValue = "Property1.IndexOf(\"\\\"\")";
            string expectedRightValue = "0";
            var expression = DynamicExpressionParser.ParseLambda(
                new[] { Expression.Parameter(typeof(string), "Property1") },
                typeof(Boolean),
                string.Format("{0} >= {1}", expectedLeftValue, expectedRightValue));

            string leftValue = ((BinaryExpression)expression.Body).Left.ToString();
            string rightValue = ((BinaryExpression)expression.Body).Right.ToString();
            Assert.Equal(typeof(Boolean), expression.Body.Type);
            Assert.Equal(expectedLeftValue, leftValue);
            Assert.Equal(expectedRightValue, rightValue);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_TupleToStringMethodCall_ReturnsStringLambdaExpression.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_TupleToStringMethodCall_ReturnsStringLambdaExpression()
        {
            var expression = DynamicExpressionParser.ParseLambda(
                typeof(Tuple<int>),
                typeof(string),
                "it.ToString()");
            Assert.Equal(typeof(string), expression.ReturnType);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_IllegalMethodCall_ThrowsException.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_IllegalMethodCall_ThrowsException()
        {
            Check.ThatCode(() => { DynamicExpressionParser.ParseLambda(typeof(System.IO.FileStream), null, "it.Close()"); })
                .Throws<ParseException>().WithMessage("Methods on type 'Stream' are not accessible");
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_CustomMethod.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_CustomMethod()
        {
            // Assign
            var config = new ParsingConfig
            {
                CustomTypeProvider = new TestCustomTypeProvider()
            };

            var context = new CustomClassWithStaticMethod();
            string expression = $"{nameof(CustomClassWithStaticMethod)}.{nameof(CustomClassWithStaticMethod.GetAge)}(10)";

            // Act
            var lambdaExpression = DynamicExpressionParser.ParseLambda(config, typeof(CustomClassWithStaticMethod), null, expression);
            Delegate del = lambdaExpression.Compile();
            int result = (int)del.DynamicInvoke(context);

            // Assert
            Check.That(result).IsEqualTo(10);
        }

        //[Fact]
        /// <summary>
        /// Dynamics the expression parser parse lambda with inner string literal.
        /// </summary>
        public void DynamicExpressionParser_ParseLambda_With_InnerStringLiteral()
        {
            // Assign
            string originalTrueValue = "simple + \"quoted\"";
            string doubleQuotedTrueValue = "simple + \"\"quoted\"\"";
            string expressionText = $"iif(1>0, \"{doubleQuotedTrueValue}\", \"false\")";

            // Act
            var lambda = DynamicExpressionParser.ParseLambda(typeof(string), null, expressionText);
            var del = lambda.Compile();
            object result = del.DynamicInvoke(string.Empty);

            // Assert
            Check.That(result).IsEqualTo(originalTrueValue);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_With_Guid_Equals_Null.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_With_Guid_Equals_Null()
        {
            // Arrange
            var user = new User();
            Guid someId = Guid.NewGuid();
            string expressionText = $"iif(@0.Id == null, @0.Id == Guid.Parse(\"{someId}\"), Id == Id)";

            // Act
            var lambda = DynamicExpressionParser.ParseLambda(typeof(User), null, expressionText, user);
            var boolLambda = lambda as Expression<Func<User, bool>>;
            Assert.NotNull(boolLambda);

            var del = lambda.Compile();
            bool result = (bool)del.DynamicInvoke(user);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_With_Null_Equals_Guid.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_With_Null_Equals_Guid()
        {
            // Arrange
            var user = new User();
            Guid someId = Guid.NewGuid();
            string expressionText = $"iif(null == @0.Id, @0.Id == Guid.Parse(\"{someId}\"), Id == Id)";

            // Act
            var lambda = DynamicExpressionParser.ParseLambda(typeof(User), null, expressionText, user);
            var boolLambda = lambda as Expression<Func<User, bool>>;
            Assert.NotNull(boolLambda);

            var del = lambda.Compile();
            bool result = (bool)del.DynamicInvoke(user);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_With_Guid_Equals_String.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_With_Guid_Equals_String()
        {
            // Arrange
            Guid someId = Guid.NewGuid();
            Guid anotherId = Guid.NewGuid();

            var user = new User
            {
                Id = someId
            };

            Guid guidEmpty = Guid.Empty;
            string expressionText =
                $"iif(@0.Id == \"{someId}\", Guid.Parse(\"{guidEmpty}\"), Guid.Parse(\"{anotherId}\"))";

            // Act
            var lambda = DynamicExpressionParser.ParseLambda(typeof(User), null, expressionText, user);
            var guidLambda = lambda as Expression<Func<User, Guid>>;
            Assert.NotNull(guidLambda);

            var del = lambda.Compile();
            Guid result = (Guid)del.DynamicInvoke(user);

            // Assert
            Assert.Equal(guidEmpty, result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_With_Concat_String_CustomType.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_With_Concat_String_CustomType()
        {
            // Arrange
            string name = "name1";
            string note = "note1";
            var textHolder = new TextHolder(name, note);
            string expressionText = "Name + \" (\" + Note + \")\"";

            // Act 1
            var lambda = DynamicExpressionParser.ParseLambda(typeof(TextHolder), null, expressionText, textHolder);
            var stringLambda = lambda as Expression<Func<TextHolder, string>>;

            // Assert 1
            Assert.NotNull(stringLambda);

            // Act 2
            var del = lambda.Compile();
            string result = (string)del.DynamicInvoke(textHolder);

            // Assert 2
            Assert.Equal("name1 (note1)", result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_With_Concat_CustomType_String.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_With_Concat_CustomType_String()
        {
            // Arrange
            string name = "name1";
            string note = "note1";
            var textHolder = new TextHolder(name, note);
            string expressionText = "Note + \" (\" + Name + \")\"";

            // Act 1
            var lambda = DynamicExpressionParser.ParseLambda(typeof(TextHolder), null, expressionText, textHolder);
            var stringLambda = lambda as Expression<Func<TextHolder, string>>;

            // Assert 1
            Assert.NotNull(stringLambda);

            // Act 2
            var del = lambda.Compile();
            string result = (string)del.DynamicInvoke(textHolder);

            // Assert 2
            Assert.Equal("note1 (name1)", result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_With_One_Way_Implicit_Conversions.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_With_One_Way_Implicit_Conversions()
        {
            // Arrange
            var testString = "test";
            var testInt = 6;
            var container = new TestImplicitConversionContainer(testString, new CustomClassWithReversedImplicitConversion(testString), testInt, new CustomClassWithReversedValueTypeImplicitConversion(testInt));

            var expressionTextString = $"OneWay == \"{testString}\"";
            var expressionTextReversed = $"Reversed == \"{testString}\"";
            var expressionTextValueType = $"ValueType == {testInt}";
            var expressionTextReversedValueType = $"ReversedValueType == {testInt}";

            var invertedExpressionTextString = $"\"{testString}\" == OneWay";
            var invertedExpressionTextReversed = $"\"{testString}\" == Reversed";
            var invertedExpressionTextValueType = $"{testInt} == ValueType";
            var invertedExpressionTextReversedValueType = $"{testInt} == ReversedValueType";

            // Act 1
            var lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextString);

            // Assert 1
            Assert.NotNull(lambda);

            // Act 2
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextReversed);

            // Assert 2
            Assert.NotNull(lambda);

            // Act 3
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextValueType);

            // Assert 3
            Assert.NotNull(lambda);

            // Act 4
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextReversedValueType);

            // Assert 4
            Assert.NotNull(lambda);

            // Act 5
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextString);

            // Assert 5
            Assert.NotNull(lambda);

            // Act 6
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextReversed);

            // Assert 6
            Assert.NotNull(lambda);

            // Act 7
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextValueType);

            // Assert 7
            Assert.NotNull(lambda);

            // Act 8
            lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextReversedValueType);

            // Assert 8
            Assert.NotNull(lambda);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_Operator_Less_Greater_With_Guids.
        /// </summary>
        [Fact]
        public void DynamicExpressionParser_ParseLambda_Operator_Less_Greater_With_Guids()
        {
            var config = new ParsingConfig
            {
                CustomTypeProvider = new TestCustomTypeProvider()
            };

            // Arrange
            Guid someId = Guid.NewGuid();
            Guid anotherId = Guid.NewGuid();

            var user = new User
            {
                Id = someId
            };

            Guid guidEmpty = Guid.Empty;
            string expressionText =
                $"iif(@0.Id == StaticHelper.GetGuid(\"name\"), Guid.Parse(\"{guidEmpty}\"), Guid.Parse(\"{anotherId}\"))";

            // Act
            var lambda = DynamicExpressionParser.ParseLambda(config, typeof(User), null, expressionText, user);
            var guidLambda = lambda as Expression<Func<User, Guid>>;
            Assert.NotNull(guidLambda);

            var del = lambda.Compile();
            Guid result = (Guid)del.DynamicInvoke(user);

            // Assert
            Assert.Equal(anotherId, result);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_RenameParameterExpression.
        /// </summary>
        /// <param name="expressionAsString">The expression as string.</param>
        /// <param name="expected">The expected.</param>
        [Theory]
        [InlineData("c => c.Age == 8", "c => (c.Age == 8)")]
        [InlineData("c => c.Name == \"test\"", "c => (c.Name == \"test\")")]
        public void DynamicExpressionParser_ParseLambda_RenameParameterExpression(string expressionAsString, string expected)
        {
            // Arrange
            var config = new ParsingConfig
            {
                RenameParameterExpression = true
            };

            // Act
            var expression = DynamicExpressionParser.ParseLambda<ComplexParseLambda1Result, bool>(config, true, expressionAsString);
            string result = expression.ToString();

            // Assert
            Check.That(result).IsEqualTo(expected);
        }

        /// <summary>
        /// Defines the test method DynamicExpressionParser_ParseLambda_SupportEnumerationStringComparison.
        /// </summary>
        /// <param name="expressionAsString">The expression as string.</param>
        /// <param name="testValue">The test value.</param>
        /// <param name="expectedResult">if set to <c>true</c> [expected result].</param>
        [Theory]
        [InlineData(@"p0.Equals(""Testing"", 3)", "testinG", true)]
        [InlineData(@"p0.Equals(""Testing"", StringComparison.InvariantCultureIgnoreCase)", "testinG", true)]
        public void DynamicExpressionParser_ParseLambda_SupportEnumerationStringComparison(string expressionAsString, string testValue, bool expectedResult)
        {
            // Arrange
            var p0 = Expression.Parameter(typeof(string), "p0");

            // Act
            var expression = DynamicExpressionParser.ParseLambda(new[] { p0 }, typeof(bool), expressionAsString);
            Delegate del = expression.Compile();
            bool? result = del.DynamicInvoke(testValue) as bool?;

            // Assert
            Check.That(result).IsEqualTo(expectedResult);
        }
    }
}
