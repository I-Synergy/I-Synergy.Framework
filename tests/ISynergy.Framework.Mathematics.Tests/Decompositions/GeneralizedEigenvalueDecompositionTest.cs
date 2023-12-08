namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics;
using ISynergy.Framework.Mathematics.Decompositions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GeneralizedEigenvalueDecompositionTest
{

    [TestMethod]
    public void GeneralizedEigenvalueDecompositionConstructorTest()
    {
        // Suppose we have the following 
        // matrices A and B shown below:

        double[,] A =
        {
            { 1, 2, 3},
            { 8, 1, 4},
            { 3, 2, 3}
        };

        double[,] B =
        {
            { 5, 1, 1},
            { 1, 5, 1},
            { 1, 1, 5}
        };

        // Now, suppose we would like to find values for λ 
        // that are solutions for the equation det(A - λB) = 0

        // For this, we can use a Generalized Eigendecomposition
        GeneralizedEigenvalueDecomposition gevd = new(A, B);

        // Now, if A and B are Hermitian and B is positive
        // -definite, then the eigenvalues λ will be real:
        double[] lambda = gevd.RealEigenvalues;

        // Check if they are indeed a solution:
        for (int i = 0; i < lambda.Length; i++)
        {
            // Compute the determinant equation show above
            double det = Matrix.Determinant(A.Subtract(lambda[i].Multiply(B))); // almost zero

            Assert.IsTrue(det < 1e-6);
        }


        double[,] expectedVectors =
        {
            { 0.427490473174445, -0.459244062074000, -0.206685960405416 },
            { 1,    1,  -1},
            { 0.615202547759401,    -0.152331764458173, 0.779372135871111}
        };

        double[,] expectedValues =
        {
          {1.13868666711946,    0,  0 },
          {0, -0.748168231839396,   0},
          {0,   0,  -0.104804149565775}
        };


        Assert.IsTrue(Matrix.IsEqual(gevd.Eigenvectors, expectedVectors, 0.00000000001));
        Assert.IsTrue(Matrix.IsEqual(gevd.DiagonalMatrix, expectedValues, 0.00000000001));
    }

    [TestMethod]
    public void GeneralizedEigenvalueDecompositionConstructorTest2()
    {
        double[,] A = Matrix.Identity(100);
        double[,] B = Matrix.Identity(100);

        GeneralizedEigenvalueDecomposition gevd = new(A, B);

        double[,] expectedVectors = Matrix.Identity(100);
        double[,] expectedValues = Matrix.Identity(100);

        Assert.IsTrue(Matrix.IsEqual(gevd.Eigenvectors, expectedVectors));
        Assert.IsTrue(Matrix.IsEqual(gevd.DiagonalMatrix, expectedValues));
    }

    [TestMethod]
    public void GeneralizedEigenvalueDecompositionConstructorTest3()
    {
        for (int i = 0; i < 10000; i++)
        {
            for (int j = 1; j < 6; j++)
            {
                double[,] A = Matrix.Random(j, j, -1.0, 1.0);
                double[,] B = Matrix.Random(j, j, -1.0, 1.0);

                GeneralizedEigenvalueDecomposition gevd = new(A, B);

                double[,] V = gevd.Eigenvectors;
                double[,] D = gevd.DiagonalMatrix;

                // A*V = B*V*D
                double[,] AV = Matrix.Dot(A, V);
                double[,] BVD = Matrix.Dot(Matrix.Dot(B, V), D);

                Assert.IsTrue(Matrix.IsEqual(AV, BVD, 0.0000001));
            }
        }

        for (int i = 0; i < 100; i++)
        {
            int j = 50;
            double[,] A = Matrix.Random(j, j, -1.0, 1.0);
            double[,] B = Matrix.Random(j, j, -1.0, 1.0);

            GeneralizedEigenvalueDecomposition gevd = new(A, B);

            double[,] V = gevd.Eigenvectors;
            double[,] D = gevd.DiagonalMatrix;

            // A*V = B*V*D
            double[,] AV = Matrix.Dot(A, V);
            double[,] BVD = Matrix.Dot(Matrix.Dot(B, V), D);

            Assert.IsTrue(Matrix.IsEqual(AV, BVD, 0.0000001));
        }
    }

    [TestMethod]
    public void GeneralizedEigenvalueDecompositionConstructorTest4()
    {
        double[,] A = new double[3, 3];
        A[0, 0] = 2.6969840958234776;
        A[0, 1] = 3.0761868753825254;
        A[0, 2] = -1.9236284084262458;
        A[1, 0] = -0.09975623250927601;
        A[1, 1] = 3.1520214626342158;
        A[1, 2] = 2.3928828222643972;
        A[2, 0] = 5.2090689722490815;
        A[2, 1] = 2.32098631016956;
        A[2, 2] = 5.522974475996091;


        double[,] B = new double[3, 3];
        B[0, 0] = -16.753710484948808;
        B[0, 1] = -14.715495544818925;
        B[0, 2] = -41.589502695291074;
        B[1, 0] = -31.78618974973736;
        B[1, 1] = -14.30788463834109;
        B[1, 2] = -18.388254830328865;
        B[2, 0] = -3.2512542741611838;
        B[2, 1] = -18.774698582838617;
        B[2, 2] = -1.5640121915210088;


        GeneralizedEigenvalueDecomposition gevd = new(A, B);

        double[,] V = gevd.Eigenvectors;
        double[,] D = gevd.DiagonalMatrix;

        // A*V = B*V*D
        double[,] AV = Matrix.Dot(A, V);
        double[,] BVD = Matrix.Dot(Matrix.Dot(B, V), D);
        Assert.IsTrue(Matrix.IsEqual(AV, BVD, 0.000001));

        double[,] expectedVectors =
        {
            {1, -0.120763598920560, -0.636412048994645},
        {-0.942794724207834,    -1, -0.363587951005355},
            {   -0.0572052757921662,    -0.0606762790704327,    1},
        };

        double[,] expectedValues =
        {
            {0.186046511627907, 0,  0},
            {0, -0.170549605858232, 0},
            {   0,  0,  0.186046511627907}
        };

        //Assert.IsTrue(Matrix.IsEqual(V, expectedVectors,0.001));
        Assert.IsTrue(Matrix.IsEqual(D, expectedValues, 0.00001));

    }

}
