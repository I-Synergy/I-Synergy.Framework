using ISynergy.Framework.Geography.Global;
using ISynergy.Framework.Geography.Utm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Tests.Utm
{
    /// <summary>
    /// Class UtmGridTests.
    /// </summary>
    [TestClass]
    public class UtmGridTests
    {
        /// <summary>
        /// The utm
        /// </summary>
        readonly UtmProjection utm = new UtmProjection();

        /// <summary>
        /// Validates the corners.
        /// </summary>
        /// <param name="g">The g.</param>
        private void ValidateCorners(UtmGrid g)
        {
            Assert.AreEqual(g.ToString(), new UtmGrid(utm, g.LowerRightCorner).ToString());
            Assert.AreEqual(g.ToString(), new UtmGrid(utm, g.UpperLeftCorner).ToString());
            Assert.AreEqual(g.ToString(), new UtmGrid(utm, g.UpperRightCorner).ToString());
        }

        /// <summary>
        /// Defines the test method TestConstructor1.
        /// </summary>
        [TestMethod]
        public void TestConstructor1()
        {
            var g = new UtmGrid(utm, 1, 'C');
            Assert.AreEqual(g.LowerLeftCorner.Longitude, -180);
            Assert.AreEqual(g.LowerLeftCorner.Latitude, utm.MinLatitude);
            Assert.AreEqual(6.0, g.Width);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestConstructor2.
        /// </summary>
        [TestMethod]
        public void TestConstructor2()
        {
            var g = new UtmGrid(utm, 1, 'X');
            Assert.AreEqual(g.LowerLeftCorner.Longitude, -180);
            Assert.AreEqual(g.LowerLeftCorner.Latitude, utm.MaxLatitude - g.Height);
            Assert.AreEqual(12.0, g.Height);
            Assert.AreEqual(6.0, g.Width);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestConstructor3.
        /// </summary>
        [TestMethod]
        public void TestConstructor3()
        {
            var loc = new GlobalCoordinates(utm.MaxLatitude + 1.0, 0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, loc));
        }

        /// <summary>
        /// Defines the test method TestConstructor4.
        /// </summary>
        [TestMethod]
        public void TestConstructor4()
        {
            var loc = new GlobalCoordinates(utm.MaxLatitude, 0);
            var g = new UtmGrid(utm, loc);
            Assert.IsTrue(true);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestConstructor5.
        /// </summary>
        [TestMethod]
        public void TestConstructor5()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 0, 'C'));
        }

        /// <summary>
        /// Defines the test method TestConstructor6.
        /// </summary>
        [TestMethod]
        public void TestConstructor6()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 1, 'A'));
        }

        /// <summary>
        /// Defines the test method TestConstructor7.
        /// </summary>
        [TestMethod]
        public void TestConstructor7()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, UtmGrid.NumberOfGrids + 1));
        }

        /// <summary>
        /// Defines the test method TestConstructor_32X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_32X()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 32, 'X'));
        }

        /// <summary>
        /// Defines the test method TestConstructor_34X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_34X()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 34, 'X'));
        }

        /// <summary>
        /// Defines the test method TestConstructor_36X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_36X()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 36, 'X'));
        }

        /// <summary>
        /// Defines the test method TestConstructor_32V.
        /// </summary>
        [TestMethod]
        public void TestConstructor_32V()
        {
            var g = new UtmGrid(utm, 32, 'V');
            Assert.AreEqual(9.0, g.Width);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestConstructor_31V.
        /// </summary>
        [TestMethod]
        public void TestConstructor_31V()
        {
            var g = new UtmGrid(utm, 31, 'V');
            Assert.AreEqual(3.0, g.Width);
            var l = g.LowerLeftCorner;
            // 4 degrees east of lower left is normally in the same grid
            // but not so in 31V
            l.Longitude += 4.0;
            var g2 = new UtmGrid(utm, l);
            Assert.AreEqual("32V", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        /// <summary>
        /// Defines the test method TestConstructor_31X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_31X()
        {
            var g = new UtmGrid(utm, 31, 'X');
            Assert.AreEqual(9.0, g.Width);
            var l = g.LowerLeftCorner;
            // Going a little more than width should bring us 
            // into the next zone 32, but not in this band
            l.Longitude += g.Width + 1.0;
            var g2 = new UtmGrid(utm, l);
            Assert.AreEqual("33X", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        /// <summary>
        /// Defines the test method TestConstructor_37X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_37X()
        {
            var g = new UtmGrid(utm, 37, 'X');
            Assert.AreEqual(9.0, g.Width);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestConstructor_33X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_33X()
        {
            var g = new UtmGrid(utm, 33, 'X');
            Assert.AreEqual(12.0, g.Width);
            var l = g.LowerRightCorner;
            l.Longitude += 1.0;
            var g2 = new UtmGrid(utm, l);
            Assert.AreEqual("35X", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        /// <summary>
        /// Defines the test method TestConstructor_35X.
        /// </summary>
        [TestMethod]
        public void TestConstructor_35X()
        {
            var g = new UtmGrid(utm, 35, 'X');
            Assert.AreEqual(12.0, g.Width);
            var l = g.LowerRightCorner;
            l.Longitude += 1.0;
            var g2 = new UtmGrid(utm, l);
            Assert.AreEqual("37X", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        /// <summary>
        /// Defines the test method TestCorners.
        /// </summary>
        [TestMethod]
        public void TestCorners()
        {
            var g = new UtmGrid(utm, 32, 'U');
            var glrc = new UtmGrid(utm, g.LowerRightCorner);
            Assert.AreEqual("32U", glrc.ToString());
            var gulc = new UtmGrid(utm, g.UpperLeftCorner);
            Assert.AreEqual("32U", gulc.ToString());
            var gurc = new UtmGrid(utm, g.UpperRightCorner);
            Assert.AreEqual("32U", gurc.ToString());
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestOrdinal.
        /// </summary>
        [TestMethod]
        public void TestOrdinal()
        {
            var g = new UtmGrid(utm, 32, 'U');
            var ord = g.Ordinal;
            var g2 = new UtmGrid(utm, ord);
            Assert.AreEqual(g, g2);
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        /// <summary>
        /// Defines the test method TestOrdinal2.
        /// </summary>
        [TestMethod]
        public void TestOrdinal2()
        {
            for (var ord = 0; ord < 1200; ord++)
            {
                if (UtmGrid.IsValidOrdinal(ord))
                {
                    var g = new UtmGrid(utm, ord);
                    Assert.AreEqual(g.Ordinal, ord);
                }
            }
        }

        /// <summary>
        /// Defines the test method TestBandSetter.
        /// </summary>
        [TestMethod]
        public void TestBandSetter()
        {
            var g0 = new UtmGrid(utm, 32, 'U');
            var g = new UtmGrid(utm, 32, 'U')
            {
                Band = 'V'
            };
            Assert.AreEqual("32V", g.ToString());
            Assert.AreEqual(g.LowerLeftCorner.Latitude - g0.LowerLeftCorner.Latitude, g0.Height);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestZoneSetter.
        /// </summary>
        [TestMethod]
        public void TestZoneSetter()
        {
            var g0 = new UtmGrid(utm, 32, 'U');
            var g = new UtmGrid(utm, 32, 'U')
            {
                Zone = 33
            };
            Assert.AreEqual("33U", g.ToString());
            Assert.AreEqual(g.LowerLeftCorner.Longitude - g0.LowerLeftCorner.Longitude, g0.Width);
            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method TestOriginCompute.
        /// </summary>
        [TestMethod]
        public void TestOriginCompute()
        {
            var g = new UtmGrid(utm, 32, 'U');

            if (g.Origin is UtmCoordinate o)
            {
                Assert.AreEqual(o.Projection, utm);
                Assert.AreEqual("32U", o.Grid.ToString());
                Assert.IsTrue(Math.Abs(o.ScaleFactor - 1.0) < 0.001);
            }

            ValidateCorners(g);
        }

        /// <summary>
        /// Defines the test method ValidateAllGridsCorners.
        /// </summary>
        [TestMethod]
        public void ValidateAllGridsCorners()
        {
            var maxHeight = double.MinValue;
            var maxWidth = double.MinValue;
            var minWidth = double.MaxValue;
            var minHeight = double.MaxValue;

            var G = new UtmGrid(utm, 1, 'C');
            for (var zone = 1; zone <= UtmGrid.NumberOfZones; zone++)
            {
                for (var band = 0; band < UtmGrid.NumberOfBands; band++)
                {
                    var valid = true;
                    try
                    {
                        G = new UtmGrid(utm, zone, band);
                    }
                    catch (Exception)
                    { valid = false; }
                    if (valid)
                    {
                        ValidateCorners(G);
                        minWidth = Math.Min(minWidth, G.MapWidth);
                        maxWidth = Math.Max(maxWidth, G.MapWidth);
                        minHeight = Math.Min(minHeight, G.MapHeight);
                        maxHeight = Math.Max(maxHeight, G.MapHeight);
                    }
                }
            }
            Assert.IsTrue(maxHeight >= minHeight && minHeight > 0.0);
            Assert.IsTrue(maxWidth >= minWidth && minWidth > 0.0);
        }

        /// <summary>
        /// Defines the test method TestEquals1.
        /// </summary>
        [TestMethod]
        public void TestEquals1()
        {
            var s = "123";
            var g = new UtmGrid(utm, 1, 'C');
            Assert.IsFalse(g.Equals(s));
        }

        /// <summary>
        /// Defines the test method TestEquals2.
        /// </summary>
        [TestMethod]
        public void TestEquals2()
        {
            var g1 = new UtmGrid(utm, 1, 'C');
            var g2 = new UtmGrid(utm, 1, 'D')
            {
                Band = 'C'
            };
            Assert.IsTrue(g1.Equals(g2));
        }

        /// <summary>
        /// Defines the test method TestEquality.
        /// </summary>
        [TestMethod]
        public void TestEquality()
        {
            var g1 = new UtmGrid(utm, 1, 'C');
            var g2 = new UtmGrid(utm, 1, 'D')
            {
                Band = 'C'
            };
            Assert.IsTrue(g1 == g2);
        }

        /// <summary>
        /// Defines the test method TestInEquality.
        /// </summary>
        [TestMethod]
        public void TestInEquality()
        {
            var g1 = new UtmGrid(utm, 1, 'C');
            var g2 = new UtmGrid(utm, 1, 'D');
            Assert.IsTrue(g1 != g2);
        }

        /// <summary>
        /// Defines the test method TestNeighbors.
        /// </summary>
        [TestMethod]
        public void TestNeighbors()
        {
            var g = new UtmGrid(utm, Constants.MyHome);
            Assert.IsTrue(g.Band == 'U' && g.Zone == 32);
            var n = g.North;
            Assert.IsTrue(n.Band == 'V' && n.Zone == 32);
            var s = g.South;
            Assert.IsTrue(s.Band == 'T' && s.Zone == 32);
            var w = g.West;
            Assert.IsTrue(w.Band == 'U' && w.Zone == 31);
            var e = g.East;
            Assert.IsTrue(e.Band == 'U' && e.Zone == 33);
        }

        /// <summary>
        /// Defines the test method TestNorth1.
        /// </summary>
        [TestMethod]
        public void TestNorth1()
        {
            var g = new UtmGrid(utm, 31, 'U');
            Assert.ThrowsException<Exception>(() => g.North);
        }

        /// <summary>
        /// Defines the test method TestNorth2.
        /// </summary>
        [TestMethod]
        public void TestNorth2()
        {
            var g = new UtmGrid(utm, 32, 'W');
            Assert.ThrowsException<Exception>(() => g.North);
        }

        /// <summary>
        /// Defines the test method TestNorth3.
        /// </summary>
        [TestMethod]
        public void TestNorth3()
        {
            var g = new UtmGrid(utm, 34, 'W');
            Assert.ThrowsException<Exception>(() => g.North);
        }

        /// <summary>
        /// Defines the test method TestNorth4.
        /// </summary>
        [TestMethod]
        public void TestNorth4()
        {
            var g = new UtmGrid(utm, 36, 'W');
            Assert.ThrowsException<Exception>(() => g.North);
        }

        /// <summary>
        /// Defines the test method TestNorth5.
        /// </summary>
        [TestMethod]
        public void TestNorth5()
        {
            var g = new UtmGrid(utm, 1, 'X');
            Assert.ThrowsException<Exception>(() => g.North);
        }

        /// <summary>
        /// Defines the test method TestSouth1.
        /// </summary>
        [TestMethod]
        public void TestSouth1()
        {
            var g = new UtmGrid(utm, 31, 'W');
            Assert.ThrowsException<Exception>(() => g.South);
        }

        /// <summary>
        /// Defines the test method TestSouth2.
        /// </summary>
        [TestMethod]
        public void TestSouth2()
        {
            var g = new UtmGrid(utm, 31, 'X');
            Assert.ThrowsException<Exception>(() => g.South);
        }

        /// <summary>
        /// Defines the test method TestSouth3.
        /// </summary>
        [TestMethod]
        public void TestSouth3()
        {
            var g = new UtmGrid(utm, 33, 'X');
            Assert.ThrowsException<Exception>(() => g.South);
        }

        /// <summary>
        /// Defines the test method TestSouth4.
        /// </summary>
        [TestMethod]
        public void TestSouth4()
        {
            var g = new UtmGrid(utm, 35, 'X');
            Assert.ThrowsException<Exception>(() => g.South);
        }

        /// <summary>
        /// Defines the test method TestSouth5.
        /// </summary>
        [TestMethod]
        public void TestSouth5()
        {
            var g = new UtmGrid(utm, 37, 'X');
            Assert.ThrowsException<Exception>(() => g.South);
        }

        /// <summary>
        /// Defines the test method TestSouth6.
        /// </summary>
        [TestMethod]
        public void TestSouth6()
        {
            var g = new UtmGrid(utm, 37, 'C');
            Assert.ThrowsException<Exception>(() => g.South);
        }
    }
}
