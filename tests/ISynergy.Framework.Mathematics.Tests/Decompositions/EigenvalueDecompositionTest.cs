namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Decompositions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EigenvalueDecompositionTest
    {

        [TestMethod]
        public void InverseTestNaN()
        {
            int n = 5;

            var I = Matrix.Identity(n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double[,] value = Matrix.Magic(n);

                    value[i, j] = double.NaN;

                    var target = new EigenvalueDecomposition(value);
                }
            }
        }

        [TestMethod]
        public void EigenvalueDecompositionConstructorTest()
        {
            // Symmetric test
            double[,] A =
            {
                { 4, 2 },
                { 2, 4 }
            };

            var target = new EigenvalueDecomposition(A);

            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[,] expectedD =
            {
                { 2, 0 },
                { 0, 6 }
            };

            double[,] expectedQ =
            {
               {  0.7071, 0.7071 },
               { -0.7071, 0.7071 }
            };


            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(expectedQ, Q, 0.0001));


            // Decomposition identity
            var inv = Q.Inverse();
            var actualA = Matrix.Dot(Matrix.Dot(Q, D), inv);


            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 0.00001));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 0.0001));

            Assert.AreSame(target.DiagonalMatrix, target.DiagonalMatrix);
        }



        [TestMethod]
        public void EigenvalueDecompositionConstructorTest2()
        {
            // Asymmetric test
            double[,] A =
            {
                {  5, 2, 1 },
                {  1, 4, 1 },
                { -1, 2, 3 }
            };

            var target = new EigenvalueDecomposition(A);
            var D = target.DiagonalMatrix;
            var Q = target.Eigenvectors;

            double[,] expectedD =
            {
                { 6, 0, 0 },
                { 0, 4, 0 },
                { 0, 0, 2 }
            };

            // Decomposition identity
            var actualA = Matrix.Dot(Matrix.Dot(Q, D), Q.Inverse());

            Assert.IsTrue(Matrix.IsEqual(expectedD, D, 1e-5));
            Assert.IsTrue(Matrix.IsEqual(A, actualA, 1e-5));
            //var actual = target.Reverse();
            //Assert.IsTrue(Matrix.IsEqual(A, actual, 1e-5));
        }
    }
}
