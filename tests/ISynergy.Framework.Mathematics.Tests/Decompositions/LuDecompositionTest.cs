using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Decompositions;
[TestClass]
public class LuDecompositionTest
{

    [TestMethod]
    public void InverseTestNaN()
    {
        int n = 5;

        double[,] I = Matrix.Identity(n);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double[,] value = Matrix.Magic(n);

                value[i, j] = double.NaN;

                LuDecomposition target = new(value);
                Assert.IsTrue(Matrix.IsEqual(target.Solve(I), target.Inverse()));

                JaggedLuDecomposition target2 = new(value.ToJagged());
                Assert.IsTrue(Matrix.IsEqual(target2.Solve(I.ToJagged()), target2.Inverse()));
            }
        }
    }


    [TestMethod]
    public void SolveTest1()
    {
        double[,] value =
        {
           {  2,  3,  0 },
           { -1,  2,  1 },
           {  0, -1,  3 }
        };

        double[] rhs = [5, 0, 1];

        double[] expected =
        [
            1.6522,
            0.5652,
            0.5217
        ];

        LuDecomposition target = new(value);
        double[] actual = target.Solve(rhs);
        Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
        Assert.IsTrue(Matrix.IsEqual(value, target.Reverse()));

        JaggedLuDecomposition target2 = new(value.ToJagged());
        actual = target2.Solve(rhs);
        Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
        Assert.IsTrue(Matrix.IsEqual(value, target2.Reverse()));
    }

    [TestMethod]
    public void InverseTest()
    {
        double[,] value =
        {
           {  2,  3,  0 },
           { -1,  2,  1 },
           {  0, -1,  3 }
        };

        double[,] expectedInverse =
        {
            { 0.3043,   -0.3913,    0.1304 },
            { 0.1304,    0.2609,   -0.0870 },
            { 0.0435,    0.0870,    0.3043 },
        };

        LuDecomposition target = new(value);
        double[,] actualInverse = target.Inverse();
        Assert.IsTrue(Matrix.IsEqual(expectedInverse, actualInverse, 0.001));
        Assert.IsTrue(Matrix.IsEqual(value, target.Reverse()));

        JaggedLuDecomposition target2 = new(value.ToJagged());
        actualInverse = target2.Inverse().ToMatrix();
        Assert.IsTrue(Matrix.IsEqual(expectedInverse, actualInverse, 0.001));
        Assert.IsTrue(Matrix.IsEqual(value, target2.Reverse()));
    }

    [TestMethod]
    public void SolveTest()
    {
        double[,] value =
        {
           {  2,  3,  0 },
           { -1,  2,  1 },
           {  0, -1,  3 }
        };

        double[,] rhs =
        {
            { 1, 2, 3 },
            { 3, 2, 1 },
            { 5, 0, 1 },
        };

        double[,] expected =
        {
            { -0.2174,   -0.1739,    0.6522 },
            {  0.4783,    0.7826,    0.5652 },
            {  1.8261,    0.2609,    0.5217 },
        };

        Assert.IsTrue(Matrix.IsEqual(expected, new LuDecomposition(value).Solve(rhs), 1e-3));
        Assert.IsTrue(Matrix.IsEqual(expected, new JaggedLuDecomposition(value.ToJagged()).Solve(rhs.ToJagged()), 1e-3));
    }

    [TestMethod]
    public void SolveTest3()
    {
        double[,] value =
        {
           {  2.000,  3.000,  0.000 },
           { -1.000,  2.000,  1.000 },
        };

        LuDecomposition target = new(value);

        double[,] L = target.LowerTriangularFactor;
        double[,] U = target.UpperTriangularFactor;

        double[,] expectedL =
        {
           {  1.000, 0.000 },
           { -0.500, 1.000 },
        };

        double[,] expectedU =
        {
            { 2.000, 3.000, 0.000 },
            { 0.000, 3.500, 1.000  },
        };

        Assert.IsTrue(Matrix.IsEqual(expectedL, L, 1e-3));
        Assert.IsTrue(Matrix.IsEqual(expectedU, U, 1e-3));
    }

    [TestMethod]
    public void SolveTest4()
    {
        double[,] value =
        {
            { 2.1, 3.1 },
            { 1.6, 4.2 },
        };

        double[] rhs = [6.1, 4.3];

        double[] expected = [3.1839, -0.1891];

        LuDecomposition target1 = new(value);
        JaggedLuDecomposition target2 = new(value.ToJagged());

        Assert.IsTrue(Matrix.IsEqual(expected, target1.Solve(rhs), 1e-3));
        Assert.IsTrue(Matrix.IsEqual(expected, target2.Solve(rhs), 1e-3));
    }

    [TestMethod]
    public void SolveTest5()
    {
        double[,] value =
        {
            { 2.1, 3.1 },
            { 1.6, 4.2 },
            { 2.1, 5.1 },
        };

        double[] rhs = [6.1, 4.3, 2.1];

        double[] expected = [3.1839, -0.1891];

        LuDecomposition target = new(value);

        bool thrown = false;
        try
        {
            double[] actual = target.Solve(rhs);
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }

        Assert.IsTrue(thrown);
    }

    [TestMethod]
    public void SolveTransposeTest()
    {
        double[,] a =
        {
            { 2, 1, 4 },
            { 6, 2, 2 },
            { 0, 1, 6 },
        };

        double[,] b =
        {
            { 1, 0, 7 },
            { 5, 2, 1 },
            { 1, 5, 2 },
        };

        double[,] expected =
        {
             { 0.5062,    0.2813,    0.0875 },
             { 0.1375,    1.1875,   -0.0750 },
             { 0.8063,   -0.2188,    0.2875 },
        };

        Assert.IsTrue(Matrix.IsEqual(expected, new LuDecomposition(b, true).SolveTranspose(a), 1e-3));
        Assert.IsTrue(Matrix.IsEqual(expected, new JaggedLuDecomposition(b.ToJagged(), true).SolveTranspose(a.ToJagged()), 1e-3));

        LuDecomposition target = new(b, true);
        int[] p = target.PivotPermutationVector;
        int[] idx = p.ArgSort();

#pragma warning disable CS0618 // Type or member is obsolete
        double[,] r = target.LowerTriangularFactor.Dot(target.UpperTriangularFactor).Submatrix(idx, null).Transpose();
#pragma warning restore CS0618 // Type or member is obsolete

        Assert.IsTrue(Matrix.IsEqual(b, r, 1e-3));
        Assert.IsTrue(Matrix.IsEqual(b.Transpose(), target.Reverse(), 1e-3));
        Assert.IsTrue(Matrix.IsEqual(b.Transpose(), new JaggedLuDecomposition(b.ToJagged(), true).Reverse(), 1e-3));
    }

    [TestMethod]
    public void LuDecompositionConstructorTest()
    {
        #region doc_ctor
        // Let's say we would like to compute the
        // LU decomposition of the following matrix:
        double[,] matrix =
        {
           {  2, -1,  0 },
           { -1,  2, -1 },
           {  0, -1,  2 }
        };

        // Compute the LU decomposition with:
        LuDecomposition lu = new(matrix);


        // Retrieve the lower triangular factor L:
        double[,] L = lu.LowerTriangularFactor;

        // Should be equal to
        double[,] expectedL =
        {
            {  1.0000,         0,         0 },
            { -0.5000,    1.0000,         0 },
            {       0,   -0.6667,    1.0000 },
        };


        // Retrieve the upper triangular factor U:
        double[,] U = lu.UpperTriangularFactor;

        // Should be equal to
        double[,] expectedU =
        {
            { 2.0000,   -1.0000,         0 },
            {      0,    1.5000,   -1.0000 },
            {      0,         0,    1.3333 },
         };


        // Certify that the decomposition has worked as expected by
        // trying to reconstruct the original matrix with R = L * U:
        double[,] reconstruction = L.Dot(U);

        // reconstruction should be equal to
        // {
        //     {  2, -1,  0 },
        //     { -1,  2, -1 },
        //     {  0, -1,  2 }
        // };
        #endregion


        Assert.IsTrue(Matrix.IsEqual(matrix, reconstruction, 1e-4));
        Assert.IsTrue(Matrix.IsEqual(expectedL, L, 1e-4));
        Assert.IsTrue(Matrix.IsEqual(expectedU, U, 1e-4));


        lu = new LuDecomposition(matrix.Transpose(), true);

        L = lu.LowerTriangularFactor;
        U = lu.UpperTriangularFactor;

        Assert.IsTrue(Matrix.IsEqual(expectedL, L, 0.001));
        Assert.IsTrue(Matrix.IsEqual(expectedU, U, 0.001));
    }

    [TestMethod]
    public void LogDeterminantTest()
    {
        LuDecomposition lu = new(CholeskyDecompositionTest.bigmatrix);
        Assert.AreEqual(0, lu.Determinant);
        Assert.AreEqual(-2224.8931093738875, lu.LogDeterminant, 1e-12);
        Assert.IsTrue(lu.Nonsingular);
    }

    [TestMethod]
    public void DeterminantTest()
    {
        double[,] value =
        {
           {  2,  3,  0 },
           { -1,  2,  1 },
           {  0, -1,  3 }
        };

        LuDecomposition lu = new(value);
        Assert.AreEqual(23, lu.Determinant);
        Assert.IsTrue(lu.Nonsingular);
    }

    [TestMethod]
    public void JaggedDeterminantTest()
    {
        double[,] value =
        {
           {  2,  3,  0 },
           { -1,  2,  1 },
           {  0, -1,  3 }
        };

        JaggedLuDecomposition lu = new(value.ToJagged());
        Assert.AreEqual(23, lu.Determinant);
        Assert.IsTrue(lu.Nonsingular);
    }

    [TestMethod]
    public void LogDeterminantTest2()
    {
        double[,] value =
        {
           {  2,  3,  0 },
           { -1,  2,  1 },
           {  0, -1,  3 }
        };

        LuDecomposition lu = new(value);
        Assert.AreEqual(23, lu.Determinant);

        double expected = Math.Log(23);
        double actual = lu.LogDeterminant;
        Assert.AreEqual(expected, actual);
    }

}
