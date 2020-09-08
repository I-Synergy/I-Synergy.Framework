using System;
using Xunit;

namespace ISynergy.Framework.Geography.Common.Tests
{
    public class AngleTests
    {
        [Fact]
        public void TestConstructor1()
        {
            var a = new Angle(90);
            Assert.Equal(90, a.Degrees);
        }

        [Fact]
        public void TestConstructor2()
        {
            var a = new Angle(45, 30);
            Assert.Equal(45.5, a.Degrees);
        }

        [Fact]
        public void TestConstructor3()
        {
            var a = new Angle(-45, 30);
            Assert.Equal(-45.5, a.Degrees);
        }

        [Fact]
        public void TestConstructor4()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Angle(45, 60));
        }

        [Fact]
        public void TestConstructor5()
        {
            var a = new Angle(45, 30, 30);
            Assert.Equal(a.Degrees, 45.5 + (1.0 / 120.0));
        }

        [Fact]
        public void TestConstructor6()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Angle(45, 30, 60));
        }

        [Fact]
        public void TestConstructor7()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Angle(45, 60, 30));
        }

        [Fact]
        public void TestConstructor8()
        {
            var a = new Angle(-45, 30, 30);
            Assert.Equal(a.Degrees, -45.5 - (1.0 / 120.0));
        }

        [Fact]
        public void TestConstructor9()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Angle(45, -1, 30));
        }

        [Fact]
        public void TestDegreeSetter()
        {
            var a = new Angle(0);
            Assert.Equal(0, a.Degrees);
            a.Degrees = 90.0;
            Assert.Equal(90, a.Degrees);
        }

        [Fact]
        public void TestRadiansGetter()
        {
            var a = new Angle(180);
            Assert.Equal(a.Radians, Math.PI);
        }

        [Fact]
        public void TestRadiansSetter()
        {
            var a = new Angle(0);
            Assert.Equal(0, a.Degrees);
            a.Radians = Math.PI;
            Assert.Equal(180, a.Degrees);
        }

        [Fact]
        public void TestAbs()
        {
            var a = new Angle(-180);
            Assert.Equal(a.Degrees, -180);
            var b = a.Abs();
            Assert.Equal(180, b.Degrees);
        }

        [Fact]
        public void TestCompareTo1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.Equal(0, a.CompareTo(a));
            Assert.Equal(a.CompareTo(b), -1);
            Assert.Equal(1, b.CompareTo(a));
        }

        [Fact]
        public void TestCompareTo2()
        {
            var x = 180.0;
            var y = x + 1e-13;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.Equal(0, a.CompareTo(a));
            Assert.Equal(0, a.CompareTo(b));
            Assert.Equal(0, b.CompareTo(a));
        }

        [Fact]
        public void TestEquals1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.False(a.Equals(b));
            b = new Angle(x);
            Assert.True(a.Equals(b));
        }

        [Fact]
        public void TestEquals2()
        {
            var a = new Angle(180);
            object s = "180";
            Assert.False(a.Equals(null));
            Assert.False(a.Equals(s));
        }

        [Fact]
        public void TestToString()
        {
            var a = new Angle(45);
            var s = a.ToString();
            Assert.Equal("45", s);
        }

        [Fact]
        public void TestLessEqual1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.True(a <= b);
        }

        [Fact]
        public void TestLessEqual2()
        {
            var x = 180.0;
            var y = 180.0 + 1e-13;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.True(a <= b);
        }

        [Fact]
        public void TestGreaterEqual1()
        {
            var x = 180.0;
            var y = 180.00000001;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.True(b >= a);
        }

        [Fact]
        public void TestGreaterEqual2()
        {
            var x = 180.0;
            var y = 180.0 + 1e-13;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.True(b >= a);
        }

        [Fact]
        public void TestOperatorAdd()
        {
            var a = new Angle(45);
            var b = new Angle(60);
            var c = a + b;
            Assert.Equal(105, c.Degrees);
        }

        [Fact]
        public void TestOperatorSub()
        {
            var a = new Angle(45);
            var b = new Angle(60);
            var c = a - b;
            Assert.Equal(c.Degrees, -15);
        }

        [Fact]
        public void TestOperatorMul()
        {
            var a = new Angle(45);
            var b = 2 * a;
            var c = a * 3;
            Assert.Equal(90, b.Degrees);
            Assert.Equal(135, c.Degrees);
        }

        [Fact]
        public void TestGreaterThan1()
        {
            var x = 180.00000001;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.True(a > b);
        }

        [Fact]
        public void TestGreaterThan2()
        {
            var x = 180.0 + 1e-12;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.False(a > b);
        }

        [Fact]
        public void TestNotEqual1()
        {
            var x = 180.00000001;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.True(a != b);
        }

        [Fact]
        public void TestNotEqual2()
        {
            var x = 180.0 + 1e-12;
            var y = 180.0;
            var a = new Angle(x);
            var b = new Angle(y);
            Assert.False(a != b);
        }

        [Fact]
        public void TestNegate()
        {
            var a = new Angle(180);
            var b = -a;
            Assert.Equal(b.Degrees, -180);
        }

        [Fact]
        public void TestImplicitConversion()
        {
            Angle a = 44.0;
            Assert.Equal(44, a.Degrees);
        }

        [Fact]
        public void TestDeg2Rad()
        {
            var a = Angle.DegToRad(90);
            Assert.Equal(a, Math.PI / 2.0);
        }

        [Fact]
        public void TestRadToDeg()
        {
            var a = Angle.RadToDeg(Math.PI);
            Assert.Equal(180, a);
        }

        [Fact]
        public void TestHashCode()
        {
            var a = new Angle(0);
            var b = new Angle(0.000000001);
            Assert.True(a.GetHashCode() != b.GetHashCode());
        }
    }
}
