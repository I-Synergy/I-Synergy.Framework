using System;
using ISynergy.Framework.Geography.Tests;
using Xunit;

namespace ISynergy.Framework.Geography.Utm.Tests
{
    public class UtmGridTests
    {
        readonly UtmProjection utm = new UtmProjection();

        private void ValidateCorners(UtmGrid g)
        {
            Assert.Equal(g.ToString(), new UtmGrid(utm, g.LowerRightCorner).ToString());
            Assert.Equal(g.ToString(), new UtmGrid(utm, g.UpperLeftCorner).ToString());
            Assert.Equal(g.ToString(), new UtmGrid(utm, g.UpperRightCorner).ToString());
        }

        [Fact]
        public void TestConstructor1()
        {
            var g = new UtmGrid(utm, 1, 'C');
            Assert.Equal(g.LowerLeftCorner.Longitude, -180);
            Assert.Equal(g.LowerLeftCorner.Latitude, utm.MinLatitude);
            Assert.Equal(6.0, g.Width);
            ValidateCorners(g);
        }

        [Fact]
        public void TestConstructor2()
        {
            var g = new UtmGrid(utm, 1, 'X');
            Assert.Equal(g.LowerLeftCorner.Longitude, -180);
            Assert.Equal(g.LowerLeftCorner.Latitude, utm.MaxLatitude - g.Height);
            Assert.Equal(12.0, g.Height);
            Assert.Equal(6.0, g.Width);
            ValidateCorners(g);
        }

        [Fact]
        public void TestConstructor3()
        {
            var loc = new GlobalCoordinates(utm.MaxLatitude + 1.0, 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, loc));
        }

        [Fact]
        public void TestConstructor4()
        {
            var loc = new GlobalCoordinates(utm.MaxLatitude, 0);
            var g = new UtmGrid(utm, loc);
            Assert.True(true);
            ValidateCorners(g);
        }

