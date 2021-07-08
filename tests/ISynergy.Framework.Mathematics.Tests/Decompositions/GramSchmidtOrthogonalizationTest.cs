namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Decompositions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var svd = new SingularValueDecomposition(value);
            Assert.AreEqual(2, svd.Rank);


            var target = new GramSchmidtOrthogonalization(value);

            var Q = target.OrthogonalFactor;
            var R = target.UpperTriangularFactor;

            double[,] inv = Q.Inverse();
            double[,] transpose = Q.Transpose();
            double[,] m = Matrix.Dot(Q, transpose);

            Assert.IsTrue(Matrix.IsEqual(m, Matrix.Identity(2), 1e-10));
            Assert.IsTrue(Matrix.IsEqual(inv, transpose, 1e-10));
        }

    }
}
