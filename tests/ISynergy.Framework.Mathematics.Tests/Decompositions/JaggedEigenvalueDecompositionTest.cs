﻿namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Decompositions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JaggedEigenvalueDecompositionTest
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
                    double[][] value = Jagged.Magic(n);

                    value[i][j] = double.NaN;

                    JaggedEigenvalueDecomposition target = new(value);
                }
            }
        }

        [TestMethod]
        public void EigenvalueDecompositionConstructorTest()
        {
            // Symmetric Test
            double[][] A =
            {
                new double[] { 4, 2 },
                new double[] { 2, 4 }
            };

            JaggedEigenvalueDecomposition target = new(A);

            double[][] D = target.DiagonalMatrix;
            double[][] Q = target.Eigenvectors;

            double[][] expectedD =
            {
                new double[] { 2, 0 },
                new double[] { 0, 6 }
            };

            double[][] expectedQ =
            {
               new double[] {  0.7071, 0.7071 },
               new double[] { -0.7071, 0.7071 }
            };


            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(expectedQ, Q, 0.0001));


            // Decomposition identity
            double[][] actualA = Matrix.Dot(Matrix.Dot(Q, D), Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 0.0001));

            Assert.AreSame(target.DiagonalMatrix, target.DiagonalMatrix);
        }



        [TestMethod]
        public void EigenvalueDecompositionConstructorTest2()
        {
            // Asymmetric Test
            double[][] A =
            {
                new double[] {  5, 2, 1 },
                new double[] {  1, 4, 1 },
                new double[] { -1, 2, 3 }
            };

            JaggedEigenvalueDecomposition target = new(A);
            double[][] D = target.DiagonalMatrix;
            double[][] Q = target.Eigenvectors;

            double[][] expectedD =
            {
                new double[] { 6, 0, 0 },
                new double[] { 0, 4, 0 },
                new double[] { 0, 0, 2 }
            };

            // Decomposition identity
            double[][] actualA = Matrix.Dot(Matrix.Dot(Q, D), Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 1e-5));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 1e-5));
        }
    }
}