        [Fact]
        public void TestConstructor5()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 0, 'C'));
        }

        [Fact]
        public void TestConstructor6()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 1, 'A'));
        }

        [Fact]
        public void TestConstructor7()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, UtmGrid.NumberOfGrids + 1));
        }

        [Fact]
        public void TestConstructor_32X()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 32, 'X'));
        }

        [Fact]
        public void TestConstructor_34X()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 34, 'X'));
        }

        [Fact]
        public void TestConstructor_36X()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new UtmGrid(utm, 36, 'X'));
        }

        [Fact]
        public void TestConstructor_32V()
        {
            var g = new UtmGrid(utm, 32, 'V');
            Assert.Equal(9.0, g.Width);
            ValidateCorners(g);
        }

        [Fact]
        public void TestConstructor_31V()
        {
            var g = new UtmGrid(utm, 31, 'V');
            Assert.Equal(3.0, g.Width);
            var l = g.LowerLeftCorner;
            // 4 degrees east of lower left is normally in the same grid
            // but not so in 31V
            l.Longitude += 4.0;
            var g2 = new UtmGrid(utm, l);
            Assert.Equal("32V", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        [Fact]
        public void TestConstructor_31X()
        {
            var g = new UtmGrid(utm, 31, 'X');
            Assert.Equal(9.0, g.Width);
            var l = g.LowerLeftCorner;
            // Going a little more than width should bring us 
            // into the next zone 32, but not in this band
            l.Longitude += g.Width + 1.0;
            var g2 = new UtmGrid(utm, l);
            Assert.Equal("33X", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        [Fact]
        public void TestConstructor_37X()
        {
            var g = new UtmGrid(utm, 37, 'X');
            Assert.Equal(9.0, g.Width);
            ValidateCorners(g);
        }

        [Fact]
        public void TestConstructor_33X()
        {
            var g = new UtmGrid(utm, 33, 'X');
            Assert.Equal(12.0, g.Width);
            var l = g.LowerRightCorner;
            l.Longitude += 1.0;
            var g2 = new UtmGrid(utm, l);
            Assert.Equal("35X", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        [Fact]
        public void TestConstructor_35X()
        {
            var g = new UtmGrid(utm, 35, 'X');
            Assert.Equal(12.0, g.Width);
            var l = g.LowerRightCorner;
            l.Longitude += 1.0;
            var g2 = new UtmGrid(utm, l);
            Assert.Equal("37X", g2.ToString());
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        [Fact]
        public void TestCorners()
        {
            var g = new UtmGrid(utm, 32, 'U');
            var glrc = new UtmGrid(utm, g.LowerRightCorner);
            Assert.Equal("32U", glrc.ToString());
            var gulc = new UtmGrid(utm, g.UpperLeftCorner);
            Assert.Equal("32U", gulc.ToString());
            var gurc = new UtmGrid(utm, g.UpperRightCorner);
            Assert.Equal("32U", gurc.ToString());
            ValidateCorners(g);
        }

        [Fact]
        public void TestOrdinal()
        {
            var g = new UtmGrid(utm, 32, 'U');
            var ord = g.Ordinal;
            var g2 = new UtmGrid(utm, ord);
            Assert.Equal(g, g2);
            ValidateCorners(g);
            ValidateCorners(g2);
        }

        [Fact]
        public void TestOrdinal2()
        {
            for (var ord = 0; ord < 1200; ord++)
            {
                if (UtmGrid.IsValidOrdinal(ord))
                {
                    var g = new UtmGrid(utm, ord);
                    Assert.Equal(g.Ordinal, ord);
                }
            }
        }

        [Fact]
        public void TestBandSetter()
        {
            var g0 = new UtmGrid(utm, 32, 'U');
            var g = new UtmGrid(utm, 32, 'U')
            {
                Band = 'V'
            };
            Assert.Equal("32V", g.ToString());
            Assert.Equal(g.LowerLeftCorner.Latitude - g0.LowerLeftCorner.Latitude, g0.Height);
            ValidateCorners(g);
        }

        [Fact]
        public void TestZoneSetter()
        {
            var g0 = new UtmGrid(utm, 32, 'U');
            var g = new UtmGrid(utm, 32, 'U')
            {
                Zone = 33
            };
            Assert.Equal("33U", g.ToString());
            Assert.Equal(g.LowerLeftCorner.Longitude - g0.LowerLeftCorner.Longitude, g0.Width);
            ValidateCorners(g);
        }

        [Fact]
        public void TestOriginCompute()
        {
            var g = new UtmGrid(utm, 32, 'U');

            if(g.Origin is UtmCoordinate o)
            {
                Assert.Equal(o.Projection, utm);
                Assert.Equal("32U", o.Grid.ToString());
                Assert.True(Math.Abs(o.ScaleFactor - 1.0) < 0.001);
            }
            
            ValidateCorners(g);
        }

        [Fact]
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
            Assert.True(maxHeight >= minHeight && minHeight > 0.0);
            Assert.True(maxWidth >= minWidth && minWidth > 0.0);
        }

        [Fact]
        public void TestEquals1()
        {
            var s = "123";
            var g = new UtmGrid(utm, 1, 'C');
            Assert.False(g.Equals(s));
        }

        [Fact]
        public void TestEquals2()
        {
            var g1 = new UtmGrid(utm, 1, 'C');
            var g2 = new UtmGrid(utm, 1, 'D')
            {
                Band = 'C'
            };
            Assert.True(g1.Equals(g2));
        }

        [Fact]
        public void TestEquality()
        {
            var g1 = new UtmGrid(utm, 1, 'C');
            var g2 = new UtmGrid(utm, 1, 'D')
            {
                Band = 'C'
            };
            Assert.True(g1 == g2);
        }

        [Fact]
        public void TestInEquality()
        {
            var g1 = new UtmGrid(utm, 1, 'C');
            var g2 = new UtmGrid(utm, 1, 'D');
            Assert.True(g1 != g2);
        }

        [Fact]
        public void TestNeighbors()
        {
            var g = new UtmGrid(utm, Constants.MyHome);
            Assert.True(g.Band == 'U' && g.Zone == 32);
            var n = g.North;
            Assert.True(n.Band == 'V' && n.Zone == 32);
            var s = g.South;
            Assert.True(s.Band == 'T' && s.Zone == 32);
            var w = g.West;
            Assert.True(w.Band == 'U' && w.Zone == 31);
            var e = g.East;
            Assert.True(e.Band == 'U' && e.Zone == 33);
        }

        [Fact]
        public void TestNorth1()
        {
            var g = new UtmGrid(utm, 31, 'U');
            Assert.Throws<Exception>(() => g.North);
        }

        [Fact]
        public void TestNorth2()
        {
            var g = new UtmGrid(utm, 32, 'W');
            Assert.Throws<Exception>(() => g.North);
        }

        [Fact]
        public void TestNorth3()
        {
            var g = new UtmGrid(utm, 34, 'W');
            Assert.Throws<Exception>(() => g.North);
        }

        [Fact]
        public void TestNorth4()
        {
            var g = new UtmGrid(utm, 36, 'W');
            Assert.Throws<Exception>(() => g.North);
        }

        [Fact]
        public void TestNorth5()
        {
            var g = new UtmGrid(utm, 1, 'X');
            Assert.Throws<Exception>(() => g.North);
        }

        [Fact]
        public void TestSouth1()
        {
            var g = new UtmGrid(utm, 31, 'W');
            Assert.Throws<Exception>(() => g.South);
        }

        [Fact]
        public void TestSouth2()
        {
            var g = new UtmGrid(utm, 31, 'X');
            Assert.Throws<Exception>(() => g.South);
        }

        [Fact]
        public void TestSouth3()
        {
            var g = new UtmGrid(utm, 33, 'X');
            Assert.Throws<Exception>(() => g.South);
        }

        [Fact]
        public void TestSouth4()
        {
            var g = new UtmGrid(utm, 35, 'X');
            Assert.Throws<Exception>(() => g.South);
        }

        [Fact]
        public void TestSouth5()
        {
            var g = new UtmGrid(utm, 37, 'X');
            Assert.Throws<Exception>(() => g.South);
        }

        [Fact]
        public void TestSouth6()
        {
            var g = new UtmGrid(utm, 37, 'C');
            Assert.Throws<Exception>(() => g.South);
        }
    }
}
