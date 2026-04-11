using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Core;

[TestClass]
public class Vector3Test
{
    [TestMethod]
    public void ConstructorTest()
    {
        Vector3 v1 = new(1, 2, 3);
        Vector3 v2 = new(-1, -2, -3);
        Vector3 v3 = new(7);

        Assert.AreEqual(1, v1.X);
        Assert.AreEqual(2, v1.Y);
        Assert.AreEqual(3, v1.Z);

        Assert.AreEqual(-1, v2.X);
        Assert.AreEqual(-2, v2.Y);
        Assert.AreEqual(-3, v2.Z);

        Assert.AreEqual(7, v3.X);
        Assert.AreEqual(7, v3.Y);
        Assert.AreEqual(7, v3.Z);
    }

    [DataTestMethod]
    [DataRow(0, 0, 0, 0, 0, 0, 0)]
    [DataRow(0, 7, 7, 0, 7, 0, 1)]
    [DataRow(0, 0, 7, 0, 7, 0, 2)]
    [DataRow(0, 7, 0, 0, 7, 0, 1)]
    [DataRow(7, 0, 7, 0, 7, 1, 0)]
    [DataRow(5, 7, 9, 5, 9, 0, 2)]
    [DataRow(5, 9, 7, 5, 9, 0, 1)]
    [DataRow(7, 5, 9, 5, 9, 1, 2)]
    [DataRow(7, 9, 5, 5, 9, 2, 1)]
    [DataRow(9, 5, 7, 5, 9, 1, 0)]
    [DataRow(9, 7, 5, 5, 9, 2, 0)]
    public void MinMaxTest(float x, float y, float z, float expectedMin, float expectedMax,
        int expectedMinIndex, int expectedMaxIndex)
    {
        Vector3 vector = new(x, y, z);

        Assert.AreEqual(expectedMin, vector.Min);
        Assert.AreEqual(expectedMax, vector.Max);

        Assert.AreEqual(expectedMinIndex, vector.MinIndex);
        Assert.AreEqual(expectedMaxIndex, vector.MaxIndex);
    }

