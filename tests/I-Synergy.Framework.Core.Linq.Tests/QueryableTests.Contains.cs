﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using ISynergy.Framework.Core.Utilities;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Contains_Dynamic_ListWithStrings()
        {
            // Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var list = new List<string> { "User1", "User5", "User10" };

            // Act
            var realQuery = baseQuery.Where(x => list.Contains(x.UserName)).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(UserName)", list).Select("Id");

            // Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }

        [Fact]
        [Trait("Issue", "130")]
        public void Contains_Dynamic_ListWithDynamicObjects()
        {
            // Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var list = new List<dynamic> { new { UserName = "User1" } };

            var keyType = DynamicClassFactory.CreateType(new[] { new DynamicProperty("UserName", typeof(string)) });
            var keyVals = (IList)CreateGenericInstance(typeof(List<>), new[] { keyType });

            var keyVal = TypeActivator.CreateInstance(keyType);
            keyType.GetProperty("UserName").SetValue(keyVal, "User1");
            keyVals.Add(keyVal);

            // Act
            var realQuery = baseQuery.Where(x => list.Contains(new { x.UserName })).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(new(it.UserName as UserName))", keyVals).Select("Id");

            // Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }

        private object CreateGenericInstance(Type type, Type[] types, params dynamic[] ctorParams)
        {
            Type genType = type.MakeGenericType(types);

            var constructor = genType.GetConstructors().First();
            return constructor.Invoke(ctorParams);
        }
    }
}
