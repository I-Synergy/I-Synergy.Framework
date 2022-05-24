using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Common.Tests
{
    /// <summary>
    /// Class AngleTests.
    /// </summary>
    [TestClass]
    public class AngleTests
    {
        /// <summary>
        /// Defines the test method TestConstructor1.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            var a = new Angle(90);
            Assert.AreEqual(90, a.Degrees);
        }

        /// <summary>
        /// Defines the test method TestConstructor2.
        /// </summary>
        [TestMethod]
        public void TestConstructor2()
        {
            var a = new Angle(45, 30);
            Assert.AreEqual(45.5, a.Degrees);
        }

        /// <summary>
        /// Defines the test method TestConstructor3.
        /// </summary>
        [TestMethod]
        public void TestConstructor3()
        {
            var a = new Angle(-45, 30);
            Assert.AreEqual(-45.5, a.Degrees);
        }

        /// <summary>
        /// Defines the test method TestConstructor4.
        /// </summary>
        [TestMethod]
        public void TestConstructor4()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Angle(45, 60));
        }

        /// <summary>
        /// Defines the test method TestConstructor5.
        /// </summary>
        [TestMethod]
        public void TestConstructor5()
        {
            var a = new Angle(45, 30, 30);
            Assert.AreEqual(a.Degrees, 45.5 + (1.0 / 120.0));
        }

        /// <summary>
        /// Defines the test method TestConstructor6.
        /// </summary>
        [TestMethod]
        public void TestConstructor6()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Angle(45, 30, 60));
        }

        /// <summary>
        /// Defines the test method TestConstructor7.
        /// </summary>
        [TestMethod]
        public void TestConstructor7()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Angle(45, 60, 30));
        }

        /// <summary>
        /// Defines the test method TestConstructor8.
        /// </summary>
        [TestMethod]
        public void TestConstructor8()
        {
            var a = new Angle(-45, 30, 30);
            Assert.AreEqual(a.Degrees, -45.5 - (1.0 / 120.0));
        }

        /// <summary>
        /// Defines the test method TestConstructor9.
        /// </summary>
        [TestMethod]
        public void TestConstructor9()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Angle(45, -1, 30));
        }

        /// <summary>
        /// Defines the test method TestDegreeSetter.
        /// </summary>
        [TestMethod]
        public void TestDegreeSetter()
        {
            var a = new Angle(0);
            Assert.AreEqual(0, a.Degrees);
            a.Degrees = 90.0;
            Assert.AreEqual(90, a.Degrees);
        }

        /// <summary>
        /// Defines the test method TestRadiansGetter.
        /// </summary>
        [TestMethod]
        public void TestRadiansGetter()
        {
            var a = new Angle(180);
            Assert.AreEqual(a.Radians, Math.PI);
        }

        /// <summary>
        /// Defines the test method TestRadiansSetter.
        /// </summary>
        [TestMethod]
        public void TestRadiansSetter()
        {
            var a = new Angle(0);
            Assert.AreEqual(0, a.Degrees);
            a.Radians = Math.PI;
            Assert.AreEqual(180, a.Degrees);
        }

        /// <summary>
        /// Defines the test method TestAbs.
        /// </summary>
        [TestMethod]
        public void TestAbs()
        {
            var a = new Angle(-180);
            Assert.AreEqual(a.Degrees, -180);
            var b = a.Abs();
            Assert.AreEqual(180, b.Degrees);
        }

        /// <summary>
        /// Defines the test method TestCompareTo1.
        /// </summary>
        [TestMethod]
        public void TestCompareTo1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(a.CompareTo(b), -1);
            Assert.AreEqual(1, b.CompareTo(a));
        }

        /// <summary>
        /// Defines the test method TestCompareTo2.
        /// </summary>
        [TestMethod]
        public void TestCompareTo2()
        {
            var x = 180.0;
            var y = x + 1e-13;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.AreEqual(0, a.CompareTo(a));
            Assert.AreEqual(0, a.CompareTo(b));
            Assert.AreEqual(0, b.CompareTo(a));
        }

        /// <summary>
        /// Defines the test method TestEquals1.
        /// </summary>
        [TestMethod]
        public void TestEquals1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsFalse(a.Equals(b));
            b = new Angle(x);
            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Defines the test method TestEquals2.
        /// </summary>
        [TestMethod]
        public void TestEquals2()
        {
            var a = new Angle(180);
            object s = "180";
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a.Equals(s));
        }

        /// <summary>
        /// Defines the test method TestToString.
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            var a = new Angle(45);
            var s = a.ToString();
            Assert.AreEqual("45", s);
        }

        /// <summary>
        /// Defines the test method TestLessEqual1.
        /// </summary>
        [TestMethod]
        public void TestLessEqual1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsTrue(a <= b);
        }

        /// <summary>
        /// Defines the test method TestLessEqual2.
        /// </summary>
        [TestMethod]
        public void TestLessEqual2()
        {
            var x = 180.0;
            var y = 180.0 + 1e-13;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsTrue(a <= b);
        }

        /// <summary>
        /// Defines the test method TestGreaterEqual1.
        /// </summary>
        [TestMethod]
        public void TestGreaterEqual1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsTrue(b >= a);
        }

        /// <summary>
        /// Defines the test method TestGreaterEqual2.
        /// </summary>
        [TestMethod]
        public void TestGreaterEqual2()
        {
            var x = 180.0;
            var y = 180.0 + 1e-13;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsTrue(b >= a);
        }

        /// <summary>
        /// Defines the test method TestOperatorAdd.
        /// </summary>
        [TestMethod]
        public void TestOperatorAdd()
        {
            var a = new Angle(45);
            var b = new Angle(60);
            var c = a + b;
            Assert.AreEqual(105, c.Degrees);
        }

        /// <summary>
        /// Defines the test method TestOperatorSub.
        /// </summary>
        [TestMethod]
        public void TestOperatorSub()
        {
            var a = new Angle(45);
            var b = new Angle(60);
            var c = a - b;
            Assert.AreEqual(c.Degrees, -15);
        }

        /// <summary>
        /// Defines the test method TestOperatorMul.
        /// </summary>
        [TestMethod]
        public void TestOperatorMul()
        {
            var a = new Angle(45);
            var b = 2 * a;
            var c = a * 3;
            Assert.AreEqual(90, b.Degrees);
            Assert.AreEqual(135, c.Degrees);
        }

        /// <summary>
        /// Defines the test method TestGreaterThan1.
        /// </summary>
        [TestMethod]
        public void TestGreaterThan1()
        {
            var x = 180.00000001;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsTrue(a > b);
        }

        /// <summary>
        /// Defines the test method TestGreaterThan2.
        /// </summary>
        [TestMethod]
        public void TestGreaterThan2()
        {
            var x = 180.0 + 1e-12;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsFalse(a > b);
        }

        /// <summary>
        /// Defines the test method TestNotEqual1.
        /// </summary>
        [TestMethod]
        public void TestNotEqual1()
        {
            var x = 180.00000001;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Defines the test method TestNotEqual2.
        /// </summary>
        [TestMethod]
        public void TestNotEqual2()
        {
            var x = 180.0 + 1e-12;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.IsFalse(a != b);
        }

        /// <summary>
        /// Defines the test method TestNegate.
        /// </summary>
        [TestMethod]
        public void TestNegate()
        {
            var a = new Angle(180);
            var b = -a;
            Assert.AreEqual(b.Degrees, -180);
        }

        /// <summary>
        /// Defines the test method TestImplicitConversion.
        /// </summary>
        [TestMethod]
        public void TestImplicitConversion()
        {
            Angle a = 44.0;
            Assert.AreEqual(44, a.Degrees);
        }

        /// <summary>
        /// Defines the test method TestDeg2Rad.
        /// </summary>
        [TestMethod]
        public void TestDeg2Rad()
        {
            var a = Angle.DegToRad(90);
            Assert.AreEqual(a, Math.PI / 2.0);
        }

        /// <summary>
        /// Defines the test method TestRadToDeg.
        /// </summary>
        [TestMethod]
        public void TestRadToDeg()
        {
            var a = Angle.RadToDeg(Math.PI);
            Assert.AreEqual(180, a);
        }

        /// <summary>
        /// Defines the test method TestHashCode.
        /// </summary>
        [TestMethod]
        public void TestHashCode()
        {
            var a = new Angle(0);
            var b = new Angle(0.000000001);
            Assert.IsTrue(a.GetHashCode() != b.GetHashCode());
        }
    }
}
