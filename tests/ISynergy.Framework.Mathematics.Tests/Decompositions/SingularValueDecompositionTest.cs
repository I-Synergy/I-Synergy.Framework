﻿using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Decompositions;
[TestClass]
public class SingularValueDecompositionTest
{

    [TestMethod]
    public void InverseTest()
    {
        double[,] value = new double[,]
        {
              { 1.0, 1.0 },
              { 2.0, 2.0 }
        };

        SingularValueDecomposition target = new(value);

        double[,] expected = new double[,]
        {
           { 0.1, 0.2 },
           { 0.1, 0.2 }
        };

        double[,] actual = target.Solve(Matrix.Identity(2));
        Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
        Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-3));

        actual = target.Inverse();
        Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
    }

    [TestMethod]
    public void InverseTest2()
    {
        int n = 5;

        double[,] I = Matrix.Identity(n);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double[,] value = Matrix.Magic(n);

                SingularValueDecomposition target = new(value);

                double[,] solution = target.Solve(I);
                double[,] inverse = target.Inverse();
                double[,] reverse = target.Reverse();

                Assert.IsTrue(Matrix.IsEqual(solution, inverse, 1e-4));
                Assert.IsTrue(Matrix.IsEqual(value, reverse, 1e-4));
            }
        }
    }

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

                SingularValueDecomposition target = new(value);

                double[,] solution = target.Solve(I);
                double[,] inverse = target.Inverse();

                Assert.IsTrue(Matrix.IsEqual(solution, inverse));
            }
        }
    }


    [TestMethod]
    public void SingularValueDecompositionConstructorTest1()
    {
        // This Test catches the bug in SingularValueDecomposition in the line
        //   for (int j = k + 1; j < nu; j++)
        // where it should be
        //   for (int j = k + 1; j < n; j++)


        // Test for m-x-n matrices where m < n. The available SVD
        // routine was not meant to be used in this case.

        double[,] value = new double[,]
         {
             { 1, 2 },
             { 3, 4 },
             { 5, 6 },
             { 7, 8 }
         }.Transpose(); // value is 2x4, having less rows than columns.

        SingularValueDecomposition target = new(value, true, true, false);

        double[,] actual = target.LeftSingularVectors.Dot(
            target.DiagonalMatrix).Dot(target.RightSingularVectors.Transpose());

        // Checking the decomposition
        Assert.IsTrue(Matrix.IsEqual(actual, value, 1e-2));
        Assert.IsTrue(Matrix.IsEqual(target.Reverse(), value, 1e-2));

        // Checking values
        double[,] U =
        {
            { -0.641423027995072, -0.767187395072177 },
            { -0.767187395072177,  0.641423027995072 },
        };

        // U should be equal
        Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));


        double[,] V = // economy svd
        {
            { -0.152483233310201,  0.822647472225661,  },
            { -0.349918371807964,  0.421375287684580,  },
            { -0.547353510305727,  0.0201031031435023, },
            { -0.744788648803490, -0.381169081397574,  },
        };

#pragma warning disable CS0618 // Type or member is obsolete
        double[,] matrix = target.RightSingularVectors.Submatrix(0, 3, 0, 1);
