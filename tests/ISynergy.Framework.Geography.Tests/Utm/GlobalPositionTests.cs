using ISynergy.Framework.Geography.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    /// <summary>
    /// Class GlobalPositionTests.
    /// </summary>
    [TestClass]
    public class GlobalPositionTests
    {
        /// <summary>
        /// The c1
        /// </summary>
        GlobalCoordinates c1 = new GlobalCoordinates(45, 9);
        /// <summary>
        /// The c2
        /// </summary>
        GlobalCoordinates c2 = new GlobalCoordinates(45, 10);
        /// <summary>
        /// The c3
        /// </summary>
        GlobalCoordinates c3 = new GlobalCoordinates(46, 9);
        /// <summary>
        /// The c4
        /// </summary>
        GlobalCoordinates c4 = new GlobalCoordinates(44, 9);

        /// <summary>
        /// Defines the test method TestConstructor1.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            var a = new GlobalPosition();
            Assert.AreEqual(0, a.Latitude);
            Assert.AreEqual(0, a.Longitude);
            Assert.AreEqual(0, a.Elevation);
        }

        /// <summary>
        /// Defines the test method TestConstructor2.
        /// </summary>
        [TestMethod]
        public void TestConstructor2()
        {
            var a = new GlobalPosition(c1, 100);
            Assert.AreEqual(a.Coordinates, c1);
            Assert.AreEqual(100, a.Elevation);
        }

        /// <summary>
        /// Defines the test method TestConstructor3.
        /// </summary>
        [TestMethod]
        public void TestConstructor3()
        {
            var a = new GlobalPosition(c1);
            Assert.AreEqual(a.Coordinates, c1);
            Assert.AreEqual(0, a.Elevation);
        }

        /// <summary>
        /// Defines the test method TestCoordSetter.
        /// </summary>
        [TestMethod]
        public void TestCoordSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.AreEqual(a.Coordinates, c1);
            Assert.AreEqual(0, a.Elevation);
            a.Coordinates = c2;
            Assert.AreEqual(a.Coordinates, c2);
        }

        /// <summary>
        /// Defines the test method TestLatSetter.
        /// </summary>
        [TestMethod]
        public void TestLatSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.AreEqual(a.Coordinates, c1);
            Assert.AreEqual(0, a.Elevation);
            a.Latitude = 46;
            Assert.AreEqual(a.Coordinates, c3);
        }

        /// <summary>
        /// Defines the test method TestLongSetter.
        /// </summary>
        [TestMethod]
        public void TestLongSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.AreEqual(a.Coordinates, c1);
            Assert.AreEqual(0, a.Elevation);
            a.Longitude = 10;
            Assert.AreEqual(a.Coordinates, c2);
        }

        /// <summary>
        /// Defines the test method TestElevSetter.
        /// </summary>
        [TestMethod]
        public void TestElevSetter()
        {
            var a = new GlobalPosition(c1);
            Assert.AreEqual(a.Coordinates, c1);
            Assert.AreEqual(0, a.Elevation);
            a.Elevation = -100;
            Assert.AreEqual(a.Elevation, -100);
        }

        /// <summary>
        /// Defines the test method TestCompareTo1.
        /// </summary>
        [TestMethod]
        public void TestCompareTo1()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c1);
            Assert.AreEqual(0, a.CompareTo(b));
            b.Elevation += 1e-13;
            Assert.AreEqual(0, a.CompareTo(b));
            b.Elevation = 100;
            Assert.AreEqual(a.CompareTo(b), -1);
            b.Elevation = -100;
            Assert.AreEqual(1, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestEquals.
        /// </summary>
        [TestMethod]
        public void TestEquals()
        {
            var a = new GlobalPosition(c1);
            Assert.IsFalse(a.Equals(null));
            object s = "x";
            Assert.IsFalse(a.Equals(s));
            var b = new GlobalPosition(c1);
            Assert.IsTrue(a.Equals(b));
            b.Elevation += 1;
            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Defines the test method TestToString.
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            var a = new GlobalPosition(c1, 200);
            Assert.AreEqual("45N;9E;200m", a.ToString());
        }

        /// <summary>
        /// Defines the test method TestGetHash.
        /// </summary>
        [TestMethod]
        public void TestGetHash()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c2);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestGetHash2.
        /// </summary>
        [TestMethod]
        public void TestGetHash2()
        {
            var a = new GlobalPosition(c1, 100);
            var b = new GlobalPosition(c2, -100);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestEquality.
        /// </summary>
        [TestMethod]
        public void TestEquality()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c1);
            Assert.IsTrue(a == b);
            b.Elevation += 1e-13;
            Assert.IsTrue(a == b);
            b.Elevation += 0.00001;
            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Defines the test method TestInEquality.
        /// </summary>
        [TestMethod]
        public void TestInEquality()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c1);
            Assert.IsFalse(a != b);
            b.Elevation += 1e-13;
            Assert.IsFalse(a != b);
            b.Elevation += 0.00001;
            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Defines the test method TestGreater.
        /// </summary>
        [TestMethod]
        public void TestGreater()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.IsTrue(a > b);
            b.Latitude = a.Latitude + 1e-13;
            Assert.IsFalse(a > b);
            a.Elevation = 100;
            Assert.IsTrue(a > b);
            b.Latitude = a.Latitude + 0.00001;
            Assert.IsFalse(a > b);
        }

        /// <summary>
        /// Defines the test method TestGreaterEqual.
        /// </summary>
        [TestMethod]
        public void TestGreaterEqual()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.IsTrue(a >= b);
            b.Latitude = a.Latitude + 1e-13;
            Assert.IsTrue(a >= b);
            a.Elevation = 100;
            Assert.IsTrue(a >= b);
            b.Latitude = a.Latitude - 0.00001;
            Assert.IsTrue(a >= b);
        }

        /// <summary>
        /// Defines the test method TestLess.
        /// </summary>
        [TestMethod]
        public void TestLess()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.IsTrue(b < a);
            b.Latitude = a.Latitude + 1e-13;
            Assert.IsFalse(b < a);
            a.Elevation = 100;
            Assert.IsTrue(b < a);
            b.Latitude = a.Latitude + 0.00001;
            Assert.IsFalse(b < a);
        }

        /// <summary>
        /// Defines the test method TestLessEqual.
        /// </summary>
        [TestMethod]
        public void TestLessEqual()
        {
            var a = new GlobalPosition(c1);
            var b = new GlobalPosition(c4);
            Assert.IsTrue(b <= a);
            b.Latitude = a.Latitude + 1e-13;
            Assert.IsTrue(b <= a);
            a.Elevation = 100;
            Assert.IsTrue(b <= a);
            b.Latitude = a.Latitude - 0.00001;
            Assert.IsTrue(b <= a);
        }
    }
}
