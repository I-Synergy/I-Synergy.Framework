using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace ISynergy.Framework.Mathematics.Matrices;
[TestClass]
public partial class MatrixTensor
{

    [TestMethod]
    public void squeeze_by_conversion()
    {
        double[,,,] a =
        {
            { { { 1 } }, { { 2 } }, { { 3 } } },
            { { { 4 } }, { { 5 } }, { { 6 } } },
        };

        double[,] expected =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
        };

        object actual = a.To<double[,]>();
        Assert.AreSequenceEqual(expected, (ICollection)actual);

        actual = a.To<double[,,]>();
        Assert.AreNotEqual(expected, (ICollection)actual);

        actual = a.To<double[,,]>().To<double[,]>();
        Assert.AreSequenceEqual(expected, (ICollection)actual);
    }

    [TestMethod]
    public void jagged_to_multidimensional()
    {
        double[][] a = Jagged.Magic(3);

        double[,] expected = Matrix.Magic(3);

        double[,] actual = a.To<double[,]>();
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void string_to_multidimensional()
    {
        string[][] a = Jagged.Magic(3).Apply((x, i, j) => x.ToString());

        double[,] expected = Matrix.Magic(3);

        double[,] actual = a.To<double[,]>();
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void string_to_jagged()
    {
        string[][] a = Jagged.Magic(3).Apply((x, i, j) => x.ToString());
        double[][] expected = Jagged.Magic(3);

        double[][] actual = a.To<double[][]>();

        for (int i = 0; i < actual.GetLength()[0]; i++)
            for (int j = 0; j < actual.GetLength()[1]; j++)
                Assert.AreEqual(expected[i][j], actual[i][j]);
    }

    [TestMethod]
    public void expand_and_squeeze()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
        };


        double[,,] actual0 = a.ExpandDimensions(0).To<double[,,]>();
        double[,,] actual1 = a.ExpandDimensions(1).To<double[,,]>();
        double[,,] actual2 = a.ExpandDimensions(2).To<double[,,]>();

        double[,,] expected0 =
        {
            { { 1, 2, 3 },
              { 4, 5, 6 } },
        };

        double[,,] expected1 =
        {
            { { 1, 2, 3 } },
            { { 4, 5, 6 } },
        };

        double[,,] expected2 =
        {
              { { 1 }, { 2 }, { 3 } },
              { { 4 }, { 5 }, { 6 } },
        };


        Assert.AreSequenceEqual(actual0, expected0);
        Assert.AreSequenceEqual(actual1, expected1);
        Assert.AreSequenceEqual(actual2, expected2);

        // Test squeeze
        Assert.AreSequenceEqual(a, expected0.Squeeze());
        Assert.AreSequenceEqual(a, expected1.Squeeze());
        Assert.AreSequenceEqual(a, expected2.Squeeze());
    }

    [TestMethod]
    public void flatten_and_reshape()
    {
        double[] expected, actual;
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
        };

        expected = a.Flatten(MatrixOrder.CRowMajor);
        actual = (double[])((Array)a).Flatten(MatrixOrder.CRowMajor);
        Assert.AreSequenceEqual(expected, actual);
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.CRowMajor));
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.CRowMajor));

        expected = a.Flatten(MatrixOrder.FortranColumnMajor);
        actual = (double[])((Array)a).Flatten(MatrixOrder.FortranColumnMajor);
        Assert.AreSequenceEqual(expected, actual);
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));


        double[,,,] b =
        {
            { { { 1 }, { 2 }, { 3 } } },
            { { { 4 }, { 5 }, { 6 } } },
        };

        expected = a.Flatten(MatrixOrder.CRowMajor);
        actual = (double[])b.Flatten(MatrixOrder.CRowMajor);
        Assert.AreSequenceEqual(expected, actual);
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.CRowMajor));
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.CRowMajor));

        expected = a.Flatten(MatrixOrder.FortranColumnMajor);
        actual = (double[])b.Flatten(MatrixOrder.FortranColumnMajor);
        Assert.AreSequenceEqual(expected, actual);
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));
        Assert.AreSequenceEqual(a, actual.Reshape(a.GetLength(), MatrixOrder.FortranColumnMajor));
    }

    [TestMethod]
    public void transpose()
    {
        double[,,,] target =
        {
            { { { 1 }, { 2 }, { 3 } } },
            { { { 4 }, { 5 }, { 6 } } },
        };

        double[,,,] expected =
        {
            { { { 1, 4 } } ,
              { { 2, 5 } } ,
              { { 3, 6 } } },
        };

        Array actual = target.Transpose();
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void convert_with_squeeze()
    {
        double[,,,] a =
        {
            { { { 1 } }, { { 2 } }, { { 3 } } },
            { { { 4 } }, { { 5 } }, { { 6 } } },
        };

        int[,] expected =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
        };

        int[,] actual = a.To<int[,]>();

        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public void convert_to_scalar()
    {
        double[,,,] a =
        {
            { { { 1 } } },
        };

        int expected = 1;
        int actual = a.To<int>();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void convert_to_bool_true()
    {
        double[,,,] a =
        {
            { { { 1 } } },
        };

        bool expected = true;
        bool actual = a.To<bool>();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void convert_to_bool_false()
    {
        double[,,,] a =
        {
            { { { 0 } } },
        };

        bool expected = false;
        bool actual = a.To<bool>();
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void slice_dimension()
    {
        double[,] x =
        {
            { 0, 1 },
            { 1, 2 },
            { 2, 3 },
            { 3, 4 },
            { 4, 5 },
            { 5, 6 },
            { 6, 7 },
            { 7, 8 },
            { 8, 9 },
            { 9, 0 },
        };


        {
            double[,] r = (double[,])x.Get(dimension: 0, indices: [0, 1]);

            double[,] expected =
            {
                { 0, 1 },
                { 1, 2 },
            };

            Assert.AreSequenceEqual(expected, r);
        }

        {
            double[,] r = (double[,])x.Get(dimension: 0, indices: [1, 2, 3]);

            double[,] expected =
            {
                { 1, 2 },
                { 2, 3 },
                { 3, 4 },
            };

            Assert.AreSequenceEqual(expected, r);
        }

        {
            double[,] r = (double[,])x.Get(dimension: 0, indices: [9, 6, 3, 2]);

            double[,] expected =
            {
                { 9, 0 },
                { 6, 7 },
                { 3, 4 },
                { 2, 3 },
            };

            Assert.AreSequenceEqual(expected, r);
        }
    }

    private static readonly int[] expected = new[] { 2, 3 };
    private static readonly int[] expectedArray = new[] { 2, 3 };
    private static readonly int[] expectedArray0 = new[] { 2, 3 };
    private static readonly int[] expectedArray1 = new[] { 2, 3 };

    [TestMethod]
    public void create_as()
    {
        double[,] a =
        {
            {1, 2, 3 },
            {3, 4, 5 }
        };

        Array actual = Jagged.CreateAs(a, typeof(int));
        Assert.AreSequenceEqual(expected, actual.GetLength());

        actual = Jagged.CreateAs(a.ToJagged(), typeof(int));
        Assert.AreSequenceEqual(expectedArray, actual.GetLength());

        actual = Matrix.CreateAs(a, typeof(int));
        Assert.AreSequenceEqual(expectedArray0, actual.GetLength());

        actual = Matrix.CreateAs(a.ToJagged(), typeof(int));
        Assert.AreSequenceEqual(expectedArray1, actual.GetLength());
    }

}
