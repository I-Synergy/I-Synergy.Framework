using ISynergy.Framework.Mathematics.Abstractions;
using ISynergy.Framework.Mathematics.Matrices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Decompositions;
[TestClass]
public class GramSchmidtTest
{


    private TestContext testContextInstance;

    public TestContext TestContext
    {
        get
        {
            return testContextInstance;
        }
        set
        {
            testContextInstance = value;
        }
    }



    [TestMethod]
    public void ConstructorTest1()
    {
        double[,] value =
        {
           {  4, -2 },
           {  3,  1 },
        };

        SingularValueDecomposition svd = new(new Matrix<double>(value));
        Assert.AreEqual(2, svd.Rank);


        GramSchmidtOrthogonalization target = new(new Matrix<double>(value));

        double[,] Q = target.OrthogonalFactor;
        double[,] R = target.UpperTriangularFactor;

        double[,] inv = Q.Inverse();
        double[,] transpose = Q.Transpose();
        double[,] m = Matrix.Dot(new Matrix<double>(Q), new Matrix<double>(transpose));

        Assert.IsTrue(Matrix.IsEqual(m, Matrix.Identity(2), 1e-10));
        Assert.IsTrue(Matrix.IsEqual(inv, transpose, 1e-10));
    }

}
