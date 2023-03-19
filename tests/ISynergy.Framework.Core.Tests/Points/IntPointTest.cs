using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Points.Tests
{
    [TestClass]
    public class IntPointTest
    {
        [DataTestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 1, 1)]
        [DataRow(0, 10, 10)]
        [DataRow(10, 0, 10)]
        [DataRow(3, 4, 5)]
        [DataRow(-3, 4, 5)]
        [DataRow(3, -4, 5)]
        [DataRow(-3, -4, 5)]
        public void EuclideanNormTest(int x, int y, double expectedNorm)
        {
            Point point = new(x, y);
            Assert.AreEqual(point.EuclideanNorm(), expectedNorm);
        }

        [DataTestMethod]
        [DataRow(1, 2, 1, 2, true)]
        [DataRow(1, 2, 3, 2, false)]
        [DataRow(1, 2, 1, 4, false)]
        [DataRow(1, 2, 3, 4, false)]
        public void EqualityOperatorTest(int x1, int y1, int x2, int y2, bool areEqual)
        {
            Point point1 = new(x1, y1);
            Point point2 = new(x2, y2);

            Assert.AreEqual(point1 == point2, areEqual);
        }

        [DataTestMethod]
        [DataRow(1, 2, 1, 2, false)]
        [DataRow(1, 2, 3, 2, true)]
        [DataRow(1, 2, 1, 4, true)]
        [DataRow(1, 2, 3, 4, true)]
        public void InequalityOperatorTest(int x1, int y1, int x2, int y2, bool areNotEqual)
        {
            Point point1 = new(x1, y1);
            Point point2 = new(x2, y2);

            Assert.AreEqual(point1 != point2, areNotEqual);
        }
    }
}
