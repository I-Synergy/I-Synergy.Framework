using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Points.Tests;

[TestClass]
public class PointTest
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
    [DataRow(0.3f, 0.4f, 0.5f)]
    public void EuclideanNormTest(float x, float y, float expectedNorm)
    {
        Point point = new(x, y);
        Assert.AreEqual(expectedNorm, point.EuclideanNorm(true, 1));
    }

    [DataTestMethod]
    [DataRow(0, 0, 0, 0)]
    [DataRow(1, 2, 1, 2)]
    [DataRow(-1, -2, -1, -2)]
    [DataRow(1.4f, 3.3f, 1, 3)]
    [DataRow(1.6f, 3.7f, 2, 4)]
    [DataRow(-1.6f, -3.3f, -2, -3)]
    [DataRow(-1.5f, 1.5f, -2, 2)]
    [DataRow(-2.5f, 2.5f, -2, 2)]
    public void RoundTest(float x, float y, int expectedX, int expectedY)
    {
        Point point1 = new(x, y, true);
        Point point2 = new(expectedX, expectedY);

        Assert.AreEqual(point2, point1);
    }

    [DataRow(1.1f, 2.2f, 1.1f, 2.2f, true)]
    [DataRow(1.1f, 2.2f, 3.3f, 2.2f, false)]
    [DataRow(1.1f, 2.2f, 1.1f, 4.4f, false)]
    [DataRow(1.1f, 2.2f, 3.3f, 4.4f, false)]
    public void EqualityOperatorTest(float x1, float y1, float x2, float y2, bool areEqual)
    {
        Point point1 = new(x1, y1);
        Point point2 = new(x2, y2);

        Assert.AreEqual(point1 == point2, areEqual);
    }

    [DataTestMethod]
    [DataRow(1.1f, 2.2f, 1.1f, 2.2f, false)]
    [DataRow(1.1f, 2.2f, 3.3f, 2.2f, true)]
    [DataRow(1.1f, 2.2f, 1.1f, 4.4f, true)]
    [DataRow(1.1f, 2.2f, 3.3f, 4.4f, true)]
    public void InequalityOperatorTest(float x1, float y1, float x2, float y2, bool areNotEqual)
    {
        Point point1 = new(x1, y1);
        Point point2 = new(x2, y2);

        Assert.AreEqual(point1 != point2, areNotEqual);
    }
}
