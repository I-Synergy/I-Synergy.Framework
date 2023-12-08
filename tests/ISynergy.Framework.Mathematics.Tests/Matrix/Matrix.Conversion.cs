namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

public partial class MatrixTest
{
    [TestMethod]
    public void ToTableTest()
    {
        double[,] matrix =
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 },
        };

        string[] columnNames = { "A", "B", };
        DataTable actual = Matrix.ToTable(matrix, columnNames);

        Assert.AreEqual("A", actual.Columns[0].ColumnName);
        Assert.AreEqual("B", actual.Columns[1].ColumnName);

        Assert.AreEqual(1, (double)actual.Rows[0][0]);
        Assert.AreEqual(2, (double)actual.Rows[0][1]);
        Assert.AreEqual(3, (double)actual.Rows[1][0]);
        Assert.AreEqual(4, (double)actual.Rows[1][1]);
        Assert.AreEqual(5, (double)actual.Rows[2][0]);
        Assert.AreEqual(6, (double)actual.Rows[2][1]);
    }

    [TestMethod]
    public void TableToJaggedTest()
    {
        #region doc_table_tojagged
        // Create a sample data table with two columns
        DataTable table = new("My DataTable");
        table.Columns.Add("x");
        table.Columns.Add("y");

        // Add some values to it
        table.Rows.Add(0.3, 0.1);
        table.Rows.Add(4.2, 1.4);
        table.Rows.Add(7.3, 8.7);

        // Convert to a jagged array
        double[][] a = table.ToJagged();
        // result will be
        // double[][] a =
        // {
        //     new[] { 0.3, 0.1 },
        //     new[] { 4.2, 1.4 },
        //     new[] { 7.3, 8.7 },
        // };

        // Convert only some of the columns
        double[][] b = table.ToJagged("x");
        // result will be
        // double[][] b =
        // {
        //     new[] { 0.3 },
        //     new[] { 4.2 },
        //     new[] { 7.3 },
        // };

        // Extract column names together with values
        string[] names;
        double[][] c = table.ToJagged(out names);
        // result will be
        // string[] names = { "x", "y" };
        // double[][] c =
        // {
        //     new[] { 0.3, 0.1 },
        //     new[] { 4.2, 1.4 },
        //     new[] { 7.3, 8.7 },
        // };

        // Convert to a particular format
        float[][] d = table.ToJagged<float>();
        // result will be
        // string[] names = { "x", "y" };
        // float[][] c =
        // {
        //     new[] { 0.3, 0.1 },
        //     new[] { 4.2, 1.4 },
        //     new[] { 7.3, 8.7 },
        // };

        // Convert some columns to a particular format
        decimal[][] e = table.ToJagged<decimal>("y");
        // result will be
        // decimal[][] e =
        // {
        //     new[] { 0.1 },
        //     new[] { 1.4 },
        //     new[] { 8.7 },
        // };
        #endregion

        double[][] ea =
        {
            new[] { 0.3, 0.1 },
            new[] { 4.2, 1.4 },
            new[] { 7.3, 8.7 },
        };

        for (int i = 0; i < a.GetLength()[0]; i++)
            for (int j = 0; j < a.GetLength()[1]; j++)
                Assert.AreEqual(ea[i][j], a[i][j]);

        for (int i = 0; i < b.GetLength()[0]; i++)
            for (int j = 0; j < b.GetLength()[1]; j++)
                Assert.AreEqual(ea.GetColumn(0).ToJagged()[i][j], b[i][j]);

        for (int i = 0; i < c.GetLength()[0]; i++)
            for (int j = 0; j < c.GetLength()[1]; j++)
                Assert.AreEqual(ea[i][j], c[i][j]);

        for (int i = 0; i < d.GetLength()[0]; i++)
            for (int j = 0; j < d.GetLength()[1]; j++)
                Assert.AreEqual(ea.ToSingle()[i][j], d[i][j]);

        for (int i = 0; i < e.GetLength()[0]; i++)
            for (int j = 0; j < e.GetLength()[1]; j++)
                Assert.AreEqual(ea.GetColumn(1).ToDecimal().ToJagged()[i][j], e[i][j]);
    }

    [TestMethod]
    public void ToTableTest2()
    {
        double[][] matrix =
        {
            new double[] { 1, 2 },
            new double[] { 3, 4 },
            new double[] { 5, 6 },
        };

        DataTable actual = Matrix.ToTable(matrix);

        Assert.AreEqual(1, (double)actual.Rows[0][0]);
        Assert.AreEqual(2, (double)actual.Rows[0][1]);
        Assert.AreEqual(3, (double)actual.Rows[1][0]);
        Assert.AreEqual(4, (double)actual.Rows[1][1]);
        Assert.AreEqual(5, (double)actual.Rows[2][0]);
        Assert.AreEqual(6, (double)actual.Rows[2][1]);
    }

    [TestMethod]
    public void FromTableToArrayTest()
    {
        DataTable table = new();
        table.Columns.Add("A", typeof(bool));
        table.Columns.Add("B", typeof(string));
        table.Rows.Add(true, "1.0");
        table.Rows.Add(true, "0");
        table.Rows.Add(false, "1");
        table.Rows.Add(false, "0.0");

        double[][] actual = table.ToJagged(System.Globalization.CultureInfo.InvariantCulture);
        double[][] expected =
        {
            new double[] { 1, 1 },
            new double[] { 1, 0 },
            new double[] { 0, 1 },
            new double[] { 0, 0 },
        };

        Assert.IsTrue(expected.IsEqual(actual));
    }

    [TestMethod]
    public void FromJaggedToMultidimensional()
    {
        // Declare a jagged matrix that we would like to convert to multi-dimensional
        int[][][] jagged = new[]
        {
            new[]
            {
                new[] { 1, 2, 3 },
                new[] { 4, 5, 6 }
            },

            new[]
            {
                new[] { 7, 8, 9 },
                new[] { 10, 11, 12 }
            },

            new[]
            {
                new[] { 13, 14, 15 },
                new[] { 16, 17, 18 }
            },

            new[]
            {
                new[] { 19, 20, 21 },
                new[] { 22, 23, 24 }
            }
        };

        // Test #1: Transform jagged matrix to a unidimensional vector. The extension method called DeepFlatten is available at:
        // https://github.com/accord-net/framework/blob/a195ce7afbd2fd2ae143a82f5214a08e2a1a2a07/Sources/ISynergy.Framework.Mathematics/Matrix/Matrix.Common.cs#L1636
        Array values = jagged.DeepFlatten();

        // As a result, the resulting array is a simple int[] vector:
        Assert.AreEqual(values.GetType(), typeof(int[]));

        // Some more checks
        int[] innerValues = values as int[];
        Assert.AreEqual(24, innerValues.Length);
        CollectionAssert.AreEqual(new[] { 24 }, innerValues.GetLength());


        // Test #2: Transform the jagged matrix to a multidimensional matrix. The extension method called DeepToMatrix is available at:
        // https://github.com/accord-net/framework/blob/a195ce7afbd2fd2ae143a82f5214a08e2a1a2a07/Sources/ISynergy.Framework.Mathematics/Matrix/Matrix.Conversions.cs#L39
        Array matrix = jagged.DeepToMatrix();

        // As a result, the resulting array is a multidimensional int[,,] array:
        Assert.AreEqual(matrix.GetType(), typeof(int[,,]));

        // Some more checks
        int[,,] innerMatrix = matrix as int[,,];
        int[] shape = innerMatrix.GetLength();
        CollectionAssert.AreEqual(new[] { 4, 2, 3 }, shape);


        int[,,] expected =
        {
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            {
                { 7, 8, 9 },
                { 10, 11, 12 }
            },
            {
                { 13, 14, 15 },
                { 16, 17, 18 }
            },
            {
                { 19, 20, 21 },
                { 22, 23, 24 }
            }
        };

        for (int i = 0; i < jagged.Length; i++)
            for (int j = 0; j < jagged[i].Length; j++)
                for (int k = 0; k < jagged[i][j].Length; k++)
                    Assert.AreEqual(jagged[i][j][k], expected[i, j, k]);
    }

    [TestMethod]
    public void example_matrix()
    {
        #region doc_convert_matrix
        // Let's say we would like to convert  the following 
        // matrix of strings to a matrix of double values:
        string[,] from =
        {
            { "0", "1", "2" },
            { "3", "4", "5" },
        };

        // Using a convertor:
        double[,] a = Matrix.Convert(from, Double.Parse);

        // Using a default converter for the type:
        double[,] b = Matrix.Convert<string, double>(from);

        // Without using generics for the input type, will
        // also work for tensors (matrices with rank > 2)
        Array tensor = Matrix.Convert<double>(from);

        // Using universal converter:
        double[,] d = Matrix.To<double[,]>(from);
        double[][] e = Matrix.To<double[][]>(from);

        // When using an universal converter, we can also use 
        // it to squeeze / reshape the matrix to a new shape:
        double[,,] f = Matrix.To<double[,,]>(from);
        double[][][] g = Matrix.To<double[][][]>(from);
        #endregion

        Assert.IsTrue(a.IsEqual(b));
        Assert.IsTrue(a.IsEqual(tensor));
        Assert.IsTrue(a.IsEqual(d));
        Assert.IsTrue(a.IsEqual(e));
        Assert.IsTrue(a.IsEqual(e.Squeeze()));
        Assert.IsTrue(a.IsEqual(f.Squeeze()));
    }

    [TestMethod]
    public void example_jagged()
    {
        #region doc_convert_jagged
        // Let's say we would like to convert  the following 
        // matrix of strings to a matrix of double values:
        string[][] from =
        {
            new[] { "0", "1", "2" },
            new[] { "3", "4", "5" },
        };

        // Using a convertor:
        double[][] a = Jagged.Convert(from, Double.Parse);

        // Using a default converter for the type:
        double[][] b = Jagged.Convert<string, double>(from);

        // Without using generics for the input type, will
        // also work for tensors (matrices with rank > 2)
        Array tensor = Jagged.Convert<double>(from);

        // Using universal converter:
        double[,] d = Matrix.To<double[,]>(from);
        double[][] e = Matrix.To<double[][]>(from);

        // When using an universal converter, we can also use 
        // it to squeeze / reshape the matrix to a new shape:
        double[,,] f = Matrix.To<double[,,]>(from);
        double[][][] g = Matrix.To<double[][][]>(from);
        #endregion

        Assert.IsTrue(a.IsEqual(b));
        Assert.IsTrue(a.IsEqual(tensor));
        Assert.IsTrue(a.IsEqual(d));
        Assert.IsTrue(a.IsEqual(e));
        Assert.IsTrue(a.IsEqual(e.Squeeze()));
        Assert.IsTrue(a.IsEqual(f.Squeeze()));
    }

    [TestMethod]
    public void to_test()
    {
        double[,] a =
        {
            { 0, 1, 2 },
            { 3, 4, 5 },
        };

        {
            double[][] expected = a.ToJagged();
            double[][] actual = a.To<double[][]>();
            Assert.IsTrue(expected.IsEqual(actual));
        }

        {
            double[][] c = a.ToJagged();
            double[,] expected = c.ToMatrix();
            double[,] actual = c.To<double[,]>();
            Assert.IsTrue(expected.IsEqual(actual));
        }
    }

    [TestMethod]
    public void ScalarFromAnyToDouble()
    {
        double[,] from =
        {
            { 1 },
        };

        test(from, from);
        test(from.ToSingle(), from);
        test(from.ToInt16(), from);
        test(from.ToInt32(), from);
    }

    [TestMethod]
    public void MatrixFromAnyToDouble()
    {
        string[,] from =
        {
            { "0", "1", "2" },
            { "3", "4", "5" },
        };

        test(from, from.ToDouble());
        test(from.ToSingle(), from.ToDouble());
        test(from.ToDouble(), from.ToDouble());
        test(from.ToInt16(), from.ToDouble());
        test(from.ToInt32(), from.ToDouble());
    }

    [TestMethod]
    public void JaggedFromAnyToDouble()
    {
        string[][] from =
        {
            new[] { "0", "1", "2" },
            new[] { "3", "4", "5" },
        };

        test(from, from.ToDouble());
        test(from.ToSingle(), from.ToDouble());
        test(from.ToDouble(), from.ToDouble());
        test(from.ToInt16(), from.ToDouble());
        test(from.ToInt32(), from.ToDouble());
    }

    [TestMethod]
    public void VectorFromAnyToDouble()
    {
        string[] from = { "0", "1", "2" };

        test(from, from.ToDouble());
        test(from.ToSingle(), from.ToDouble());
        test(from.ToDouble(), from.ToDouble());
        test(from.ToInt16(), from.ToDouble());
        test(from.ToInt32(), from.ToDouble());
    }

    private static void test<T>(T[,] from, double[,] expected)
    {
        double[,] z = from.Convert<T, double>();
        double[,] a = Matrix.Convert<T, double>(from);
        double[][] b = Jagged.Convert<T, double>(from);
        double[,] c = (double[,])Matrix.Convert<double>(from);
        double[][] d = (double[][])Jagged.Convert<double>(from);
        double[,] e = Matrix.To<double[,]>(from);
        double[][] f = Matrix.To<double[][]>(from);

        Assert.IsTrue(expected.IsEqual(z));
        Assert.IsTrue(expected.IsEqual(a));
        Assert.IsTrue(expected.IsEqual(b));
        Assert.IsTrue(expected.IsEqual(c));
        Assert.IsTrue(expected.IsEqual(d));
        Assert.IsTrue(expected.IsEqual(e));
        Assert.IsTrue(expected.IsEqual(f));
    }

    private static void test<T>(T[][] from, double[][] expected)
    {
        double[][] z = from.Convert<T, double>();
        double[,] a = Matrix.Convert<T, double>(from);
        double[][] b = Jagged.Convert<T, double>(from);
        double[,] c = (double[,])Matrix.Convert<double>(from);
        double[][] d = (double[][])Jagged.Convert<double>(from);
        double[,] e = Matrix.To<double[,]>(from);
        double[][] f = Matrix.To<double[][]>(from);

        Assert.IsTrue(expected.IsEqual(z));
        Assert.IsTrue(expected.IsEqual(a));
        Assert.IsTrue(expected.IsEqual(b));
        Assert.IsTrue(expected.IsEqual(c));
        Assert.IsTrue(expected.IsEqual(d));
        Assert.IsTrue(expected.IsEqual(e));
        Assert.IsTrue(expected.IsEqual(f));
    }

    private static void test<T>(T[] from, double[] expected)
    {
        double[] actual1 = Matrix.Convert<T, double>(from);
        double[] actual2 = (double[])Matrix.Convert<double>(from);
        double[] actual3 = Matrix.To<double[]>(from);

        Assert.IsTrue(expected.IsEqual(actual1));
        Assert.IsTrue(expected.IsEqual(actual2));
        Assert.IsTrue(expected.IsEqual(actual3));
    }
}