#pragma warning restore CS0618 // Type or member is obsolete

        // V can be different, but for the economy SVD it is often equal
        Assert.IsTrue(
            Matrix.IsEqual(matrix, V, 0.0001));

        double[,] S =
        {
            { 14.2690954992615, 0.000000000000000 },
            {  0.0000000000000, 0.626828232417543 },
        };

        // The diagonal values should be equal
        Assert.IsTrue(Matrix.IsEqual(target.Diagonal.First(2),
            Matrix.Diagonal(S), 0.001));
    }


    [TestMethod]
    public void SingularValueDecompositionConstructorTest3()
    {
        // Test using SVD assumption auto-correction feature.

        // Test for m-x-n matrices where m < n. The available SVD
        // routine was not meant to be used in this case.

        double[,] value = new double[,]
         {
             { 1, 2 },
             { 3, 4 },
             { 5, 6 },
             { 7, 8 }
         }.Transpose(); // value is 2x4, having less rows than columns.

        SingularValueDecomposition target = new(value, true, true, true);

        double[,] actual = Matrix.Dot(Matrix.Dot(target.LeftSingularVectors,
            Matrix.Diagonal(target.Diagonal)), target.RightSingularVectors.Transpose());

        // Checking the decomposition
        Assert.IsTrue(Matrix.IsEqual(actual, value, 0.01));
        Assert.IsTrue(Matrix.IsEqual(target.Reverse(), value, 1e-2));

        // Checking values
        double[,] U =
        {
            { 0.641423027995072, -0.767187395072177 },
            { 0.767187395072177,  0.641423027995072 },
        };

        // U should be equal despite some sign changes
        Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));


        double[,] V = // economy svd
        {
            {  0.152483233310201,  0.822647472225661,  },
            {  0.349918371807964,  0.421375287684580,  },
            {  0.547353510305727,  0.0201031031435023, },
            {  0.744788648803490, -0.381169081397574,  },
        };

        // V can be different, but for the economy SVD it is often equal
        Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V, 0.0001));


        double[,] S =
        {
            { 14.2690954992615, 0.000000000000000 },
            {  0.0000000000000, 0.626828232417543 },
        };

        // The diagonal values should be equal
        Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
    }


    [TestMethod]
    public void SingularValueDecompositionConstructorTest2()
    {
        // Test for m-x-n matrices where m > n (4 > 2)

        double[,] value = new double[,]
         {
             { 1, 2 },
             { 3, 4 },
             { 5, 6 },
             { 7, 8 }
         }; // value is 4x2, thus having more rows than columns


        SingularValueDecomposition target = new(value, true, true, false);

        double[,] actual = Matrix.Dot(Matrix.Dot(target.LeftSingularVectors,
            Matrix.Diagonal(target.Diagonal)), target.RightSingularVectors.Transpose());

        // Checking the decomposition
        Assert.IsTrue(Matrix.IsEqual(actual, value, 0.01));
        Assert.IsTrue(Matrix.IsEqual(target.Reverse(), value, 1e-2));

        double[,] U = // economy svd
        {
            {  0.152483233310201,  0.822647472225661,  },
            {  0.349918371807964,  0.421375287684580,  },
            {  0.547353510305727,  0.0201031031435023, },
            {  0.744788648803490, -0.381169081397574,  },
        };

        // U should be equal except for some sign changes
        Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));



        // Checking values
        double[,] V =
        {
            {  0.641423027995072, -0.767187395072177 },
            {  0.767187395072177,  0.641423027995072 },
        };

        // V should be equal except for some sign changes
        Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V, 0.0001));


        double[,] S =
        {
            { 14.2690954992615, 0.000000000000000 },
            {  0.0000000000000, 0.626828232417543 },
        };

        // The diagonal values should be equal
        Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
    }


    [TestMethod]
    public void SingularValueDecompositionConstructorTest4()
    {
        // Test using SVD assumption auto-correction feature
        // without computing the right singular vectors.

        double[,] value = new double[,]
         {
             { 1, 2 },
             { 3, 4 },
             { 5, 6 },
             { 7, 8 }
         }.Transpose(); // value is 2x4, having less rows than columns.

        SingularValueDecomposition target = new(value, true, false, true);


        // Checking values
        double[,] U =
        {
            { 0.641423027995072, -0.767187395072177 },
            { 0.767187395072177,  0.641423027995072 },
        };

        // U should be equal despite some sign changes
        Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U, 0.001));


        // Checking values
        double[,] V =
        {
            {  0.0, 0.0 },
            {  0.0, 0.0 },
            {  0.0, 0.0 },
            {  0.0, 0.0 },
        };

        // V should not have been computed.
        Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V));


        double[,] S =
        {
            { 14.2690954992615, 0.000000000000000 },
            {  0.0000000000000, 0.626828232417543 },
        };

        // The diagonal values should be equal
        Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
    }

    [TestMethod]
    public void SingularValueDecompositionConstructorTest5()
    {
        // Test using SVD assumption auto-correction feature
        // without computing the left singular vectors.

        double[,] value = new double[,]
         {
             { 1, 2 },
             { 3, 4 },
             { 5, 6 },
             { 7, 8 }
         }.Transpose(); // value is 2x4, having less rows than columns.

        SingularValueDecomposition target = new(value, false, true, true);


        // Checking values
        double[,] U =
        {
            { 0.0, 0.0 },
            { 0.0, 0.0 },
        };

        // U should not have been computed
        Assert.IsTrue(Matrix.IsEqual(target.LeftSingularVectors, U));


        double[,] V = // economy svd
        {
            {  0.152483233310201,  0.822647472225661,  },
            {  0.349918371807964,  0.421375287684580,  },
            {  0.547353510305727,  0.0201031031435023, },
            {  0.744788648803490, -0.381169081397574,  },
        };

        // V can be different, but for the economy SVD it is often equal
        Assert.IsTrue(Matrix.IsEqual(target.RightSingularVectors, V, 0.0001));



        double[,] S =
        {
            { 14.2690954992615, 0.000000000000000 },
            {  0.0000000000000, 0.626828232417543 },
        };

        // The diagonal values should be equal
        Assert.IsTrue(Matrix.IsEqual(target.Diagonal, Matrix.Diagonal(S), 0.001));
    }


    [TestMethod]
    public void SingularValueDecompositionConstructorTest6()
    {
        // Test using SVD assumption auto-correction feature in place

        double[,] value1 =
        {
            { 2.5,  2.4 },
            { 0.5,  0.7 },
            { 2.2,  2.9 },
            { 1.9,  2.2 },
            { 3.1,  3.0 },
            { 2.3,  2.7 },
            { 2.0,  1.6 },
            { 1.0,  1.1 },
            { 1.5,  1.6 },
            { 1.1,  0.9 }
        };

        double[,] value2 = value1.Transpose();

        double[,] cvalue1 = value1.Copy();
        double[,] cvalue2 = value2.Copy();

        SingularValueDecomposition target1 = new(cvalue1, true, true, true, true);
        SingularValueDecomposition target2 = new(cvalue2, true, true, true, true);

        Assert.IsFalse(cvalue1.IsEqual(value1, 1e-5));
        // Assert.IsFalse(cvalue2.IsEqual(value2, 1e-5));

        Assert.IsTrue(target1.LeftSingularVectors.IsEqual(target2.RightSingularVectors));
        Assert.IsTrue(target1.RightSingularVectors.IsEqual(target2.LeftSingularVectors));
        Assert.IsTrue(target1.DiagonalMatrix.IsEqual(target2.DiagonalMatrix));

        Assert.IsTrue(Matrix.IsEqual(target1.Reverse(), value1, 1e-5));
        Assert.IsTrue(Matrix.IsEqual(target2.Reverse(), value2, 1e-5));

        Assert.AreSame(target1.DiagonalMatrix, target1.DiagonalMatrix);
    }

    [TestMethod]
    public void SingularValueDecompositionConstructorTest7()
    {
        int count = 100;
        double[,] value = new double[count, 3];
        double[] output = new double[count];

        for (int i = 0; i < count; i++)
        {
            double x = i + 1;
            double y = 2 * (i + 1) - 1;
            value[i, 0] = x;
            value[i, 1] = y;
            value[i, 2] = 1;
            output[i] = 4 * x - y + 3;
        }



        SingularValueDecomposition target = new(value,
            computeLeftSingularVectors: true,
            computeRightSingularVectors: true);

        {
            double[,] expected = value;
            double[,] actual = Matrix.Dot(Matrix.Dot(target.LeftSingularVectors,
                Matrix.Diagonal(target.Diagonal)), target.RightSingularVectors.Transpose());

            // Checking the decomposition
            Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-8));
            Assert.IsTrue(Matrix.IsEqual(target.Reverse(), expected, 1e-8));
        }

        {
            double[] solution = target.Solve(output);

            double[] expected = output;
            double[] actual = value.Dot(solution);

            Assert.IsTrue(Matrix.IsEqual(actual, expected, 1e-8));
        }
    }

    [TestMethod]
    public void issue_614()
    {
        // https://github.com/accord-net/framework/issues/614

        double[,] A =
        {
            { 1 },
            { 0 }
        };

        double[,] B =
        {
            { 1 },
            { 0 }
        };


        double[,] X = Matrix.Solve(A, B, true);

        double[,] expected =
        {
            { 1 }
        };

        Assert.IsTrue(expected.IsEqual(X));

        X = new SingularValueDecomposition(A).Solve(B);

        Assert.IsTrue(expected.IsEqual(X));
    }

}
