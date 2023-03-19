namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Decompositions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QrDecompositionTest
    {

        [TestMethod]
        public void InverseTest()
        {
            int n = 5;

            double[,] I = Matrix.Identity(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double[,] value = Matrix.Magic(n);

                    QrDecomposition target = new(value);
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

                    QrDecomposition target = new(value);
                    double[,] solution = target.Solve(I);
                    double[,] inverse = target.Inverse();

                    Assert.IsTrue(Matrix.IsEqual(solution, inverse));
                }
            }
        }

        [TestMethod]
        public void QrDecompositionConstructorTest()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };


            QrDecomposition target = new(value);

            // Decomposition Identity
            double[,] Q = target.OrthogonalFactor;
            double[,] QQt = Q.DotWithTransposed(Q);
            Assert.IsTrue(Matrix.IsEqual(QQt, Matrix.Identity(3), 1e-6));
            Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-6));


            // Linear system solving
            double[,] B = Matrix.ColumnVector(new double[] { 1, 2, 3 });
            double[,] expected = Matrix.ColumnVector(new double[] { 2.5, 4.0, 3.5 });
            double[,] actual = target.Solve(B);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
        }

        [TestMethod]
        public void QrDecompositionCachingTest()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };

            QrDecomposition target = new(value);

            // Decomposition Identity
            double[,] Q1 = target.OrthogonalFactor;
            double[,] Q2 = target.OrthogonalFactor;

            double[,] utf1 = target.UpperTriangularFactor;
            double[,] utf2 = target.UpperTriangularFactor;

            Assert.AreSame(Q1, Q2);
            Assert.AreSame(utf1, utf2);
        }


        [TestMethod]
        public void InverseTest2()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };

            double[,] expected =
            {
                { 0.7500,    0.5000,    0.2500},
                { 0.5000,    1.0000,    0.5000},
                { 0.2500,    0.5000,    0.7500},
            };


            QrDecomposition target = new(value);

            double[,] actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));

            target = new QrDecomposition(value.Transpose(), true);
            actual = target.Inverse();
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
        }

        [TestMethod]
        public void SolveTest()
        {
            double[,] value =
            {
               {  2, -1,  0 },
               { -1,  2, -1 },
               {  0, -1,  2 }
            };

            double[] b = { 1, 2, 3 };

            double[] expected = { 2.5000, 4.0000, 3.5000 };

            QrDecomposition target = new(value);
            double[] actual = target.Solve(b);

            Assert.IsTrue(Matrix.IsEqual(expected, actual, 0.0000000000001));
        }

        [TestMethod]
        public void SolveTest2()
        {
            // Example from Lecture notes for MATHS 370: Advanced Numerical Methods
            // http://www.math.auckland.ac.nz/~sharp/370/qr-solving.pdf

            double[,] value =
            {
                { 1,           0,           0 },
                { 1,           7,          49 },
                { 1,          14,         196 },
                { 1,          21,         441 },
                { 1,          28,         784 },
                { 1,          35,        1225 },
            };

            // Matrices
            {
                double[,] b =
                {
                    { 4 },
                    { 1 },
                    { 0 },
                    { 0 },
                    { 2 },
                    { 5 },
                };

                double[,] expected =
                {
                    { 3.9286  },
                    { -0.5031 },
                    { 0.0153  },
                };

                QrDecomposition target = new(value);
                double[,] actual = target.Solve(b);

                Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
                Assert.IsTrue(Matrix.IsEqual(value, target.Reverse(), 1e-6));


                JaggedQrDecomposition target2 = new(value.ToJagged());
                double[][] actual2 = target2.Solve(b.ToJagged());

                Assert.IsTrue(Matrix.IsEqual(expected, actual2, atol: 1e-4));
                Assert.IsTrue(Matrix.IsEqual(value, target2.Reverse(), 1e-6));
            }

            // Vectors
            {
                double[] b = { 4, 1, 0, 0, 2, 5 };
                double[] expected = { 3.9286, -0.5031, 0.0153 };

                QrDecomposition target = new(value);
                double[] actual = target.Solve(b);

                Assert.IsTrue(Matrix.IsEqual(expected, actual, atol: 1e-4));
            }
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

            QrDecomposition target = new(b, true);
            double[,] actual = target.SolveTranspose(a);
            Assert.IsTrue(Matrix.IsEqual(expected, actual, 1e-3));
            Assert.IsTrue(Matrix.IsEqual(b.Transpose(), target.Reverse(), 1e-6));

        }
    }
}