    [DataTestMethod]
    [DataRow(0, 0, 0, 0)]
    [DataRow(1, 0, 0, 1)]
    [DataRow(0, 2, 0, 2)]
    [DataRow(0, 0, 3, 3)]
    [DataRow(3, 4, 0, 5)]
    [DataRow(-3, -4, 0, 5)]
    public void NormTest(float x, float y, float z, float expectedNorm)
    {
        Vector3 vector = new(x, y, z);

        float norm = vector.Norm;

        Assert.AreEqual(expectedNorm, norm);
        Assert.AreEqual(vector.Square, norm * norm);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 1, 2, 3, true)]
    [DataRow(-1, -2, -3, -1, -2, -3, true)]
    [DataRow(-1, -2, -3, -1, -2, 3, false)]
    public void EqualityTest(float x1, float y1, float z1, float x2, float y2, float z2, bool expected)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);

        Assert.AreEqual(expected, vector1 == vector2);
        Assert.AreEqual(!expected, vector1 != vector2);

        Assert.AreEqual(expected, vector1.Equals(vector2));
        Assert.AreEqual(expected, vector1.Equals((object)vector2));
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 4, 5, 6, 5, 7, 9)]
    [DataRow(1, 2, 3, -4, -5, -6, -3, -3, -3)]
    public void AdditionTest(float x1, float y1, float z1, float x2, float y2, float z2,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector1 + vector2;
        Vector3 result2 = Vector3.Add(vector1, vector2);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 4, 5, 6, 7)]
    [DataRow(1, 2, 3, -4, -3, -2, -1)]
    public void AdditionWithConstTest(float x, float y, float z, float value,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector = new(x, y, z);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector + value;
        Vector3 result2 = Vector3.Add(vector, value);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 4, 5, 6, -3, -3, -3)]
    [DataRow(1, 2, 3, -4, -5, -6, 5, 7, 9)]
    public void SubtractionTest(float x1, float y1, float z1, float x2, float y2, float z2,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector1 - vector2;
        Vector3 result2 = Vector3.Subtract(vector1, vector2);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 4, -3, -2, -1)]
    [DataRow(1, 2, 3, -4, 5, 6, 7)]
    public void SubtractionWithConstTest(float x, float y, float z, float value,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector = new(x, y, z);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector - value;
        Vector3 result2 = Vector3.Subtract(vector, value);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 4, 5, 6, 4, 10, 18)]
    [DataRow(1, 2, 3, -4, -5, -6, -4, -10, -18)]
    public void MultiplicationTest(float x1, float y1, float z1, float x2, float y2, float z2,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector1 * vector2;
        Vector3 result2 = Vector3.Multiply(vector1, vector2);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 4, 4, 8, 12)]
    [DataRow(1, 2, 3, -4, -4, -8, -12)]
    public void MultiplicationWithConstTest(float x, float y, float z, float value,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector = new(x, y, z);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector * value;
        Vector3 result2 = Vector3.Multiply(vector, value);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1f, 2f, 3f, 1f, 4f, 2f, 1f, 0.5f, 1.5f)]
    [DataRow(1f, 2f, 3f, -1f, -4f, -2f, -1f, -0.5f, -1.5f)]
    public void DivisionTest(float x1, float y1, float z1, float x2, float y2, float z2,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector1 / vector2;
        Vector3 result2 = Vector3.Divide(vector1, vector2);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1f, 2f, 3f, 2f, 0.5f, 1f, 1.5f)]
    [DataRow(1f, 2f, 3f, -2f, -0.5f, -1f, -1.5f)]
    public void DivisionWithConstTest(float x, float y, float z, float value,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector = new(x, y, z);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result1 = vector / value;
        Vector3 result2 = Vector3.Divide(vector, value);

        Assert.IsTrue(expectedResult == result1);
        Assert.IsTrue(expectedResult == result2);
    }

    [DataTestMethod]
    [DataRow(1, 0, 0, 1, 0, 0)]
    [DataRow(0, 1, 0, 0, 1, 0)]
    [DataRow(0, 0, 1, 0, 0, 1)]
    [DataRow(3f, 4f, 0f, 0.6f, 0.8f, 0f)]
    [DataRow(3f, 0f, 4f, 0.6f, 0f, 0.8f)]
    public void NormalizeTest(float x, float y, float z, float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector = new(x, y, z);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        float norm1 = vector.Norm;
        float norm2 = vector.Normalize();

        Assert.IsTrue(expectedResult == vector);
        Assert.AreEqual(norm1, norm2);
    }

    [DataTestMethod]
    [DataRow(1, 0, 0, 1, 0, 0)]
    [DataRow(0, 0, 0, 0, 0, 0)]
    [DataRow(2f, 4f, 8f, 0.5f, 0.25f, 0.125f)]
    [DataRow(-2f, -4f, -8f, -0.5f, -0.25f, -0.125f)]
    [DataRow(0.5f, 0.25f, 0.125f, 2f, 4f, 8f)]
    public void InverseTest(float x, float y, float z, float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector = new(x, y, z);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Vector3 result = vector.Inverse();

        Assert.IsTrue(expectedResult == result);
    }

    [DataTestMethod]
    [DataRow(1, 2, 3, 0, 0, 0, 0)]
    [DataRow(1, 2, 3, 1, 1, 1, 6)]
    [DataRow(1, 2, 3, 3, 2, 1, 10)]
    [DataRow(1, 2, 3, -3, -2, -1, -10)]
    public void DotTest(float x1, float y1, float z1, float x2, float y2, float z2, float expectedResult)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);

        Assert.AreEqual(expectedResult, Vector3.Dot(vector1, vector2));
    }

    [DataTestMethod]
    [DataRow(1, 0, 0, 0, 1, 0, 0, 0, 1)]
    [DataRow(1, 1, 1, 1, 1, 1, 0, 0, 0)]
    [DataRow(1, 2, 3, 4, 5, 6, -3, 6, -3)]
    public void CrossTest(float x1, float y1, float z1, float x2, float y2, float z2,
        float expectedX, float expectedY, float expectedZ)
    {
        Vector3 vector1 = new(x1, y1, z1);
        Vector3 vector2 = new(x2, y2, z2);
        Vector3 expectedResult = new(expectedX, expectedY, expectedZ);

        Assert.IsTrue(Vector3.Cross(vector1, vector2) == expectedResult);
    }
}
