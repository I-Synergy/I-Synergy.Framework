namespace ISynergy.Framework.Mathematics.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Globalization;
    using ISynergy.Framework.Mathematics.Geometry;

    [TestClass]
    public class PlaneTest
    {

        [TestMethod]
        public void FromPointsTest()
        {
            Point3 point1 = new Point3(0, 1, -7);
            Point3 point2 = new Point3(3, 1, -9);
            Point3 point3 = new Point3(0, -5, -8);

            Plane actual = Plane.FromPoints(point1, point2, point3);
            Vector3 expected = new Vector3(-12, 3, -18);

            Assert.AreEqual(expected, actual.Normal);
        }

        [TestMethod]
        public void FromPointsTest2()
        {
            Point3 point1 = new Point3(1, 2, -2);
            Point3 point2 = new Point3(3, -2, 1);
            Point3 point3 = new Point3(5, 1, -4);

            Plane expected = new Plane(11, 16, 14, -15);
            Plane actual = Plane.FromPoints(point1, point2, point3);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Plane target = new Plane(-12, 3, -18, 1);

            {
                string expected = "-12x +3y -18z +1 = 0";
                string actual = target.ToString();
                Assert.AreEqual(expected, actual);
            }

            {
                string expected = "x = +0.25y -1.5z +0.083333336";
                string actual = target.ToString('x', CultureInfo.InvariantCulture);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
