namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics;
using ISynergy.Framework.Mathematics.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public partial class MatrixTest
{

    #region Reshape
    [TestMethod]
    public void ReshapeTest()
    {
        int[,] start =
        {
            { 1, 3, 5},
            { 2, 4, 6},
        };

        int[] middle = Matrix.Reshape(start);

        int[] array = { 1, 3, 5, 2, 4, 6 };
        Assert.IsTrue(Matrix.IsEqual(array, middle));

        int rows = 3;
        int cols = 2;

        int[,] expected =
        {
            { 1, 3 },
            { 5, 2 },
            { 4, 6 },
        };

        int[,] actual = Matrix.Reshape(array, rows, cols);
        Assert.IsTrue(Matrix.IsEqual(expected, actual));

        rows = 2;
        cols = 3;

        actual = Matrix.Reshape(array, rows, cols);
        Assert.IsTrue(Matrix.IsEqual(start, actual));
    }

    [TestMethod]
    public void ReshapeTest1()
    {
        double[,] array =
        {
            { 1, 2, 3},
            { 4, 5, 6},
        };

        int dimension = 1;
        double[] expected = { 1, 2, 3, 4, 5, 6 };
        double[] actual = Matrix.Reshape(array, (MatrixOrder)dimension);
        Assert.IsTrue(Matrix.IsEqual(expected, actual));

        dimension = 0;
        expected = new double[] { 1, 4, 2, 5, 3, 6 };
        actual = Matrix.Reshape(array, (MatrixOrder)dimension);
        Assert.IsTrue(Matrix.IsEqual(expected, actual));
    }

    [TestMethod]
    public void ReshapeTest2()
    {
        double[][] array =
        {
            new double[] { 1, 2, 3 },
            new double[] { 4, 5, 6 }
        };

        {
            double[] expected = { 1, 2, 3, 4, 5, 6 };
            double[] actual = Matrix.Reshape(array, (MatrixOrder)1);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        {
            double[] expected = { 1, 4, 2, 5, 3, 6 };
            double[] actual = Matrix.Reshape(array, 0);
            Assert.IsTrue(expected.IsEqual(actual));
        }
    }

    [TestMethod]
    public void ReshapeTest3()
    {
        double[][] array =
        {
            new double[] { 1, 2 },
            new double[] { 3, 4, 5, 6 }
        };

        {
            double[] expected = { 1, 2, 3, 4, 5, 6 };
            double[] actual = Matrix.Reshape(array);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        {
            double[] expected = { 1, 2, 3, 4, 5, 6 };
            double[] actual = Matrix.Reshape(array, (MatrixOrder)1);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        {
            double[] expected = { 1, 3, 2, 4, 5, 6 };
            double[] actual = Matrix.Reshape(array, 0);
            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
    #endregion


    [TestMethod]
    public void GetLengthTest()
    {
        double[][] a = Jagged.Zeros(1, 1);
        double[,] b = Matrix.Zeros(1, 1);
        int[] actual = a.GetLength();
        int[] expected = b.GetLength();
        Assert.AreEqual(actual.Length, 2);
        Assert.AreEqual(actual[0], 1);
        Assert.AreEqual(actual[1], 1);
        Assert.AreEqual(expected.Length, 2);
        Assert.AreEqual(expected[0], 1);
        Assert.AreEqual(expected[1], 1);
        Assert.IsTrue(actual.IsEqual(expected));
    }

    [TestMethod]
    public void GetLengthTest2()
    {
        int[] a = new int[0];
        double[] b = new double[0];
        int[] actual = a.GetLength();
        int[] expected = b.GetLength();
        Assert.AreEqual(actual.Length, 0);
        Assert.AreEqual(expected.Length, 0);
        Assert.IsTrue(actual.IsEqual(expected));
    }

    [TestMethod]
    public void RelativelyEqualsTest()
    {
        Assert.IsFalse(double.PositiveInfinity.IsEqual(1, 1e-10));
        Assert.IsFalse(1.0.IsEqual(double.PositiveInfinity, 1e-10));

        Assert.IsFalse(double.NegativeInfinity.IsEqual(1, 1e-10));
        Assert.IsFalse(1.0.IsEqual(double.NegativeInfinity, 1e-10));

        Assert.IsFalse(double.PositiveInfinity.IsEqual(double.NegativeInfinity, 1e-10));
        Assert.IsFalse(double.NegativeInfinity.IsEqual(double.PositiveInfinity, 1e-10));

        Assert.IsTrue(double.PositiveInfinity.IsEqual(double.PositiveInfinity, 1e-10));
        Assert.IsTrue(double.NegativeInfinity.IsEqual(double.NegativeInfinity, 1e-10));

        Assert.IsTrue(1.0.IsEqual(1.1, 0.11));
        Assert.IsTrue(1.1.IsEqual(1.0, 0.11));

        Assert.IsFalse(0.0.IsEqual(1.1, 0.11));
        Assert.IsFalse(1.1.IsEqual(0.0, 0.11));

        Assert.IsFalse(1.0.IsEqual(1.2, 0.11));
        Assert.IsFalse(1.2.IsEqual(1.0, 0.11));

    }

    [TestMethod]
    public void AddAxisTest()
    {
        int[,] m = Vector.Range(0, 15).Reshape(3, 5);

        int[,] actual = m.Add(5);
        Assert.IsTrue(actual.IsEqual(new int[,]
        {
            { 5,    6,   7,  8,  9 },
            { 10,  11,  12, 13, 14 },
            { 15,  16,  17, 18, 19 },
        }));

        actual = m.Add(new[] { 10, 20, 30 }, dimension: VectorType.ColumnVector);
        Assert.IsTrue(actual.IsEqual(new int[,]
        {
            { 10,   11,  12,  13,  14 },
            { 25,   26,  27,  28,  29 },
            { 40,   41,  42,  43,  44 },
        }));

        actual = m.Add(new[] { 10, 20, 30, 40, 50 }, dimension: 0);
        Assert.IsTrue(actual.IsEqual(new int[,]
        {
            { 10,   21,  32,  43,  54 },
            { 15,   26,  37,  48,  59 },
            { 20,   31,  42,  53,  64 },
        }));
    }

    [TestMethod]
    public void GetSymmetricTest()
    {
        int[,] m =
        {
            { 1, 2, 3 },
            { 0, 1, 2 },
            { 0, 0, 1 }
        };

        int[,] expected =
        {
            { 1, 2, 3 },
            { 2, 1, 2 },
            { 3, 2, 1 }
        };

        CollectionAssert.AreEqual(expected, m.GetSymmetric(MatrixType.UpperTriangular));
    }
}
