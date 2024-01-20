using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests;
public partial class MatrixTest
{

    [TestMethod]
    public void set_value_specific1()
    {
        double[,] m;
        double[,] expected;

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d, null, 2, -1); // tested against numpy
        expected = new double[,]
        {
            { 0, 1, 10, 3 },
             { 4, 5, 10, 7 },
             { 8, 9, 10, 11 },
        };
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4).Transpose();
        m.Set(10d, 2, -1, null); // tested against numpy
        expected = new double[,]
        {
            { 0, 1, 10, 3 },
             { 4, 5, 10, 7 },
             { 8, 9, 10, 11 },
        }.Transpose();
        CollectionAssert.AreEqual(expected, m);
    }

    [TestMethod]
    public void set_value()
    {
        double[,] m;
        double[,] expected;

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d);
        expected = new double[,]
        {
             { 10, 10, 10, 10 },
             { 10, 10, 10, 10 },
             { 10, 10, 10, 10 },
        };
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d, null, 2, 3);
        expected = new double[,]
        {
             { 0, 1, 10, 3 },
             { 4, 5, 10, 7 },
             { 8, 9, 10, 11 },
        };
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d, 2, 3, null);
        expected = new double[,]
        {
            { 0, 1, 2, 3 },
            { 4, 5, 6, 7 },
            { 10, 10, 10, 10 },
        };
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d, 0, 1,
                  0, 2);
        expected = new double[,]
        {
            { 10, 10, 2, 3 },
            { 4, 5, 6, 7 },
            { 8, 9, 10, 11 },
        };
        CollectionAssert.AreEqual(expected, m);

        m.Set(42d, new[] { 2 }, new[] { 3 });
        expected = new double[,]
        {
             { 10, 10, 2, 3 },
             { 4, 5, 6, 7 },
             { 8, 9, 10, 42 },
        };
        CollectionAssert.AreEqual(expected, m);


        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d, null, 2, -1);
        expected = new double[,]
        {
            { 0, 1, 10, 3 },
             { 4, 5, 10, 7 },
             { 8, 9, 10, 11 },
        };
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4).Transpose();
        m.Set(10d, 2, -1, null);
        expected = new double[,]
        {
            { 0, 1, 10, 3 },
             { 4, 5, 10, 7 },
             { 8, 9, 10, 11 },
        }.Transpose();
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(10d, 1, -1, null);
        expected = new double[,]
        {
            { 0, 1, 2, 3 },
            { 10, 10, 10, 10 },
            { 8, 9, 10, 11 },
        };
        CollectionAssert.AreEqual(expected, m);

        m.Set(10d, 0, -1,
                  0, -2);
        expected = new double[,]
        {
            { 10, 10, 2, 3 },
            { 10, 10, 10, 10 },
            { 8, 9, 10, 11 },
        };
        CollectionAssert.AreEqual(expected, m);

        m = Vector.Interval(0, 12).Reshape(3, 4);
        m.Set(42d, new[] { -1 }, new[] { -2 });
        expected = new double[,]
        {
            { 0, 1, 2, 3 },
            { 4, 5, 6, 7 },
            { 8, 9, 42, 11 },
        };
        CollectionAssert.AreEqual(expected, m);
    }

    //[TestMethod]
    //public void set_matrix()
    //{
    //    int[,] m = Vector.Interval(0, 12).Reshape(3, 4);
    //    int[,] expected;

    //    m.Set(new[,] {
    //        { 10, 20 },
    //        { 30, 40 } });

    //    m.Set(new[,] {
    //        { 10, 20 },
    //        { 30, 40 } }, );

    //    m.Set(new[,] {
    //        { 10, 20 },
    //        { 30, 40 } });

    //    m.Set(new[,] {
    //        { 10, 20 },
    //        { 30, 40 } });
    //}

    [TestMethod]
    public void set_vector()
    {
        int[] m = Vector.Range(0, 5);
        int[] expected;

        m.Set(42, -1);
        expected = new int[]
        {
            0, 1, 2, 3, 42
        };
        CollectionAssert.AreEqual(expected, m);

        m.Set(10, 0);
        expected = new int[]
        {
            10, 1, 2, 3, 42
        };
        CollectionAssert.AreEqual(expected, m);

        m.Set(42, new[] { 4, 2 });
        expected = new int[]
        {
            10, 1, 42, 3, 42
        };
        CollectionAssert.AreEqual(expected, m);
    }

}
