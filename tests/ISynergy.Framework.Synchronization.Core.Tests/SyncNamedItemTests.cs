using ISynergy.Framework.Synchronization.Core.Extensions;
using ISynergy.Framework.Synchronization.Core.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ISynergy.Framework.Synchronization.Core.Tests
{
    [TestClass]
    public class SyncNamedItemTests
    {
        [TestMethod]
        public void Compare_Two_Enumerable_Empty_ShouldBe_Count_Equals()
        {
            var lst1 = new List<string>();
            var lst2 = new List<string>();

            Assert.IsTrue(lst1.CompareWith(lst2));
        }


        [TestMethod]
        public void Compare_Two_Enumerable_ShouldBe_Count_Equals()
        {
            var lst1 = new List<string>() { "1" };
            var lst2 = new List<string>() { "1" };

            Assert.IsTrue(lst1.CompareWith(lst2));
        }

        [TestMethod]
        public void Compare_Two_Enumerable_FirstIsNull_ShouldNotBe_Count_Equals()
        {
            List<string> lst1 = null;
            var lst2 = new List<string>() { "2" };

            Assert.IsFalse(lst1.CompareWith(lst2));
        }

        [TestMethod]
        public void Compare_Two_Enumerable_SecondIsNull_ShouldNotBe_Count_Equals()
        {
            var lst1 = new List<string>() { "2" };
            List<string> lst2 = null;

            Assert.IsFalse(lst1.CompareWith(lst2));
        }

        [TestMethod]
        public void Compare_Two_Enumerable_Null_ShouldBe_Count_Equals()
        {
            List<string> lst1 = null;
            List<string> lst2 = null;

            Assert.IsTrue(lst1.CompareWith(lst2));
        }


        [TestMethod]
        public void Compare_Two_Enumerable_Different_ShouldNotBe_Equals()
        {
            var lst1 = new List<string>() { "1" };
            var lst2 = new List<string>() { "2" };

            Assert.IsFalse(lst1.CompareWith(lst2));
        }


        [TestMethod]
        public void Compare_SyncColumnIdentifier_When_Empty_ShouldBe_Equals()
        {
            var columnId1 = new SyncColumnIdentifier();
            var columnId2 = new SyncColumnIdentifier();

            var isNamedEquals = columnId1.EqualsByName(columnId2);

            // Check operator EqualsByName
            Assert.IsTrue(isNamedEquals);

            // Check operator == (who should make an equality on the references)
            Assert.IsFalse(columnId1 == columnId2);

            // Check default Equals which should use the EqualsByName as well
            Assert.AreEqual(columnId1, columnId2);
        }

        [TestMethod]
        public void Compare_SyncColumnIdentifier_When_OneField_NotEmpty_ShouldBe_Equals()
        {
            var columnId1 = new SyncColumnIdentifier("CustomerId", string.Empty);
            var columnId2 = new SyncColumnIdentifier("CustomerId", string.Empty);

            var isNamedEquals = columnId1.EqualsByName(columnId2);

            Assert.IsTrue(isNamedEquals);
            Assert.IsFalse(columnId1 == columnId2);
            Assert.AreEqual(columnId1, columnId2);

        }

        [TestMethod]
        public void Compare_SyncColumnIdentifier_When_OneField_Empty_And_OtherField_Null_ShouldBe_Equals()
        {
            var columnId1 = new SyncColumnIdentifier("CustomerId", string.Empty);
            var columnId2 = new SyncColumnIdentifier("CustomerId", null, string.Empty);

            var isNamedEquals = columnId1.EqualsByName(columnId2);

            Assert.IsTrue(isNamedEquals);
            Assert.IsFalse(columnId1 == columnId2);
            Assert.AreEqual(columnId1, columnId2);

        }

        [TestMethod]
        public void Compare_SyncColumnIdentifier_When_OtherInstance_Null_ShouldNotBe_Equals()
        {
            var columnId1 = new SyncColumnIdentifier("CustomerId", string.Empty);
            SyncColumnIdentifier columnId2 = null;

            var isNamedEquals = columnId1.EqualsByName(columnId2);

            Assert.IsFalse(isNamedEquals);
            Assert.IsFalse(columnId1 == columnId2);
            Assert.AreNotEqual(columnId1, columnId2);

        }

        [TestMethod]
        public void Compare_SyncColumnIdentifier_When_Instances_Null_ShouldNotBe_Equals()
        {
            SyncColumnIdentifier columnId1 = null;
            SyncColumnIdentifier columnId2 = null;

            Assert.ThrowsException<NullReferenceException>(() => columnId1.EqualsByName(columnId2));

        }


        [TestMethod]
        public void Compare_SyncColumnIdentifier_When_Property_IsDifferent_ShouldNotBe_Equals()
        {
            var columnId1 = new SyncColumnIdentifier("CustomerId1", "Customer");
            var columnId2 = new SyncColumnIdentifier("CustomerId2", "Customer");

            var isNamedEquals = columnId1.EqualsByName(columnId2);

            Assert.IsFalse(isNamedEquals);
        }
    }
}
