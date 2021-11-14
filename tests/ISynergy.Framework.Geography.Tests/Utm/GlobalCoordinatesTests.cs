using ISynergy.Framework.Geography.Global;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Tests.Utm
{
    /// <summary>
    /// Class GlobalCoordinatesTests.
    /// </summary>
    [TestClass]
    public class GlobalCoordinatesTests
    {
        /// <summary>
        /// Defines the test method TestConstructor1.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            var g = new GlobalCoordinates(45, 9);
            Assert.AreEqual(45, g.Latitude.Degrees);
            Assert.AreEqual(9, g.Longitude.Degrees);
        }

        /// <summary>
        /// Defines the test method TestConstructor2.
        /// </summary>
        [TestMethod]
        public void TestConstructor2()
        {
            var g = new GlobalCoordinates(-181, 9);
            Assert.AreEqual(1, g.Latitude.Degrees);
            Assert.AreEqual(g.Longitude.Degrees, -171);
        }

        /// <summary>
        /// Defines the test method TestConstructor3.
        /// </summary>
        [TestMethod]
        public void TestConstructor3()
        {
            var g = new GlobalCoordinates(-811, 0);
            Assert.AreEqual(g.Latitude.Degrees, -89);
            Assert.AreEqual(g.Longitude.Degrees, -180);
        }

        /// <summary>
        /// Defines the test method TestConstructor4.
        /// </summary>
        [TestMethod]
        public void TestConstructor4()
        {
            var g = new GlobalCoordinates(-0, -811);
            Assert.AreEqual(0, g.Latitude.Degrees);
            Assert.AreEqual(g.Longitude.Degrees, -91);
        }

        /// <summary>
        /// Defines the test method TestLatitudeSetter.
        /// </summary>
        [TestMethod]
        public void TestLatitudeSetter()
        {
            var a = new GlobalCoordinates(0.0, 45.0)
            {
                Latitude = 45.0
            };
            Assert.AreEqual(45, a.Latitude.Degrees);
        }

        /// <summary>
        /// Defines the test method TestLongitudeSetter.
        /// </summary>
        [TestMethod]
        public void TestLongitudeSetter()
        {
            var a = new GlobalCoordinates(0.0, 45.0)
            {
                Longitude = -10.0
            };
            Assert.AreEqual(a.Longitude.Degrees, -10);
        }

        /// <summary>
        /// Defines the test method TestCompareTo1.
        /// </summary>
        [TestMethod]
        public void TestCompareTo1()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.AreEqual(0, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestCompareTo2.
        /// </summary>
        [TestMethod]
        public void TestCompareTo2()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(46, 9);
            Assert.AreEqual(a.CompareTo(b), -1);
        }

        /// <summary>
        /// Defines the test method TestCompareTo3.
        /// </summary>
        [TestMethod]
        public void TestCompareTo3()
        {
            var a = new GlobalCoordinates(45, 10);
            var b = new GlobalCoordinates(45, 9);
            Assert.AreEqual(1, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestCompareTo4.
        /// </summary>
        [TestMethod]
        public void TestCompareTo4()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 10);
            Assert.AreEqual(a.CompareTo(b), -1);
        }

        /// <summary>
        /// Defines the test method TestCompareTo5.
        /// </summary>
        [TestMethod]
        public void TestCompareTo5()
        {
            var a = new GlobalCoordinates(44, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.AreEqual(a.CompareTo(b), -1);
        }

        /// <summary>
        /// Defines the test method TestCompareTo6.
        /// </summary>
        [TestMethod]
        public void TestCompareTo6()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(44, 9);
            Assert.AreEqual(1, a.CompareTo(b));
        }

        /// <summary>
        /// Defines the test method TestEquals.
        /// </summary>
        [TestMethod]
        public void TestEquals()
        {
            var a = new GlobalCoordinates(45, 9);
            Assert.IsFalse(a.Equals(null));
            object s = "x";
            Assert.IsFalse(a.Equals(s));
            var b = new GlobalCoordinates(45, 9);
            Assert.IsTrue(a.Equals(b));
            b.Longitude += 1;
            Assert.IsFalse(a.Equals(b));
        }

        /// <summary>
        /// Defines the test method TestToString.
        /// </summary>
        [TestMethod]
        public void TestToString()
        {
            var a = new GlobalCoordinates(45, 9);
            Assert.AreEqual("45N;9E;", a.ToString());
            var b = new GlobalCoordinates(-45, -9);
            Assert.AreEqual("45S;9W;", b.ToString());
        }

        /// <summary>
        /// Defines the test method TestGetHash.
        /// </summary>
        [TestMethod]
        public void TestGetHash()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9.000000001);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Defines the test method TestEquality.
        /// </summary>
        [TestMethod]
        public void TestEquality()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.IsTrue(a == b);
            b.Longitude += 1e-13;
            Assert.IsTrue(a == b);
            b.Longitude += 0.00001;
            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Defines the test method TestInEquality.
        /// </summary>
        [TestMethod]
        public void TestInEquality()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 9);
            Assert.IsFalse(a != b);
            b.Longitude += 1e-13;
            Assert.IsFalse(a != b);
            b.Longitude += 0.00001;
            Assert.IsTrue(a != b);
        }

        /// <summary>
        /// Defines the test method TestGreater.
        /// </summary>
        [TestMethod]
        public void TestGreater()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 8);
            Assert.IsTrue(a > b);
            b.Longitude = a.Longitude + 1e-13;
            Assert.IsFalse(a > b);
            b.Longitude = a.Longitude + 0.00001;
            Assert.IsFalse(a > b);
        }

        /// <summary>
        /// Defines the test method TestGreaterEqual.
        /// </summary>
        [TestMethod]
        public void TestGreaterEqual()
        {
            var a = new GlobalCoordinates(45, 9);
            var b = new GlobalCoordinates(45, 8);
            Assert.IsTrue(a >= b);
            b.Longitude = a.Longitude + 1e-13;
            Assert.IsTrue(a >= b);
            b.Longitude = a.Longitude - 0.00001;
            Assert.IsTrue(a > b);
        }

        /// <summary>
        /// Defines the test method TestLess.
        /// </summary>
        [TestMethod]
        public void TestLess()
        {
            var a = new GlobalCoordinates(45, 8);
            var b = new GlobalCoordinates(45, 9);
            Assert.IsTrue(a < b);
            a.Longitude = b.Longitude + 1e-13;
            Assert.IsFalse(a < b);
            a.Longitude = b.Longitude + 0.00001;
            Assert.IsFalse(a < b);
        }

        /// <summary>
        /// Defines the test method TestLessEqual.
        /// </summary>
        [TestMethod]
        public void TestLessEqual()
        {
            var a = new GlobalCoordinates(45, 8);
            var b = new GlobalCoordinates(45, 9);
            Assert.IsTrue(a <= b);
            a.Longitude = b.Longitude + 1e-13;
            Assert.IsTrue(a <= b);
            a.Longitude = b.Longitude + 0.00001;
            Assert.IsFalse(a <= b);
        }

        /// <summary>
        /// Defines the test method TestAntipode.
        /// </summary>
        [TestMethod]
        public void TestAntipode()
        {
            var loc = new GlobalCoordinates(27.97, -82.53);
            var antiloc = new GlobalCoordinates(-27.97, 97.47);
            Assert.AreEqual(loc.Antipode, antiloc);
        }
    }
}
